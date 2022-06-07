using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ONQHL7.QueueDataProcessor.Models.KendoUiModels
{
    public class Group : Sort
    {
        [DataMember(Name = "aggregates")]
        public IEnumerable<Aggregator> Aggregates { get; set; }
    }
}
