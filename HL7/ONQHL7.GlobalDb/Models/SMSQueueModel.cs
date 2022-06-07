using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONQHL7.GlobalDb.Models
{
    public class SMSQueueModel
    {
        public int HL7Queue_Id { get; set; }
        public string GivenName { get; set; }
        public string ScheduledTime { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string SampleId { get; set; }
        public string PhoneNumber { get; set; }
        public string ObservationDateAndTime { get; set; }
        public string ObservationValue { get; set; }
        public string OrderingFacilityIdNo { get; set; }
    }
}
