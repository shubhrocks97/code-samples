using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONQHL7.MessageGenerator_V1.GenerateHL7Message
{
	public class GenerateSegmentORC
	{
		public string CreateORCSegment(CommonOrder a_mCommonOrder)
		{
			string sErrorMsg;
			var delim = HL7Encoding.FieldDelimiter;
			var compDelim = HL7Encoding.ComponentDelimiter;
			StringBuilder msgSb = new StringBuilder("ORC");
			msgSb.Append(delim);
			msgSb.Append(a_mCommonOrder.OrderControl != null ? a_mCommonOrder.OrderControl : "");
			msgSb.Append(delim);
			//PlacerOrderNumber
			msgSb.Append(delim);
			msgSb.Append(a_mCommonOrder.FillerOrderNumber != null ? a_mCommonOrder.FillerOrderNumber : "");
			msgSb.Append(delim);
			//PlacerGroupNumber
			msgSb.Append(delim);
			//OrderState
			msgSb.Append(delim);
			//ResponseFlag
			msgSb.Append(delim);
			//Quantity
			msgSb.Append(compDelim);
			//Interval
			msgSb.Append(compDelim);
			//Duration
			msgSb.Append(compDelim);
			msgSb.Append(a_mCommonOrder.QuantityOrTiming != null ? ConvertTime.ConvertTimetoAEDT(Convert.ToDateTime(a_mCommonOrder.QuantityOrTiming)).ToString("yyyyMMddHHmmss") : "");
			msgSb.Append(delim);
			//Parent
			msgSb.Append(delim);
			//DateTimeOfTransaction
			msgSb.Append(delim);
			//EnteredBy
			msgSb.Append(delim);
			//VerifiedBy
			msgSb.Append(delim);
			msgSb.Append(a_mCommonOrder.OrderingProviderIdNo != null ? a_mCommonOrder.OrderingProviderIdNo : "");
			msgSb.Append(compDelim);
			msgSb.Append(a_mCommonOrder.OrderingProviderFamilyName != null ? a_mCommonOrder.OrderingProviderFamilyName : "");
			msgSb.Append(compDelim);
			msgSb.Append(a_mCommonOrder.OrderingProviderGivenName != null ? a_mCommonOrder.OrderingProviderGivenName : "");
			msgSb.Append(delim);
			//EnterersLocation
			msgSb.Append(delim);
			//CallBackPhoneNo
			msgSb.Append(delim);
			//OrderEffectiveDateTime
			msgSb.Append(delim);
			//OrderControlCodeReason
			msgSb.Append(delim);
			//EnteringOrganization
			msgSb.Append(delim);
			//EnteringDevice
			msgSb.Append(delim);
			//ActionBy
			msgSb.Append(delim);
			//AdvanceBeneficiaryNoticeCode
			msgSb.Append(delim);
			msgSb.Append(a_mCommonOrder.OrderingFacilityName != null ? a_mCommonOrder.OrderingFacilityName : "");
			msgSb.Append(compDelim);
			//OrderingFacilityCodeType
			msgSb.Append(compDelim);
			msgSb.Append(a_mCommonOrder.OrderingFacilityIdNo != null ? a_mCommonOrder.OrderingFacilityIdNo : "");
			msgSb.Append(delim);
			msgSb.Append(a_mCommonOrder.OrderingFacilityAddressStreet != null ? a_mCommonOrder.OrderingFacilityAddressStreet : "");
			msgSb.Append(compDelim);
			//OrderingFacilityAddressOtherDesignation
			msgSb.Append(compDelim);
			msgSb.Append(a_mCommonOrder.OrderingFacilityAddressCity != null ? a_mCommonOrder.OrderingFacilityAddressCity : "");
			msgSb.Append(compDelim);
			msgSb.Append(a_mCommonOrder.OrderingFacilityAddressState != null ? a_mCommonOrder.OrderingFacilityAddressState : "");
			msgSb.Append(compDelim);
			msgSb.Append(a_mCommonOrder.OrderingFacilityAddressPostalCode != null ? a_mCommonOrder.OrderingFacilityAddressPostalCode : "");
			msgSb.Append(delim);
			msgSb.Append(a_mCommonOrder.OrderingFacilityPhoneNumber != null ? a_mCommonOrder.OrderingFacilityPhoneNumber : "");

			bool bIsValid = ValidateORCSegment(msgSb.ToString(), out sErrorMsg);
			if (bIsValid && string.IsNullOrEmpty(sErrorMsg))
			{
				return msgSb.ToString();
			}
			else
				throw new Exception(sErrorMsg);
		}

		public bool ValidateORCSegment(string a_sMessage, out string a_sErrorMsg)
		{
			a_sErrorMsg = "";
			if (!string.IsNullOrEmpty(a_sMessage))
			{
				// Check message length
				if (a_sMessage.Length < 20)
				{
					a_sErrorMsg = "Invalid ORC Segment: Incomplete Data";
					return false;
				}

				//// Check if message starts with header segment
				//if (!a_sMessage.StartsWith("ORC"))
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
