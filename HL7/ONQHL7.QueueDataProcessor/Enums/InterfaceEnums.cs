using System.ComponentModel;

namespace ONQHL7.QueueDataProcessor.Enums
{
    public enum InterfaceTypes
    {
        [Description("Plugin for generation of HL7 Messages")]
        MessageGenerator =1,
        [Description("Plugin for sending messages")]
        NotificationCenter = 2
    }
}
