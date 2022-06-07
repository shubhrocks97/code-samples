using Dapper;
using ONQHL7.GlobalDb.Models;
using System;
using System.Collections.Generic;

namespace ONQHL7.GlobalDb.Interfaces
{
	public interface IOHDataRepository
	{
		public List<object> GetListOfSampleId(int a_nIdLimit);
		public List<object> GetMessageData(string a_nSampleId);
		public void UpdateStatus(string a_nSampleId, string a_sId, char a_cStatus);
		public List<StatusDetail> GetStatusDetail(int a_nTake, int a_nSkip, string a_sDir, string a_sField, string a_sSearchedItem);
		public int TotalItemsInTable();
		public void CaptureExceptionAndUpdateStatus(string a_nSampleId,string a_sId, char a_cStatus, string a_sExceptionMsg);
		public void CaptureExceptionAndUpdateExceptionStatusType(string a_nSampleId,string a_sId, string a_sExceptionStatus);
		public void InsertUpdatedDataToHL7QueueTable(int a_nIdLimit);
		public void UpdateFileSucessfullyUploaded(string a_nSampleId, string a_sId);
		public List<string> getARRLLabNo(string a_nSampleId);
		public void UpdateSMSQueue(int a_nHL7Id,string a_nGivenName,string DOB, string a_nARRLLabNo, string a_nSampleId, string a_nContactNo, string a_nObservationTime, string a_nObservationResult,int ScheduleMessageMinutes);
		public void SetStatusDiscardedSMSQueue();
		public void SetStatusSentSMSQueue(string SampleId);
		public void setExceptionFailedSMS(int HL7Queue_Id, string ExceptionStatusType);
		public List<object> GetListFromSMSQueue();
		public void DiscardSampleId(string a_nSampleId, char a_cStatus);
	}
}
