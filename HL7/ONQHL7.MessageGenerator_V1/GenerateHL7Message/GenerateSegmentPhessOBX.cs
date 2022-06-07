using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONQHL7.MessageGenerator_V1.GenerateHL7Message
{
	public class GenerateSegmentPhessOBX
	{
		public string CreatePhessOBXSegment(ObservationResult a_mObservationResult)
		{
			string sErrorMsg;
			var delim = HL7Encoding.FieldDelimiter;
			var compDelim = HL7Encoding.ComponentDelimiter;
			StringBuilder msgSb = new StringBuilder("OBX");
			msgSb.Append(delim);
			msgSb.Append(a_mObservationResult.SetId != null ? a_mObservationResult.SetId : "");
			msgSb.Append(delim);
			msgSb.Append(a_mObservationResult.ValueType != null ? a_mObservationResult.ValueType : "");
			msgSb.Append(delim);
			msgSb.Append(a_mObservationResult.ObservationIdentifier != null ? a_mObservationResult.ObservationIdentifier : "");
			msgSb.Append(delim);
			msgSb.Append(a_mObservationResult.ObservationSubId != null ? a_mObservationResult.ObservationSubId : "");
			msgSb.Append(delim);
			msgSb.Append(a_mObservationResult.ObservationValue != null ? a_mObservationResult.ObservationValue : "");
			msgSb.Append(delim);
			//Units
			msgSb.Append(delim);
			//ReferenceRange
			msgSb.Append(delim);
			//AbnormalFlags
			msgSb.Append(delim);
			//Probability
			msgSb.Append(delim);
			//NatureOfAbnormalTest
			msgSb.Append(delim);
			msgSb.Append(a_mObservationResult.ObservationResultStatus != null ? a_mObservationResult.ObservationResultStatus : "");
			msgSb.Append(delim);
			//DateLastObsNormalValues
			msgSb.Append(delim);
			//UserDefinedAccessChecks
			msgSb.Append(delim);
			msgSb.Append(a_mObservationResult.DateAndTimeOfObservation != null ? ConvertTime.ConvertTimetoAEDT(Convert.ToDateTime(a_mObservationResult.DateAndTimeOfObservation)).ToString("yyyyMMddHHmmss") : "");

			bool bIsValid = ValidateOBXSegment(msgSb.ToString(), out sErrorMsg);
			if (bIsValid && string.IsNullOrEmpty(sErrorMsg))
			{
				return msgSb.ToString();
			}
			else
				throw new Exception(sErrorMsg);
		}

		public bool ValidateOBXSegment(string a_sMessage, out string a_sErrorMsg)
		{
			a_sErrorMsg = "";
			if (!string.IsNullOrEmpty(a_sMessage))
			{
				// Check message length
				if (a_sMessage.Length < 20)
				{
					a_sErrorMsg = "Invalid OBX2 Segment: Incomplete Data";
					return false;
				}

				//// Check if message starts with header segment
				//if (!a_sMessage.StartsWith("OBX"))
				//{
				//	a_sErrorMsg = "";
				//	return false;
				//}
			}
			else
			{
				a_sErrorMsg = "Invalid OBX2 Segment: Empty Data";
				return false;
			}
			return true;
		}
	}
}
