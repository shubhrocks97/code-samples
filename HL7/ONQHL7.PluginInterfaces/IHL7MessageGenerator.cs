using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ONQHL7.PluginInterfaces
{
	public interface IHL7MessageGenerator
	{
		string GenerateH7Message(List<object> a_oMessageData, string a_sMessageFilePath);
		bool UploadMessageToBlobStorage(string a_sFilePath, string a_sBlobName, string sBlobSasUrl, string sBlobContainer, out string a_sErrorMsg);
    }
}