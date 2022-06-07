using Amazon;
using Amazon.Pinpoint;
using Amazon.Pinpoint.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ONQHL7.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using NodaTime;

namespace ONQHL7.NotificationSender_V1
{
    public class NotificationSender : INotificationSender
    {

        private readonly string COUNTRY_CODE_NUMBER = "61";
        private readonly string COUNTRY_CODE_PREFIX = "+";
        private readonly string COUNTRY_CODE_NAME = "AU";
        
        public async Task<string> SendSMS(object Sample, object awsSettings)
        {


            var details = SMSDataMapping( Sample, out string sErrorMsg);


            string date = ConvertTimetoAEDT(Convert.ToDateTime(((SMSDetails)details).ObservationTime)).ToString("dd/MM/yyyy");

            var resultString = ((SMSDetails)details).ObservationResult;

            var contact = (Regex.Replace(((SMSDetails)details).ContactNo, @"\s+", "")).Substring(1);

            var firstName = ((SMSDetails)details).FirstName;

            var DOB = ConvertTimetoAEDT(Convert.ToDateTime(((SMSDetails)details).DOB)).ToString("dd/MM/yyyy");

            var ARRLLabNo = ((SMSDetails)details).ARRLLabNo;

            var m_awsKeyId = ((dynamic)awsSettings).AwsKeyId;
            var m_awsKeySecret = ((dynamic)awsSettings).AwsKeySecret;
            var m_region = ((dynamic)awsSettings).AwsRegion;
            var m_appId = ((dynamic)awsSettings).AwsAppId;
            var m_messageType = ((dynamic)awsSettings).AwsMessageType;
            string m_senderId = ((dynamic)awsSettings).AwsSenderId;
            string m_channelType = ((dynamic)awsSettings).AwsChannelType;


            var result = resultString =="Not Detected"? false : true;
            var m_destinationNumber = COUNTRY_CODE_PREFIX + COUNTRY_CODE_NUMBER + contact;
            string readTemplate = result ? File.ReadAllText(@"wwwroot/MessageTemplate/COVID_message_positive.txt"/*@"../ONQHL7.MessageGenerator_V1/MessageTemplate/COVID_message_positive.txt"*/)
                                            : File.ReadAllText(@"wwwroot/MessageTemplate/COVID_message_negative.txt"/*@"../ONQHL7.MessageGenerator_V1/MessageTemplate/COVID_message_negative.txt"*/);
            string m_message = readTemplate.Replace("DD/MM/YYYY", date);
            m_message = m_message.Replace("PATIENTFIRSTNAME", firstName);
            m_message = m_message.Replace("DOB", DOB);
            m_message = m_message.Replace("ARRL Lab #:", "ARRL Lab #: " + ARRLLabNo);


            var awsCredentials = new BasicAWSCredentials(m_awsKeyId, m_awsKeySecret);
            using (AmazonPinpointClient client = new AmazonPinpointClient(awsCredentials, RegionEndpoint.GetBySystemName(m_region)))
            {


                PhoneNumberValidateRequest validateRequest = new PhoneNumberValidateRequest
                {
                    NumberValidateRequest = new NumberValidateRequest
                    {
                        IsoCountryCode = COUNTRY_CODE_NAME,
                        PhoneNumber = $"{COUNTRY_CODE_NUMBER }{contact}"
                    }
                };

                

                SendMessagesRequest sendRequest = new SendMessagesRequest
                {
                    ApplicationId = m_appId,
                    MessageRequest = new MessageRequest
                    {
                        Addresses = new Dictionary<string, AddressConfiguration>
                        {
                            {
                                $"{COUNTRY_CODE_NUMBER}{contact}",
                                new AddressConfiguration
                                {
                                    ChannelType = m_channelType
                                }
                            }
                        },
                        MessageConfiguration = new DirectMessageConfiguration
                        {
                            SMSMessage = new SMSMessage
                            {
                                Body = m_message,
                                MessageType = m_messageType,
                                SenderId = m_senderId
                            }
                        }
                    }
                };
                try
                {

                    PhoneNumberValidateResponse validateResponse = await client.PhoneNumberValidateAsync(validateRequest);
                    if (validateResponse.NumberValidateResponse.PhoneType == "INVALID")
                    {
                        throw new Exception("Invalid number Exception");
                    }
                    /*else
                    {
                       m_destinationNumber = validateResponse.NumberValidateResponse.CleansedPhoneNumberE164;
                    }*/

                    SendMessagesResponse res = await client.SendMessagesAsync(sendRequest);

                    var response = JsonConvert.SerializeObject(res.MessageResponse.Result);
                    if (res.MessageResponse.Result[$"{COUNTRY_CODE_NUMBER }{ contact}"].StatusCode != 200)
                    {
                        throw new Exception(res.MessageResponse.Result[m_destinationNumber].StatusMessage);
                    }
                    return "OK";
                }
                catch(Exception ex)
                {                   
                    ex.Data.Add("InnerMessage", "Message could not be sent because ");
                    throw;
                }
            }
        }

        public object SMSDataMapping(object Sample, out string sErrorMsg)
        {
            SMSDetails data = new SMSDetails();
            sErrorMsg = string.Empty;
            try
            {
                
                string date = JsonConvert.DeserializeObject<SMSDetails>(JsonConvert.SerializeObject(Sample)).ObservationTime;
                data.ObservationTime = date;

                var resultString = JsonConvert.DeserializeObject<SMSDetails>(JsonConvert.SerializeObject(Sample)).ObservationResult;
                data.ObservationResult = resultString;

                var contact = JsonConvert.DeserializeObject<SMSDetails>(JsonConvert.SerializeObject(Sample)).ContactNo;
                data.ContactNo = contact;

                var firstName = JsonConvert.DeserializeObject<SMSDetails>(JsonConvert.SerializeObject(Sample)).FirstName;
                data.FirstName = firstName;

                var DOB = JsonConvert.DeserializeObject<SMSDetails>(JsonConvert.SerializeObject(Sample)).DOB;
                data.DOB = DOB;

                var ARRLLabNo = JsonConvert.DeserializeObject<SMSDetails>(JsonConvert.SerializeObject(Sample)).ARRLLabNo;
                data.ARRLLabNo = ARRLLabNo;
            }
            catch(Exception ex)
            {
                sErrorMsg = $"Unable to map the data to message model: {ex.Message}";
                return null;
            }
            return data;
        }

        public DateTimeOffset ConvertTimetoAEDT(DateTime date)
        {
            var countryTimeZone = "Australia/Sydney";
            if (!string.IsNullOrEmpty(countryTimeZone))
            {
                var zone = DateTimeZoneProviders.Tzdb[countryTimeZone];
                var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(date, DateTimeKind.Utc));
                var localDateTime = instant.InZone(zone).ToDateTimeOffset();
                return localDateTime;
            }
            else
            {
                return new DateTimeOffset(DateTime.SpecifyKind(date, DateTimeKind.Utc)).ToOffset(TimeSpan.Parse(countryTimeZone.Replace("+", "")));
            }
        }
        
    }
    
}



