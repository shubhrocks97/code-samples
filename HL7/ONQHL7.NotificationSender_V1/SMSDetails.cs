using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONQHL7.NotificationSender_V1
{
    public class SMSDetails
    {
        public string SampleId { get; set; }
        public string FirstName { get; set; }
        public int HL7Queue_Id { get; set; }
        public DateTime? DOB { get; set; }
        public string ARRLLabNo { get; set; }
        public string ContactNo { get; set; }
        public string ObservationTime { get; set; }
        public string ObservationResult { get; set; }
        public string Status { get; set; }
        public bool IsScheduled { get; set; }
        public DateTime? ScheduledTime { get; set; }
    }
}
