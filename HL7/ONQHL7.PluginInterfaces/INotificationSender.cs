using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONQHL7.PluginInterfaces
{
    public interface INotificationSender
    {
        Task<string> SendSMS(object a_oMessageData, object aWSSettings);
        object SMSDataMapping(object sample, out string sErrorMsg);
    }
}
