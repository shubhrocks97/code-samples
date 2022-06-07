using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONQHL7.MessageGenerator_V1.GenerateHL7Message
{
    public class GenerateSegmentPID
    {
        public string CreatePIDSegment(PatientIdentification a_mPatientIdentification)
        {
            string sErrorMsg;
            var delim = HL7Encoding.FieldDelimiter;
            var compDelim = HL7Encoding.ComponentDelimiter;
            a_mPatientIdentification.DateOfBirth = a_mPatientIdentification.DateOfBirth==null?"": ConvertTime.ConvertTimetoAEDT(Convert.ToDateTime(a_mPatientIdentification.DateOfBirth)).ToString("yyyyMMdd");
            StringBuilder msgSb = new StringBuilder("PID");
            msgSb.Append(delim);
            //SetId
            msgSb.Append(delim);
            //PatientId
            msgSb.Append(delim);
            msgSb.Append(a_mPatientIdentification.PatientIdentifierList != null ? a_mPatientIdentification.PatientIdentifierList : "");
            msgSb.Append(delim);
            //AlternatePatientId
            msgSb.Append(delim);
            msgSb.Append(a_mPatientIdentification.FamilyName != null ? a_mPatientIdentification.FamilyName : "");
            msgSb.Append(compDelim);
            msgSb.Append(a_mPatientIdentification.GivenName != null ? a_mPatientIdentification.GivenName : "");
            msgSb.Append(delim);
            //MotherMaidenName
            msgSb.Append(delim);
            msgSb.Append(a_mPatientIdentification.DateOfBirth != null ? a_mPatientIdentification.DateOfBirth : "");
            msgSb.Append(delim);
            msgSb.Append(a_mPatientIdentification.Sex != null ? a_mPatientIdentification.Sex : "");
            msgSb.Append(delim);
            //PatientAlias
            msgSb.Append(delim);
            //Race
            msgSb.Append(delim);
            msgSb.Append(a_mPatientIdentification.Street != null ? a_mPatientIdentification.Street : "");
            msgSb.Append(compDelim);
            //PatientAddressOtherDesignation
            msgSb.Append(compDelim);
            msgSb.Append(a_mPatientIdentification.Suburb != null ? a_mPatientIdentification.Suburb : "");
            msgSb.Append(compDelim);
            msgSb.Append(a_mPatientIdentification.State != null ? a_mPatientIdentification.State : "");
            msgSb.Append(compDelim);
            msgSb.Append(a_mPatientIdentification.PostalCode != null ? a_mPatientIdentification.PostalCode : "");
            msgSb.Append(delim);
            //CountryCode
            msgSb.Append(delim);
            //PhoneNoHome
            msgSb.Append(delim);
            msgSb.Append(a_mPatientIdentification.PhoneNumber != null ? a_mPatientIdentification.PhoneNumber : "");

            bool bIsValid = ValidatePIDSegment(msgSb.ToString(), out sErrorMsg);
            if (bIsValid && string.IsNullOrEmpty(sErrorMsg))
            {
                return msgSb.ToString();
            }
            else
                throw new Exception(sErrorMsg);
        }

        public bool ValidatePIDSegment(string a_sMessage, out string a_sErrorMsg)
        {
            a_sErrorMsg = "";
            if (!string.IsNullOrEmpty(a_sMessage))
            {
                // Check message length
                if (a_sMessage.Length < 20)
                {
                    a_sErrorMsg = "Invalid PID Segment: Incomplete Data";
                    return false;
                }

                //// Check if message starts with header segment
                //if (!a_sMessage.StartsWith("PID"))
                //{
                //	a_sErrorMsg = "";
                //	return false;
                //}
            }
            else
            {
                a_sErrorMsg = "Invalid PID Segment: Empty Data";
                return false;
            }
            return true;
        }

    }
}
