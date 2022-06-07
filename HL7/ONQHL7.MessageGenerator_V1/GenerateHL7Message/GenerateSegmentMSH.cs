using System;
using System.Text;

namespace ONQHL7.MessageGenerator_V1.GenerateHL7Message
{
	public class GenerateSegmentMSH
	{
		private readonly string TIMEOFMSG = DateTime.UtcNow.ToString("HHmmfff");
		private readonly string DATETIMEOFMSG = ConvertTime.ConvertTimetoAEDT(DateTime.UtcNow).ToString("yyyyMMddHHmm");
		public string CreateMSGSegment(MessageHeader a_mMessageHeader)
		{
			string sErrorMsg;
			var delim = HL7Encoding.FieldDelimiter;
			var compDelim = HL7Encoding.ComponentDelimiter;
			a_mMessageHeader.MsgControlId = a_mMessageHeader.MsgControlId + TIMEOFMSG;
			a_mMessageHeader.DateAndTimeOfMsg = DATETIMEOFMSG;
			StringBuilder msgSb = new StringBuilder("MSH");
			msgSb.Append(HL7Encoding.AllDelimiters);
			msgSb.Append(delim);
			msgSb.Append(a_mMessageHeader.SendingApplication != null ? a_mMessageHeader.SendingApplication : "");
			msgSb.Append(delim);
			msgSb.Append(a_mMessageHeader.SendingFacility != null ? a_mMessageHeader.SendingFacility : "");
			msgSb.Append(delim);
			msgSb.Append(a_mMessageHeader.ReceivingApplication != null ? a_mMessageHeader.ReceivingApplication : "");
			msgSb.Append(delim);
			msgSb.Append(a_mMessageHeader.ReceivingFacility != null ? a_mMessageHeader.ReceivingFacility : "");
			msgSb.Append(delim);
			msgSb.Append(a_mMessageHeader.DateAndTimeOfMsg != null ? a_mMessageHeader.DateAndTimeOfMsg : "");
			msgSb.Append(delim);
			//Security
			msgSb.Append(delim);
			msgSb.Append(a_mMessageHeader.MsgType != null ? a_mMessageHeader.MsgType : "");
			msgSb.Append(compDelim);
			msgSb.Append(a_mMessageHeader.EventType != null ? a_mMessageHeader.EventType : "");
			msgSb.Append(delim);
			msgSb.Append(a_mMessageHeader.MsgControlId != null ? a_mMessageHeader.MsgControlId : "");
			msgSb.Append(delim);
			msgSb.Append(a_mMessageHeader.ProcessingId != null ? a_mMessageHeader.ProcessingId : "");
			msgSb.Append(delim);
			msgSb.Append(a_mMessageHeader.VersionId != null ? a_mMessageHeader.VersionId : "");

			bool bIsValid = ValidateMSHSegment(msgSb.ToString(), out sErrorMsg);
			if (bIsValid && string.IsNullOrEmpty(sErrorMsg))
			{
				return msgSb.ToString();
			}
			else
				throw new Exception(sErrorMsg);
		}

		public bool ValidateMSHSegment(string a_sMessage, out string a_sErrorMsg)
		{
			a_sErrorMsg = "";
			if (!string.IsNullOrEmpty(a_sMessage))
			{
				// Check message length
				if (a_sMessage.Length < 20)
				{
					a_sErrorMsg = "Invalid MSH Segment: Incomplete Data";
					return false;
				}

				//// Check if message starts with header segment
				//if (!a_sMessage.StartsWith("MSH"))
				//{
				//	a_sErrorMsg = "";
				//	return false;
				//}
			}
			else
			{
				a_sErrorMsg = "Invalid MSH Segment: Empty Data";
				return false;
			}
			return true;
		}
	}
}