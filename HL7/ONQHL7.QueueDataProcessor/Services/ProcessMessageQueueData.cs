using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ONQHL7.GlobalDb.Interfaces;
using ONQHL7.GlobalDb.Models;
using ONQHL7.PluginInterfaces;
using ONQHL7.QueueDataProcessor.CommonMethods;
using ONQHL7.QueueDataProcessor.Models;
using System;
using System.Collections.Generic;

namespace ONQHL7.QueueDataProcessor.Services
{
    public class ProcessMessageQueueData : IProcessMessageQueueData
    {
        private readonly IOptions<AppSettings> m_config;
        private readonly IOHDataRepository m_ohDataRepo;
        private readonly ILogger<ProcessMessageQueueData> m_logger;
        private static List<INotificationSender> m_plugins = null;
        public ProcessMessageQueueData(IOptions<AppSettings> a_mConfig, IOHDataRepository a_mOHDataRepo, ILogger<ProcessMessageQueueData> a_mLogger)
        {
            m_config = a_mConfig;
            m_ohDataRepo = a_mOHDataRepo;
            m_logger = a_mLogger;
        }
        public bool SendMessageQueueDataToPlugin()
        {
            try
            {
                AWSSettings awsSettings = new AWSSettings()
                {
                    AwsKeyId = m_config.Value.AwsKeyId,
                    AwsKeySecret = m_config.Value.AwsKeySecret,
                    AwsRegion = m_config.Value.AwsRegion,
                    AwsAppId = m_config.Value.AwsAppId,
                    AwsMessageType = m_config.Value.AwsMessageType,
                    AwsSenderId = m_config.Value.AwsSenderId,
                    AwsChannelType = m_config.Value.AwsChannelType,
                };
               

                m_plugins = CommonMethod.ReadAssembliesNotification(m_config);

                foreach (var plugin in m_plugins)
                {

                    m_ohDataRepo.SetStatusDiscardedSMSQueue();
                    var SampleIds = m_ohDataRepo.GetListFromSMSQueue();

                    foreach (var Sample in SampleIds)
                    {
                        try
                        {
                            var smsItem = plugin.SMSDataMapping(Sample, out string sErrorMsg);
                            var response = plugin.SendSMS(Sample, awsSettings).Result;

                            string SampleId = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(Sample)).SampleID.ToString();

                            if (response == "OK")
                            {
                                m_logger.LogInformation("Message sent");
                                m_ohDataRepo.SetStatusSentSMSQueue(SampleId);

                            }
                            

                        }
                        catch (Exception ex)
                        {
                            if (ex.Data.Contains("InnerMessage"))
                            {
                                m_logger.LogError(ex.Data["InnerMessage"].ToString() + ex.Message);
                            }
                            else
                            {
                                m_logger.LogError(ex.Message);
                            }
                       
                            m_ohDataRepo.setExceptionFailedSMS(JsonConvert.DeserializeObject<SMSQueueModel>(JsonConvert.SerializeObject(Sample)).HL7Queue_Id, ExceptionStatusType.SMSException.ToString());

                        }
                    }
                }
            }
            catch(Exception ex)
            {
                m_logger.LogError(ex.Message);
            }
            return true;
        }
    }
}
