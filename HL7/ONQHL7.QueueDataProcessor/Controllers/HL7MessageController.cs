using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ONQHL7.GlobalDb.Interfaces;
using ONQHL7.PluginInterfaces;
using ONQHL7.QueueDataProcessor.CommonMethods;
using ONQHL7.QueueDataProcessor.Exceptions;
using ONQHL7.QueueDataProcessor.Models;
using ONQHL7.QueueDataProcessor.Models.KendoUiModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ONQHL7.QueueDataProcessor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HL7MessageController : ControllerBase
    {
        private readonly IOptions<AppSettings> m_config;
        private readonly IOHDataRepository m_ohDataRepo;
        private readonly ILogger<HL7MessageController> m_logger;
        private static List<IHL7MessageGenerator> m_plugins = null;
        public HL7MessageController(IOptions<AppSettings> a_mConfig, IOHDataRepository a_mOHDataRepo, ILogger<HL7MessageController> a_mLogger)
        {
            m_config = a_mConfig;
            m_ohDataRepo = a_mOHDataRepo;
            m_logger = a_mLogger;
        }
        /// <summary>
        /// Get the Paginated Status Detail List
        /// </summary>
        /// <param name="options">Option Model having Take,Skip,Sort(asc,desc),FieldToSort,SearchItem</param>
        /// <returns>DataSourceResult model</returns>
        [HttpPost]
        [Route("getListOfProcessedData")]
        public DataSourceResult GetListOfProcessedData(DataSourceRequest options)
        {
            DataSourceResult result = new DataSourceResult();
            try
            {
                string sField = "";
                string sDir = "";
                string sSearchedItem = "";
                if (options.Sort != null)
                {
                    sField = options.Sort.Select(x => x.Field).FirstOrDefault();
                    sDir = options.Sort.Select(x => x.Dir).FirstOrDefault();
                }
                if (options.Filter != null)
                {
                    sSearchedItem = options.Filter.Filters.Select(x => x.Value.ToString()).FirstOrDefault();
                }
                result.Data = m_ohDataRepo.GetStatusDetail(options.Take, options.Skip, sDir, sField, sSearchedItem);
                result.Total = m_ohDataRepo.TotalItemsInTable();
            }
            catch (Exception ex)
            {
                m_logger.LogError($"{ex.Message}=>{ex.StackTrace}");
            }
            return result;
        }

        /// <summary>
        /// Generate HL7Message of SampleId
        /// </summary>
        /// <param name="a_sSampleId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GenerateHL7MessageOfSampleId")]
        public ResponseMsgType GenerateHL7MessageOfSampleId(string a_sSampleId,string a_sId)
        {
            try
            {
                m_plugins = CommonMethod.ReadAssemblies(m_config);

                foreach (var plugin in m_plugins)
                {
                    string sBlobContainer = m_config.Value.BlobContainer;
                    string sMessageFilePath = m_config.Value.MsgFilesPath;
                    string sBlobSasUrl = m_config.Value.BlobSasUrl;

                    if (!string.IsNullOrEmpty(a_sSampleId))
                    {
                        //Get all information from different table to generate HL7Message on the basis of SampleId
                        var oMessageData = m_ohDataRepo.GetMessageData(a_sSampleId);
                        try
                        {
                            if (oMessageData != null)
                            {
                                m_logger.LogInformation($"Generating HL7Message...");
                                //Update status to Inprogress
                                m_ohDataRepo.UpdateStatus(a_sSampleId, a_sId, (char)StausType.Retry);
                                //Generate the HL7 Message and store data into file and also in Blob storage
                                string sGeneratedFile = plugin.GenerateH7Message(oMessageData, sMessageFilePath);
                                if (!string.IsNullOrEmpty(sGeneratedFile))
                                {
                                    m_logger.LogInformation($"File successfully generated");
                                    //Update status to success
                                    m_ohDataRepo.UpdateStatus(a_sSampleId,a_sId, (char)StausType.Success);

                                    m_logger.LogInformation($"Storing file into blob storage");
                                    //Upload file to azure blob storage
                                    bool bIsUploaded = plugin.UploadMessageToBlobStorage(sMessageFilePath, sGeneratedFile,sBlobSasUrl,sBlobContainer, out string sErrorMsg);
                                    if (bIsUploaded)
                                    {
                                        m_logger.LogInformation($"File sucessfully uploaded");
                                        m_ohDataRepo.UpdateFileSucessfullyUploaded(a_sSampleId, a_sId);

                                        //update SMS Q

                                    }
                                    else
                                    {
                                        m_logger.LogError($"Unable to upload file into blob storage: {sErrorMsg}");
                                        return ResponseMsgType.PartialComplete;
                                    }
                                    try
                                    {
                                        string sFilePath = $"{sMessageFilePath}{sGeneratedFile}";
                                        if (System.IO.File.Exists(sFilePath))
                                        {
                                            System.IO.File.Delete(sFilePath);
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
                        catch (Exception ex)
                        {
                            m_ohDataRepo.CaptureExceptionAndUpdateStatus(a_sSampleId,a_sId, (char)StausType.Failed, ex.Message);
                            m_logger.LogError($"{ExceptionMessage.MSG_GENEARATION_ERROR} because {ex.Message}=>{ex.StackTrace}");
                            return ResponseMsgType.Failed;
                        }
                    }
                    else
                    {
                        throw new OHException(ExceptionMessage.NOT_FOUND_ID);
                    }
                }
            }
            catch (OHException ex)
            {
                m_ohDataRepo.CaptureExceptionAndUpdateStatus(a_sSampleId, a_sId,(char)StausType.Failed, "Internal System Error");
                m_logger.LogError($"{ExceptionMessage.MSG_GENEARATION_ERROR} because {ex.Message}=>{ex.StackTrace}");
                return ResponseMsgType.Failed;
            }
            return ResponseMsgType.Success;
        }



        [HttpPost]
        [Route("DiscardSampleId")]
        public StatusChangeResponse DiscardSampleId(string a_sSampleId , string a_sId)
        {
            try
            {
                m_plugins = CommonMethod.ReadAssemblies(m_config);

                foreach (var plugin in m_plugins)
                {
                    string sBlobContainer = m_config.Value.BlobContainer;
                    string sMessageFilePath = m_config.Value.MsgFilesPath;
                    string sBlobSasUrl = m_config.Value.BlobSasUrl;

                    if (!string.IsNullOrEmpty(a_sSampleId))
                    {
                        try
                        {
                            m_logger.LogWarning("Discarding Sample for " + a_sSampleId);
                            m_ohDataRepo.DiscardSampleId(a_sSampleId, (char)SMSQueueStatusType.Discarded);
                        }
                        catch (Exception ex)
                        {

                            m_logger.LogError($"{ExceptionMessage.STATUS_NOT_CHANGED} because {ex.Message}=>{ex.StackTrace}");
                            return StatusChangeResponse.Failed;
                        }
                    }
                    else
                    {
                        throw new OHException(ExceptionMessage.NOT_FOUND_ID);
                    }
                }
            }
            catch (OHException ex)
            {
                m_ohDataRepo.CaptureExceptionAndUpdateStatus(a_sSampleId, a_sId,(char)StausType.Failed, "Internal System Error");
                m_logger.LogError($"{ExceptionMessage.MSG_GENEARATION_ERROR} because {ex.Message}=>{ex.StackTrace}");
                return StatusChangeResponse.Failed;
            }
            return StatusChangeResponse.Success;
        }



        /// <summary>
        /// Generate HL7Message of All remaining Sample Ids
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GenerateAllHl7Messages")]
        public ResponseMsgTypeMulti GenerateAllHL7Messages([FromBody] string[][] allFailedIds)
        {
            int errorOccurred = 0;
            for (int i = 0; i < allFailedIds.Length; i++)
            {
                string a_sSampleId = allFailedIds[i][0];
                string a_sId = allFailedIds[i][1];
                try
                {
                    m_plugins = CommonMethod.ReadAssemblies(m_config);

                    foreach (var plugin in m_plugins)
                    {
                        string sBlobContainer = m_config.Value.BlobContainer;
                        string sMessageFilePath = m_config.Value.MsgFilesPath;
                        string sBlobSasUrl = m_config.Value.BlobSasUrl;

                        if (!string.IsNullOrEmpty(a_sSampleId))
                        {
                            //Get all information from different table to generate HL7Message on the basis of SampleId
                            var oMessageData = m_ohDataRepo.GetMessageData(a_sSampleId);
                            try
                            {
                                if (oMessageData != null)
                                {
                                    m_logger.LogInformation($"Generating HL7Message...");
                                    //Update status to Inprogress
                                    m_ohDataRepo.UpdateStatus(a_sSampleId, a_sId, (char)StausType.Retry);
                                    //Generate the HL7 Message and store data into file and also in Blob storage
                                    string sGeneratedFile = plugin.GenerateH7Message(oMessageData, sMessageFilePath);
                                    if (!string.IsNullOrEmpty(sGeneratedFile))
                                    {
                                        m_logger.LogInformation($"File successfully generated");
                                        //Update status to success
                                        m_ohDataRepo.UpdateStatus(a_sSampleId, a_sId, (char)StausType.Success);

                                        m_logger.LogInformation($"Storing file into blob storage");
                                        //Upload file to azure blob storage
                                        bool bIsUploaded = plugin.UploadMessageToBlobStorage(sMessageFilePath, sGeneratedFile,sBlobSasUrl,sBlobContainer, out string sErrorMsg);
                                        if (bIsUploaded)
                                        {
                                            m_logger.LogInformation($"File sucessfully uploaded");
                                            m_ohDataRepo.UpdateFileSucessfullyUploaded(a_sSampleId, a_sId);

                                            //update SMS Q

                                        }
                                        else
                                        {
                                            m_logger.LogError($"Unable to upload file into blob storage: {sErrorMsg}");
                                        }
                                        try
                                        {
                                            string sFilePath = $"{sMessageFilePath}{sGeneratedFile}";
                                            if (System.IO.File.Exists(sFilePath))
                                            {
                                                System.IO.File.Delete(sFilePath);
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
                            catch (Exception ex)
                            {
                                m_ohDataRepo.CaptureExceptionAndUpdateStatus(a_sSampleId,a_sId, (char)StausType.Failed, ex.Message);
                                m_logger.LogError($"{ExceptionMessage.MSG_GENEARATION_ERROR} because {ex.Message}=>{ex.StackTrace}");
                                errorOccurred = 1;
                                continue;
                            }
                        }
                        else
                        {
                            throw new OHException(ExceptionMessage.NOT_FOUND_ID);
                        }
                    }
                }
                catch (OHException ex)
                {
                    m_ohDataRepo.CaptureExceptionAndUpdateStatus(a_sSampleId,a_sId, (char)StausType.Failed, "Internal System Error");
                    m_logger.LogError($"{ExceptionMessage.MSG_GENEARATION_ERROR} because {ex.Message}=>{ex.StackTrace}");
                    errorOccurred = 1;
                    continue;
                }
            }
            return errorOccurred == 0 ? ResponseMsgTypeMulti.Success : ResponseMsgTypeMulti.PartialComplete;
        }
    }
}