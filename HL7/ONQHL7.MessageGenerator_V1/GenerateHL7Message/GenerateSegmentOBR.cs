using System;
using System.Text;

namespace ONQHL7.MessageGenerator_V1.GenerateHL7Message
{
	public class GenerateSegmentOBR
	{
		public string CreateOBRSegment(ObservationRequest a_mObservationRequest)
		{
			string sErrorMsg;
			var delim = HL7Encoding.FieldDelimiter;
			var compDelim = HL7Encoding.ComponentDelimiter;
			StringBuilder msgSb = new StringBuilder("OBR");
			msgSb.Append(delim);
			//SetId
			msgSb.Append(delim);
			msgSb.Append(a_mObservationRequest.PlacerOrderNumber != null ? a_mObservationRequest.PlacerOrderNumber : "");
			msgSb.Append(delim);
			msgSb.Append(a_mObservationRequest.FillerOrderNumber != null ? a_mObservationRequest.FillerOrderNumber : "");
			msgSb.Append(delim);
			msgSb.Append(a_mObservationRequest.UniversalServiceId != null ? a_mObservationRequest.UniversalServiceId : "");
			msgSb.Append(delim);
			//Priority-OBR
			msgSb.Append(delim);
			//RequestedDateTime
			msgSb.Append(delim);
			msgSb.Append(a_mObservationRequest.ObservationDateAndTime != null ? ConvertTime.ConvertTimetoAEDT(Convert.ToDateTime(a_mObservationRequest.ObservationDateAndTime)).ToString("yyyyMMddHHmmss") : "");
			msgSb.Append(delim);
			//ObservationEndDateTime
			msgSb.Append(delim);
			//CollectionVolume
			msgSb.Append(delim);
			//CollectorIdentifier
			msgSb.Append(delim);
			//SpecimenActionCode
			msgSb.Append(delim);
			//DangerCode
			msgSb.Append(delim);
			//RelevantClinicalInfo
			msgSb.Append(delim);
			msgSb.Append(a_mObservationRequest.SpecimenReceivedDateAndTime != null ? ConvertTime.ConvertTimetoAEDT(Convert.ToDateTime(a_mObservationRequest.SpecimenReceivedDateAndTime)).ToString("yyyyMMddHHmmss") : "");
			msgSb.Append(delim);
			msgSb.Append(a_mObservationRequest.SpecimenSource != null ? a_mObservationRequest.SpecimenSource : "");
			msgSb.Append(delim);
			msgSb.Append(a_mObservationRequest.OrderingProviderIdNo != null ? a_mObservationRequest.OrderingProviderIdNo : "");
			msgSb.Append(compDelim);
			msgSb.Append(a_mObservationRequest.OrderingProviderFamilyName != null ? a_mObservationRequest.OrderingProviderFamilyName : "");
			msgSb.Append(compDelim);
			msgSb.Append(a_mObservationRequest.OrderingProviderGivenName != null ? a_mObservationRequest.OrderingProviderGivenName : "");
			msgSb.Append(delim);
			//OrderCallBackPhoneNo
			msgSb.Append(delim);
			//PlacerField1
			msgSb.Append(delim);
			//PlacerField2
			msgSb.Append(delim);
			//FillerField1
			msgSb.Append(delim);
			//FillerField2
			msgSb.Append(delim);
			msgSb.Append(a_mObservationRequest.ResultsRptOrStatusChngDateAndTime != null ? ConvertTime.ConvertTimetoAEDT(Convert.ToDateTime(a_mObservationRequest.ResultsRptOrStatusChngDateAndTime)).ToString("yyyyMMddHHmmss") : "");
			msgSb.Append(delim);
			//ChargeToPractice
			msgSb.Append(delim);
			//DiagnosticServSectId
			msgSb.Append(delim);
			//ResultStatus
			msgSb.Append(delim);
			//ParentResult
			msgSb.Append(delim);
			//Quantity
			msgSb.Append(compDelim);
			//Interval
			msgSb.Append(compDelim);
			//Duration
			msgSb.Append(compDelim);
			msgSb.Append(a_mObservationRequest.QuantityOrTiming != null ? ConvertTime.ConvertTimetoAEDT(Convert.ToDateTime(a_mObservationRequest.QuantityOrTiming)).ToString("yyyyMMddHHmmss") : "");

			bool bIsValid = ValidateOBRSegment(msgSb.ToString(), out sErrorMsg);
			if (bIsValid && string.IsNullOrEmpty(sErrorMsg))
			{
				return msgSb.ToString();
			}
			else
				throw new Exception(sErrorMsg);
		}
		public bool ValidateOBRSegment(string a_sMessage, out string a_sErrorMsg)
		{
			a_sErrorMsg = "";
			if (!string.IsNullOrEmpty(a_sMessage))
			{
				// Check message length
				if (a_sMessage.Length < 20)
				{
					a_sErrorMsg = "Invalid OBR Segment: Incomplete Data";
					return false;
				}

				//// Check if message starts with header segment
				//if (!a_sMessage.StartsWith("OBR"))
				//{
				//	a_sErrorMsg = "";
				//	return false;
				//}
			}
			else
			{
				a_sErrorMsg = "Invalid OBR Segment: Empty Data";
				return false;
			}
			return true;
		}
	}
}
