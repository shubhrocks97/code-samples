using System;
using System.IO;
using ONQHL7.PluginInterfaces;
using ONQHL7.GlobalDb.Interfaces;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ONQHL7.QueueDataProcessor.Models;
using ONQHL7.QueueDataProcessor.Exceptions;
using ONQHL7.QueueDataProcessor.CommonMethods;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ONQHL7.GlobalDb.Models;

namespace ONQHL7.QueueDataProcessor.Services
{
    public class ProcessQueueData : IProcessQueueData
    {
        private readonly IOptions<AppSettings> m_config;
        private readonly IOHDataRepository m_ohDataRepo;
        private readonly ILogger<ProcessQueueData> m_logger;
        private static List<IHL7MessageGenerator> m_plugins = null;

        public ProcessQueueData(IOptions<AppSettings> a_mConfig, IOHDataRepository a_mOHDataRepo, ILogger<ProcessQueueData> a_mLogger)
        {
            m_config = a_mConfig;
            m_ohDataRepo = a_mOHDataRepo;
            m_logger = a_mLogger;
        }
        public bool SendQueueDataToPlugin()
        {
            try
            {
                m_plugins = CommonMethod.ReadAssemblies(m_config);

                foreach (var plugin in m_plugins)
                {
                    string sBlobContainer = m_config.Value.BlobContainer;
                    string sMessageFilePath = m_config.Value.MsgFilesPath;
                    string sBlobSasUrl = m_config.Value.BlobSasUrl;
                    //Insert the approved status data into HL7Queue table captured by CDC
                    m_ohDataRepo.InsertUpdatedDataToHL7QueueTable(m_config.Value.LimitOfSampleId);

                    //

                    //Get list of New Status SampleId from HL7Queue Table 
                    var SampleIds = m_ohDataRepo.GetListOfSampleId(m_config.Value.LimitOfSampleId);

                    if (SampleIds != null && SampleIds.Count > 0)
                    {
                        for (int i = 0; i < SampleIds.Count; i++)
                        {
                            //Get all information from different table to generate HL7Message on the basis of SampleId
                            var sampleId = JsonConvert.DeserializeObject<HL7QueueResponse>(JsonConvert.SerializeObject(SampleIds[i])).SampleId;
                            var Id = JsonConvert.DeserializeObject<HL7QueueResponse>(JsonConvert.SerializeObject(SampleIds[i])).Id.ToString();
                            var oMessageData = m_ohDataRepo.GetMessageData(sampleId);
                            try
                            {
                                if (oMessageData != null)
                                {
                                    m_logger.LogInformation($"Generating HL7Message for SampleId=> {JsonConvert.DeserializeObject<HL7QueueResponse>(JsonConvert.SerializeObject(SampleIds[i])).SampleId}");
                                    //Update status to Inprogress
                                    m_ohDataRepo.UpdateStatus(sampleId, Id, (char)StausType.InProgress);
                                    //Generate the HL7 Message and store data into file and also in Blob storage
                                    string sGeneratedFile = plugin.GenerateH7Message(oMessageData, sMessageFilePath);


                                    if (!string.IsNullOrEmpty(sGeneratedFile))
                                    {
                                        m_logger.LogInformation($"File successfully generated");

                                        //Update status to success
                                        m_ohDataRepo.UpdateStatus(sampleId, Id, (char)StausType.Success);

                                        m_logger.LogInformation($"Storing file=>{sGeneratedFile} into blob storage");
                                        //Upload file to azure blob storage
                                        bool bIsUploaded = plugin.UploadMessageToBlobStorage(sMessageFilePath, sGeneratedFile, sBlobSasUrl, sBlobContainer ,out string sErrorMsg);

                                        

                                        if (bIsUploaded)
                                        {
                                            m_logger.LogInformation($"File sucessfully uploaded");
                                            m_ohDataRepo.UpdateFileSucessfullyUploaded(sampleId,Id);

                                            // update smsQueue table
                                            string a_nSampleId = sampleId;
                                            var a_nHL7Id = JsonConvert.DeserializeObject<HL7QueueResponse>(JsonConvert.SerializeObject(SampleIds[i])).Id;
                                            string a_nFirstName = JsonConvert.DeserializeObject<SMSQueueModel>(JsonConvert.SerializeObject(oMessageData[1])).GivenName;
                                            string DOB = JsonConvert.DeserializeObject<SMSQueueModel>(JsonConvert.SerializeObject(oMessageData[1])).DateOfBirth.ToString();
                                            string a_nContactNo = JsonConvert.DeserializeObject<SMSQueueModel>(JsonConvert.SerializeObject(oMessageData[1])).PhoneNumber;
                                            string a_nObservationTime = Convert.ToDateTime(JsonConvert.DeserializeObject<SMSQueueModel>(JsonConvert.SerializeObject(oMessageData[3])).ObservationDateAndTime).ToString("yyyy-MM-dd HH:mm:ss");
                                            string a_nObservationResult = JsonConvert.DeserializeObject<SMSQueueModel>(JsonConvert.SerializeObject(oMessageData[4])).ObservationValue;
                                            string a_nARRLLabNo = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(m_ohDataRepo.getARRLLabNo(sampleId)))[0];

                                            m_ohDataRepo.UpdateSMSQueue(a_nHL7Id, a_nFirstName, DOB, a_nARRLLabNo, a_nSampleId, a_nContactNo, a_nObservationTime, a_nObservationResult, m_config.Value.ScheduleMessageMinutes);

                                            m_logger.LogInformation($"Record added to SMSQueue");
                                        }
                                        else
                                        {
                                            m_logger.LogError($"Unable to upload file into blob storage: {sErrorMsg}");
                                            m_ohDataRepo.CaptureExceptionAndUpdateExceptionStatusType(sampleId, Id,ExceptionStatusType.StorageException.ToString());
                                        }

                                        //Delete local HL7Message file
                                        DeleteFile($"{sMessageFilePath}{sGeneratedFile}");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                m_ohDataRepo.CaptureExceptionAndUpdateStatus(sampleId, Id, (char)StausType.Failed, ex.Message);
                                m_ohDataRepo.CaptureExceptionAndUpdateExceptionStatusType(sampleId, Id,ExceptionStatusType.FileException.ToString());
                                m_logger.LogError($"{ExceptionMessage.MSG_GENEARATION_ERROR} because {ex.Message}=>{ex.StackTrace}");
                            }
                        }
                    }
                    else
                    {
                        m_logger.LogWarning($"{ExceptionMessage.NOT_FOUND_ID}");
                    }
                }
            }
            catch (OHException ex)
            {
                m_logger.LogError($"{ExceptionMessage.MSG_GENEARATION_ERROR} because {ex.Message}=>{ex.StackTrace}");
            }
            return true;
        }

        public void DeleteFile(string sFilePath)
        {
            try
            {
                if (File.Exists(sFilePath))
                {
                    File.Delete(sFilePath);
                    m_logger.LogInformation($"File deleted from local storage");
                }
                else
                {
                    m_logger.LogInformation($"Unable to find file to delete from local storage");
                }
            }
            catch (Exception ex)
            {
                m_logger.LogError($"Unable to delete file => {ex.Message}");
            }
        }
    }
}