namespace ONQHL7.MessageGenerator_V1
{
    public class AppSettings
    {
		public bool IsTest { get; set; }
		public string PluginFilesPath { get; set; }
		public string PluginFilesFormat { get; set; }
		public string MsgFilesPath { get; set; }
		public int LimitOfSampleId { get; set; }
		public int ScheduleMessageMinutes { get; set; }
		public string BlobSasUrl { get; set; }
		public string BlobContainer { get; set; }
		public string AwsKeyId { get; set; }
		public string AwsKeySecret { get; set; }
		public string AwsRegion { get; set; }
		public string AwsDestinationNumber { get; set; }
		public string AwsMessageType { get; set; }
		public string AwsAppId { get; set; }
		public string AwsSenderId { get; set; }
		public string AwsChannelType { get; set; }
	}
}
