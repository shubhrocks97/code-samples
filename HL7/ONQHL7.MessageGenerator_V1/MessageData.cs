using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONQHL7.MessageGenerator_V1
{
	public class MessageData
	{
		public MessageHeader MessageHeader { get; set; }
		public PatientIdentification PatientIdentification { get; set; }
		public PatientVisit PatientVisit { get; set; }
		public CommonOrder CommonOrder { get; set; }
		public ObservationRequest ObservationRequest { get; set; }
		public ObservationResult ObservationResult { get; set; }
		public ObservationResult ObservationResultPhess { get; set; }
	}

	public class MessageHeader
	{
		public string SendingApplication { get; set; }
		public string SendingFacility { get; set; }
		public string ReceivingApplication { get; set; }
		public string ReceivingFacility { get; set; }
		public string DateAndTimeOfMsg { get; set; }
		public string MsgType { get; set; }
		public string EventType { get; set; }
		public string MsgControlId { get; set; }
		public string ProcessingId { get; set; }
		public string VersionId { get; set; }
	}
	public class PatientIdentification
	{
		public string PatientIdentifierList { get; set; }
		public string FamilyName { get; set; }
		public string GivenName { get; set; }
		public string DateOfBirth { get; set; }
		public string Sex { get; set; }
		public string Street { get; set; }
		public string Suburb { get; set; }
		public string State { get; set; }
		public string PostalCode { get; set; }
		public string PhoneNumber { get; set; }
	}
	public class PatientVisit
	{
		public string PatientClass { get; set; }
		public string AssignedPatientLocation { get; set; }
	}
	public class CommonOrder
	{
		public string OrderControl { get; set; }
		public string FillerOrderNumber { get; set; }
		public string QuantityOrTiming { get; set; }
		public string OrderingProviderIdNo { get; set; }
		public string OrderingProviderGivenName { get; set; }
		public string OrderingProviderFamilyName { get; set; }
		public string OrderingFacilityName { get; set; }
		public string OrderingFacilityIdNo { get; set; }
		public string OrderingFacilityAddressStreet { get; set; }
		public string OrderingFacilityAddressCity { get; set; }
		public string OrderingFacilityAddressState { get; set; }
		public string OrderingFacilityAddressPostalCode { get; set; }
		public string OrderingFacilityPhoneNumber { get; set; }
	}
	public class ObservationRequest
	{
		public string FillerOrderNumber { get; set; }
		public string PlacerOrderNumber { get; set; }
		public string UniversalServiceId { get; set; }
		public string ObservationDateAndTime { get; set; }
		public string SpecimenReceivedDateAndTime { get; set; }
		public string SpecimenSource { get; set; }
		public string OrderingProviderIdNo { get; set; }
		public string OrderingProviderFamilyName { get; set; }
		public string OrderingProviderGivenName { get; set; }
		public string ResultsRptOrStatusChngDateAndTime { get; set; }
		public string QuantityOrTiming { get; set; }
	}
	public class ObservationResult
	{
		public string SetId { get; set; }
		public string ValueType { get; set; }
		public string ObservationIdentifier { get; set; }
		public string ObservationSubId { get; set; }
		public string ObservationValue { get; set; }
		public string ObservationResultStatus { get; set; }
		public string DateAndTimeOfObservation { get; set; }
	}
}
