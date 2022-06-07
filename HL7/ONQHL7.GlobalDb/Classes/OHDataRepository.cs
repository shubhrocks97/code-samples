using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ONQHL7.GlobalDb.Interfaces;
using ONQHL7.GlobalDb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ONQHL7.GlobalDb.Classes
{
	public class OHDataRepository : IOHDataRepository
	{

		private readonly DapperContext m_context;
		private readonly ILogger<OHDataRepository> m_logger;
		public OHDataRepository(DapperContext a_mContext, ILogger<OHDataRepository> a_mLogger)
		{
			m_context = a_mContext;
			m_logger = a_mLogger;
		}

		public int TotalItemsInTable()
		{
			try
			{
				using (var connection = m_context.CreateConnection())
				{
					string sSqlQuery = $"Select Count(*) as Total from HL7Queue where IsActive=1";
					return OHDataRepositoryWrapper.ExecuteCountQuery(connection, sSqlQuery);
				}
			}
			catch (Exception ex) 
			{
				m_logger.LogError($"Unable to fetch count=> {ex.Message}");
				return 0; 
			}
		}

		public List<StatusDetail> GetStatusDetail(int a_nTake, int a_nSkip, string a_sDir, string a_sField, string a_sSearchedItem)
		{
			try
			{
				using (var connection = m_context.CreateConnection())
				{
					string sSqlQuery = "";
					if (!string.IsNullOrEmpty(a_sSearchedItem) && string.IsNullOrEmpty(a_sDir) && string.IsNullOrEmpty(a_sField))
					{
						sSqlQuery = $"Select h.Id as HL7Id,h.SampleId,sam.Text1 as ArrlLabNo, h.Status,h.IsStored,h.CreatedDate,h.ModifiedDate,h.ExceptionMsg,h.ExceptionStatus,s.ScheduledTime, s.Status as NotificationStatus from HL7Queue h left join SMSQueue s on h.Id=s.HL7Queue_Id left join SAMple sam on h.sampleId = sam.Sample_id  where IsActive=1 and h.SampleId LIKE '%{a_sSearchedItem}%' or h.Status  LIKE '%{a_sSearchedItem}%' or IsStored  LIKE '%{a_sSearchedItem}%' order by h.ModifiedDate OFFSET {a_nSkip} ROWS FETCH NEXT {a_nTake} ROWS ONLY";
					}
					else if (!string.IsNullOrEmpty(a_sDir) && !string.IsNullOrEmpty(a_sField) && string.IsNullOrEmpty(a_sSearchedItem))
					{
						sSqlQuery = $"Select h.Id as HL7Id,h.SampleId,sam.Text1 as ArrlLabNo,h.Status,h.IsStored,h.CreatedDate,h.ModifiedDate,h.ExceptionMsg,h.ExceptionStatus,s.ScheduledTime, s.Status as NotificationStatus from HL7Queue h left join SMSQueue s on h.Id=s.HL7Queue_Id left join SAMple sam on h.sampleId = sam.Sample_id  where IsActive=1 order by {a_sField} {a_sDir} OFFSET {a_nSkip} ROWS FETCH NEXT {a_nTake} ROWS ONLY";
					}
					else if (!string.IsNullOrEmpty(a_sDir) && !string.IsNullOrEmpty(a_sField) && !string.IsNullOrEmpty(a_sSearchedItem))
					{
						sSqlQuery = $"Select h.Id as HL7Id,h.SampleId,sam.Text1 as ArrlLabNo,h.Status,h.IsStored,h.CreatedDate,h.ModifiedDate,h.ExceptionMsg, h.ExceptionStatus, s.ScheduledTime,s.Status as NotificationStatus from HL7Queue h left join SMSQueue s on h.Id=s.HL7Queue_Id left join SAMple sam on h.sampleId = sam.Sample_id where IsActive=1 and h.SampleId LIKE '%{a_sSearchedItem}%' or h.Status  LIKE '%{a_sSearchedItem}%' or IsStored  LIKE '%{a_sSearchedItem}%' order by {a_sField} {a_sDir} OFFSET {a_nSkip} ROWS FETCH NEXT {a_nTake} ROWS ONLY";
					}
					else
					{
						sSqlQuery = $"Select h.Id as HL7Id,h.SampleId,sam.Text1 as ArrlLabNo,h.Status,h.IsStored,h.CreatedDate,h.ModifiedDate,h.ExceptionMsg,h.ExceptionStatus, s.ScheduledTime, s.Status as NotificationStatus from HL7Queue h left join SMSQueue s on h.Id=s.HL7Queue_Id left join SAMple sam on h.sampleId = sam.Sample_id where IsActive=1 order by h.ModifiedDate OFFSET {a_nSkip} ROWS FETCH NEXT {a_nTake} ROWS ONLY";
					}
					return OHDataRepositoryWrapper.ExecuteQuery<StatusDetail>(connection, sSqlQuery);
				}
			}
			catch (Exception ex) 
			{
				m_logger.LogError($"Unable to fetch details to display=> {ex.Message}");
				return null; 
			}
		}

		public List<object> GetListOfSampleId(int a_nIdLimit)
		{
			try
			{
				using (var connection = m_context.CreateConnection())
				{
					string sSqlQuery = $"Select top {a_nIdLimit} SampleId, Id from HL7Queue where Status='N' and IsActive=1";
					var result = OHDataRepositoryWrapper.ExecuteQuery<object>(connection, sSqlQuery);
					if (result != null && result.Count > 0)
						m_logger.LogInformation($"SampleIds are selected");
					else
						m_logger.LogInformation($"Files are generated for all sample id's");
					return result;
				}
			}
			catch (Exception ex)
			{
				m_logger.LogError($"Unable to fetch sampleIds => {ex.Message}");
				return null;
			}
		}

		public List<object> GetMessageData(string a_nSampleId)
		{
			try
			{
				m_logger.LogInformation($"Fetching details to generate HL7Message");
				using (var connection = m_context.CreateConnection())
				{
					var param = new DynamicParameters();
					param.Add("@SampleId", a_nSampleId);
					var result = OHDataRepositoryWrapper.ExecuteQuery<object, object, object, object, object, object>(connection, param);
					m_logger.LogInformation($"Details successfully fetched");
					return result;
				}
			}
			catch (Exception ex)
			{
				m_logger.LogError($"Unable to featch details => {ex.Message}");
				return null;
			}
		}

		public void UpdateStatus(string a_nSampleId, string a_sId, char a_cStatus)
		{
			try
			{
				using (var connection = m_context.CreateConnection())
				{
					string sSqlQuery = $"Update HL7Queue Set Status='{a_cStatus}', ModifiedDate = '{DateTime.UtcNow.ToString("MM/dd/yyyy h:mm tt")}' where SampleId='{a_nSampleId}' and Id ='{a_sId}' and IsActive=1";
					OHDataRepositoryWrapper.ExecuteUpdateQuery(connection, sSqlQuery);
				}
			}
			catch (Exception ex)
			{
				m_logger.LogError($"Unable to update Status => {ex.Message}");
			}
		}

		public void CaptureExceptionAndUpdateStatus(string a_nSampleId,string a_sId, char a_cStatus, string a_sExceptionMsg)
		{
			try
			{
				using (var connection = m_context.CreateConnection())
				{
					string sSqlQuery = $"Update HL7Queue Set Status='{a_cStatus}', ModifiedDate = '{DateTime.UtcNow.ToString("MM/dd/yyyy h:mm tt")}', ExceptionMsg='{a_sExceptionMsg}' where SampleId='{a_nSampleId}' and Id = '{a_sId}' and IsActive=1";
					OHDataRepositoryWrapper.ExecuteUpdateQuery(connection, sSqlQuery);
				}
			}
			catch (Exception ex)
			{
				m_logger.LogError($"Unable to capture exception update Status => {ex.Message}");
			}
		}

		public void CaptureExceptionAndUpdateExceptionStatusType(string a_nSampleId, string a_sId,string a_sExceptionStatus)
		{
			try
			{
				using (var connection = m_context.CreateConnection())
				{
					string sSqlQuery = $"Update HL7Queue Set ModifiedDate = '{DateTime.UtcNow.ToString("MM/dd/yyyy h:mm tt")}', ExceptionStatus='{a_sExceptionStatus}' where SampleId='{a_nSampleId}' and Id = '{a_sId}' and IsActive=1";
					OHDataRepositoryWrapper.ExecuteUpdateQuery(connection, sSqlQuery);
				}
			}
			catch (Exception ex)
			{
				m_logger.LogError($"Unable to set exception Status Type => {ex.Message}");
			}
		}

		public void InsertUpdatedDataToHL7QueueTable(int a_nIdLimit)
		{
			try
			{
				m_logger.LogInformation($"Inserting the data into HL7Queue table");
				using (var connection = m_context.CreateConnection())
				{
					string sSqlQuery = $"INSERT INTO hl7queue(SampleId, CreatedDate, FileId) (SELECT Top {a_nIdLimit} SCT.SAMPLE_ID, '{DateTime.UtcNow.ToString("MM/dd/yyyy h:mm tt")}' , uniqueId FROM(select DISTINCT SAMPLE_ID, __$operation, sys.fn_cdc_map_lsn_to_time(__$start_lsn) CaptureTime, Sample_Status, concat(Sample_id, format(sys.fn_cdc_map_lsn_to_time(__$start_lsn), 'ddMMyyyyhhmmss')) as uniqueId , row_number() over(partition by SAMPLE_ID order by sys.fn_cdc_map_lsn_to_time(__$start_lsn) desc) as rn from cdc.dbo_Sample_Status_CT where __$operation = 4)SCT LEFT JOIN hl7queue HQ ON HQ.FileId = SCT.uniqueId left join Sample on Sample.SAMPLE_ID = sct.sample_id WHERE SCT.__$operation = 4 and SCT.Sample_Status = 'A' and SCT.rn = 1  and HQ.FileId is Null and SAMPLE_TYPE = 'SARS-CoV2 specific PCR Assay')";
					int nIsStored = OHDataRepositoryWrapper.ExecuteBatch(connection, sSqlQuery);
					if (nIsStored == 0)
						m_logger.LogInformation($"Do not have new data to insert");
					else
						m_logger.LogInformation($"Data is inserted successfully");
				}				
			}
			catch (Exception ex)
			{
				m_logger.LogError($"Unable to insert data => {ex.Message}");
			}
		}

		public void UpdateFileSucessfullyUploaded(string a_nSampleId, string a_sId)
		{
			try
			{
				using (var connection = m_context.CreateConnection())
				{
					string sSqlQuery = $"Update HL7Queue Set IsStored = 1 where SampleId='{a_nSampleId}' and Id = '{a_sId}' and IsActive=1";
					OHDataRepositoryWrapper.ExecuteUpdateQuery(connection, sSqlQuery);
				}
			}
			catch (Exception ex)
			{
				m_logger.LogError($"Unable to update Status => {ex.Message}");
			}
		}
		

		public List<string> getARRLLabNo(string a_nSampleId)
        {
			try
			{
				using (var connection = m_context.CreateConnection())
				{
					string sSqlQuery = $"select Text1 from sample where sample_Id ='{a_nSampleId}'";
					return OHDataRepositoryWrapper.ExecuteQuery<string>(connection, sSqlQuery);
				}
			}
			catch (Exception ex)
			{
				m_logger.LogError($"Unable to get ARRLLabNo. => {ex.Message}");
			}
			return null;
		}



		public void UpdateSMSQueue(int a_nHL7Id, string a_nGivenName ,string DOB,string a_nARRLLabNo , string a_nSampleId, string a_nContactNo,string a_nObservationTime, string a_nObservationResult,int ScheduleMessageMinutes)
        {
            try
            {
                using (var connection = m_context.CreateConnection())
                {
					//set status of duplicate sampleIds as D and unschedule them.
					string sPreSqlQuery = $"update SMSQueue set status = 'D' , isScheduled = 'false' where sampleId = {a_nSampleId} and status!='S'";
					OHDataRepositoryWrapper.ExecuteUpdateQuery(connection, sPreSqlQuery);

                    string sSqlQuery = $"Insert into SMSQueue(SampleId ,FirstName,DOB,ARRLLabNo, ContactNo,ObservationTime, ObservationResult, CreatedDate,ModifiedDate,ScheduledTime, HL7Queue_Id ) values ('{a_nSampleId}', '{a_nGivenName}', CONVERT(datetime, '{DOB}', 120),'{a_nARRLLabNo}', '{a_nContactNo}', CONVERT(datetime, '{a_nObservationTime}', 120), '{a_nObservationResult}' , '{DateTime.UtcNow.ToString("MM/dd/yyyy h:mm tt")}','{DateTime.UtcNow.ToString("MM/dd/yyyy h:mm tt")}', '{DateTime.UtcNow.AddMinutes(ScheduleMessageMinutes).ToString("MM/dd/yyyy h:mm tt")}',{a_nHL7Id})";
                    int dIsStored = OHDataRepositoryWrapper.ExecuteBatch(connection, sSqlQuery);
					if (dIsStored == 0)
						m_logger.LogInformation($"Do not have new data to insert in Message Queue table");
					else
						m_logger.LogInformation($"Data is inserted successfully in Message Queue table");
				}
            }
            catch (Exception ex)
            {
                m_logger.LogError($"Unable to queue for sending SMS=>{ex.Message}");
				throw;
            }
        }

        public List<Object> GetListFromSMSQueue()
        {
			try
			{
				using (var connection = m_context.CreateConnection())
				{

					string sSqlQuery = $"Select SampleID,FirstName,DOB, ARRLLabNo,HL7Queue_Id, ObservationTime ,ObservationResult,ContactNo from SMSQueue where Status='N' and IsScheduled='true' and CONVERT(varchar(20),CONVERT(datetime, ScheduledTime),120)< CONVERT(varchar(20),GETUTCDATE(), 120)";
					var result = OHDataRepositoryWrapper.ExecuteQuery<object>(connection, sSqlQuery);
					//m_logger.LogInformation(Convert.ToDateTime('2022 - 03 - 29 10:50:00.000'));
					var current = DateTime.UtcNow.ToString();
					var low = DateTime.UtcNow.AddMinutes(-3).ToString();
					var high = DateTime.UtcNow.AddMinutes(5).ToString();
					m_logger.LogInformation($"current : {current} , low : {low}, high: { high}");

					m_logger.LogInformation($"current : {Convert.ToDateTime(current)} , low : {Convert.ToDateTime(current)},high:{Convert.ToDateTime(current)}");
					if (result != null && result.Count > 0)

						m_logger.LogInformation($"SampleIds collected from SMSQueue");
					else
						m_logger.LogInformation($"No new data in SMSQueue");
					return result;

				}
			}
			catch (Exception ex)
			{
				m_logger.LogError($"Unable to fetch sampleIds => {ex.Message}");
				throw;
			}
			return null;
		}

		public void SetStatusDiscardedSMSQueue()
        {
			try
			{
				m_logger.LogInformation($"Filtering out unapproved records from SMSQueue");
				using (var connection = m_context.CreateConnection())
				{
					//addd modified date
					string sSqlQuery = $"update SMSQUEUE set IsScheduled='false' , status='D' where Status!='S' and SampleID in (select sample_id from(select DISTINCT SAMPLE_ID, sample_status, __$operation Operation, sys.fn_cdc_map_lsn_to_time(__$start_lsn) TransactionTime, row_number() over(partition by SAMPLE_ID order by sys.fn_cdc_map_lsn_to_time(__$start_lsn) desc) as rn from cdc.dbo_Sample_Status_CT where __$operation = 3) as a where a.rn = 1 and a.SAMPLE_STATUS = 'A')";
					int nIsStored = OHDataRepositoryWrapper.ExecuteBatch(connection, sSqlQuery);
					if (nIsStored == 0)
						m_logger.LogInformation($"No records unapproved in SMSQueue.");
					else
						m_logger.LogInformation($"Unapproved records filtered from SMSQueue");
				}
			}
			catch (Exception ex)
			{
				m_logger.LogError($"Unable to insert data => {ex.Message}");
			}
		}

		public void SetStatusSentSMSQueue(string SampleId)
		{
			try
			{
				m_logger.LogInformation($"Setting Status 'S' of Sent messages");
				using (var connection = m_context.CreateConnection())
				{
					string sSqlQuery = $"Update SMSQueue set Status ='S',ModifiedDate = '{DateTime.UtcNow.ToString("MM/dd/yyyy h:mm tt")}'  where SampleId='{SampleId}' and status='N'";
					int nIsStored = OHDataRepositoryWrapper.ExecuteBatch(connection, sSqlQuery);
					if (nIsStored == 0)
						m_logger.LogInformation($"Status of sent messages could not be set");
					else
						m_logger.LogInformation($"Status of sent messages set to 'S'");
				}
			}
			catch (Exception ex)
			{
				ex.Data.Add("InnerMessage", "Could not set status 'S' because ");
				throw;
			}
		}

		public void setExceptionFailedSMS(int HL7Queue_Id , string ExceptionStatusType)
        {
			try
			{
				m_logger.LogInformation($"Setting Exception Status Type 'SMSException' of Failed messages");
				using (var connection = m_context.CreateConnection())
				{
					string sSqlQuery = $"Update HL7Queue set ExceptionStatus ='{ExceptionStatusType}',ModifiedDate = '{DateTime.UtcNow.ToString("MM/dd/yyyy h:mm tt")}'  where Id='{HL7Queue_Id}'; update SMSQueue set status = 'D' , IsScheduled = 'false' where HL7Queue_Id = {HL7Queue_Id};";
					int nIsStored = OHDataRepositoryWrapper.ExecuteBatch(connection, sSqlQuery);
					if (nIsStored == 0)
						m_logger.LogInformation($"Could not set Exception Status to { ExceptionStatusType}"); 
					else
						m_logger.LogInformation($"Exception Status set to {ExceptionStatusType}");
				}
			}
			catch (Exception ex)
			{
				ex.Data.Add("InnerMessage", $"Could not set Exception status type to {ExceptionStatusType} because ");
				throw;
			}
		}


		public void DiscardSampleId(string a_nSampleId, char a_cStatus)
        {
            try
            {
				m_logger.LogInformation($"Setting Status 'D' of Discarded Samples");
				using (var connection = m_context.CreateConnection())
				{
					string sSqlQuery = $"Update SMSQueue set Status = '{a_cStatus}' where SampleId = '{a_nSampleId}' and status!='S' ";
					int nIsStored = OHDataRepositoryWrapper.ExecuteBatch(connection, sSqlQuery);
					if (nIsStored == 0)
					{
						m_logger.LogInformation($"Could not discard SampleIds");
						throw new Exception("Could not find SampleId");
					}
					else
					{
						m_logger.LogInformation($"Sample successfully discarded");
					}
				}
			}
			catch (Exception ex)
            {
				m_logger.LogError($"Unable to Discard sampleId => {ex.Message}");
				throw;
			}
			
        }
    }


	internal static class OHDataRepositoryWrapper
	{
		internal static List<T> ExecuteQuery<T>(IDbConnection a_mConn, string a_sSqlQuery)
		{
			try
			{
				var result = a_mConn.Query<T>(a_sSqlQuery).ToList();
				return result;
			}
			catch (Exception ex) { throw new Exception(ex.Message, ex.InnerException); }
		}
		internal static int ExecuteBatch(IDbConnection a_mConn, string a_sSqlQuery)
		{
			try
			{
				return a_mConn.Execute(a_sSqlQuery);
			}
			catch (Exception ex) { throw new Exception(ex.Message, ex.InnerException); }
		}

		internal static List<object> ExecuteQuery<T1, T2, T3, T4, T5, T6>(IDbConnection a_mConn, DynamicParameters a_mParam)
		{
			try
			{
				List<object> result = new List<object>();
				var data = a_mConn.QueryMultiple("sp_GetHL7MessageDetail", a_mParam, commandType: CommandType.StoredProcedure);
				result.Add(data.Read<T1>().FirstOrDefault());
				result.Add(data.Read<T2>().FirstOrDefault());
				result.Add(data.Read<T3>().FirstOrDefault());
				result.Add(data.Read<T4>().FirstOrDefault());
				result.Add(data.Read<T5>().FirstOrDefault());
				result.Add(data.Read<T6>().FirstOrDefault());
				return result;
			}
			catch (Exception ex) { throw new Exception(ex.Message, ex.InnerException); }
		}
		internal static void ExecuteUpdateQuery(IDbConnection a_mConn, string a_sSqlQuery)
		{
			try
			{
				var result = a_mConn.Query(a_sSqlQuery);
			}
			catch (Exception ex) { throw new Exception(ex.Message, ex.InnerException); }
		}
		internal static int ExecuteCountQuery(IDbConnection a_mConn, string a_sSqlQuery)
		{
			try
			{
				var result = a_mConn.Query<int>(a_sSqlQuery).FirstOrDefault();
				return result;
			}
			catch (Exception ex) { throw new Exception(ex.Message, ex.InnerException); }
		}
	}
}
