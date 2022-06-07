using System;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using ONQHL7.PluginInterfaces;
using System.Collections.Generic;
using ONQHL7.MessageGenerator_V1;
using ONQHL7.MessageGenerator_V1.GenerateHL7Message;
using Microsoft.Extensions.Options;

namespace ONQHL7MessageGenerator_V1
{
	public class HL7MessageGenerator : IHL7MessageGenerator
	{

        /// <summary>
        /// Get the detail from database and map the data into Message model and pass to generate HL7 Message
        /// </summary>
        /// <param name="a_oMessageData"></param>
        /// <returns>ture if message sucesfully generated</returns>
        /// <exception cref="Exception"></exception>
        public string GenerateH7Message(List<object> a_oMessageData, string a_sMessageFilePath)
		{
			try
			{
				GenerateMessage mGenerateMessage = new GenerateMessage();
				//Map data to MessageModel
				MessageData mMessageData = MessageDataMapping(a_oMessageData, out string sErrorMsg);
				if (mMessageData != null)
				{
					//Generate message file and store into file storage
					return mGenerateMessage.GenerateHL7Message(a_sMessageFilePath, mMessageData);
				}
				else
					throw new Exception(sErrorMsg);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message, ex.InnerException);
			}
		}

		/// <summary>
		/// Map the data into message model
		/// </summary>
		/// <param name="a_oMessageData"></param>
		/// <returns>Mapped message model instance</returns>
		public MessageData MessageDataMapping(List<object> a_oMessageData, out string sErrorMsg)
		{
			MessageData mMessageData = new MessageData();
			sErrorMsg = string.Empty;
			try
			{
				//MessageHeader
				string jsonMH = JsonConvert.SerializeObject(a_oMessageData[0]);
				mMessageData.MessageHeader = JsonConvert.DeserializeObject<MessageHeader>(jsonMH);
				//PatientIdentification
				string jsonPI = JsonConvert.SerializeObject(a_oMessageData[1]);
				mMessageData.PatientIdentification = JsonConvert.DeserializeObject<PatientIdentification>(jsonPI);
				//CommonOrder
				string jsonORC = JsonConvert.SerializeObject(a_oMessageData[2]);
				mMessageData.CommonOrder = JsonConvert.DeserializeObject<CommonOrder>(jsonORC);
				//ObservationRequest
				string jsonOBR = JsonConvert.SerializeObject(a_oMessageData[3]);
				mMessageData.ObservationRequest = JsonConvert.DeserializeObject<ObservationRequest>(jsonOBR);
				//ObservationResult
				string jsonOBX = JsonConvert.SerializeObject(a_oMessageData[4]);
				mMessageData.ObservationResult = JsonConvert.DeserializeObject<ObservationResult>(jsonOBX);
				//ObservationResultPhess
				string jsonPhessOBX = JsonConvert.SerializeObject(a_oMessageData[5]);
				mMessageData.ObservationResultPhess = JsonConvert.DeserializeObject<ObservationResult>(jsonPhessOBX);
			}
			catch (Exception ex)
			{
				sErrorMsg = $"Unable to map the data to message model: {ex.Message}";
				return null;
			}
			return mMessageData;
		}

        /// <summary>
        /// Upload file to azur blob storage
        /// </summary>
        /// <param name="a_sFilePath"></param>
        /// <exception cref="Exception"></exception>
        public bool UploadMessageToBlobStorage(string a_sFilePath, string a_sBlobName, string sBlobSasUrl, string sBlobContainer, out string a_sErrorMsg)
		{
			var m_BlobSasUrl = sBlobSasUrl;
			var m_BlobContainer = sBlobContainer;
			a_sErrorMsg = string.Empty;
			try
			{
				BlobServiceClient mServiceClient = new BlobServiceClient(new Uri(m_BlobSasUrl));
				BlobContainerClient mBlobContainer = mServiceClient.GetBlobContainerClient(m_BlobContainer);
				BlobClient mBlobClient = mBlobContainer.GetBlobClient(a_sBlobName);
				mBlobClient.Upload($"{ a_sFilePath}{a_sBlobName}");
				return true;
			}
			catch (Exception ex)
			{
				a_sErrorMsg = ex.Message;
				return false;
			}
		}

        
    }
}