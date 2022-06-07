using System;
using System.Collections.Generic;

namespace ONQHL7.GlobalDb.Models
{
	public class StatusDetail
	{
		public string HL7Id { get; set; }
        public string ArrlLabNo { get; set; }
        public string SampleId { get; set; }
		public string Status { get; set; }
		public string IsStored { get; set; }
		public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ScheduledTime { get; set; }
		public string NotificationStatus { get; set; }
        public string ExceptionMsg { get; set; }
		public string ExceptionStatus { get; set; }
	}
}
