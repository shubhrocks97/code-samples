using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ONQHL7.PluginInterfaces;
using ONQHL7.QueueDataProcessor.Exceptions;
using ONQHL7.QueueDataProcessor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using static ONQHL7.QueueDataProcessor.Exceptions.OHException;

namespace ONQHL7.QueueDataProcessor.CommonMethods
{
	public static class CommonMethod
	{
		/// <summary>
		/// Load dll files and read the assmbely
		/// </summary>
		/// <param name="a_mConfig"></param>
		/// <returns>instence list of plugin</returns>
		/// <exception cref="OHException"></exception>
		public static List<IHL7MessageGenerator/*INotificationSender*/> ReadAssemblies(IOptions<AppSettings> a_mConfig/*IConfiguration a_mConfig*/)
		{
			var pluginsLists = new List<IHL7MessageGenerator/*INotificationSender*/>();
			try
			{
				//Read the dll files from the folder
				var files = Directory.GetFiles(a_mConfig.Value.PluginFilesPath, a_mConfig.Value.PluginFilesFormat/*a_mConfig.GetValue<string>("AppSettings:PluginFilesPath"),a_mConfig.GetValue<string>("AppSettings:PluginFilesFormat")*/);

				if (files == null)
					throw new OHException(ExceptionMessage.FILE_NOT_FOUND);

				//Read the assembly from files
				foreach (var file in files)
				{

					var d = Path.Combine(Directory.GetCurrentDirectory(), file);
					var assembly = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), file));

					//Exteract all the types that implements IPlugin 
					var pluginTypes = assembly.GetTypes().Where(t => typeof(IHL7MessageGenerator/*INotificationSender*/).IsAssignableFrom(t) && !t.IsInterface).ToArray();

					foreach (var pluginType in pluginTypes)
					{
						//Create an instance from the extracted type 
						//var pluginInstance = Activator.CreateInstance(pluginType) as IHL7MessageGenerator;
						var pluginInstance=Activator.CreateInstance(pluginType) as IHL7MessageGenerator/*INotificationSender*/;
						pluginsLists.Add(pluginInstance);
					}
				}
			}
			catch (Exception ex)
			{
				throw new OHException($"{ExceptionMessage.UNABLE_TO_READ_DLL} because : {ex.Message}");
			}
			return pluginsLists;
		}


        public static List<INotificationSender> ReadAssembliesNotification(IOptions<AppSettings> a_mConfig)
        {
            var pluginsLists = new List<INotificationSender>();
            try
            {
                //Read the dll files from the folder
                var files = Directory.GetFiles(a_mConfig.Value.PluginFilesPath, a_mConfig.Value.PluginFilesFormat);

                if (files == null)
                    throw new OHException(ExceptionMessage.FILE_NOT_FOUND);

                //Read the assembly from files
                foreach (var file in files)
                {
					

                    var d = Path.Combine(Directory.GetCurrentDirectory(), file);
                    var assembly = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), file));

                    //Exteract all the types that implements IPlugin 
                    var pluginTypes = assembly.GetTypes().Where(t => typeof(INotificationSender).IsAssignableFrom(t) && !t.IsInterface).ToArray();

                    foreach (var pluginType in pluginTypes)
                    {
						// log dll not found matching

						//Create an instance from the extracted type 
						var pluginInstance = Activator.CreateInstance(pluginType) as INotificationSender;
                        pluginsLists.Add(pluginInstance);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new OHException($"{ExceptionMessage.UNABLE_TO_READ_DLL} because : {ex.Message}");
            }
            return pluginsLists;
        }
    }



	public enum StausType
	{
		New = 'N',
		InProgress = 'P',
		Success = 'S',
		Failed = 'F',
		Retry = 'R'
	}

	public enum SMSQueueStatusType
    {
		Sent='S',
		NotSent ='N',
		Discarded='D'
    }

	public enum ResponseMsgType
	{
		[Description("File successfully generated")]
		Success = 1,
		[Description("File successfully generated but unable to upload file on blob")]
		PartialComplete = 2,
		[Description("Unable to generate file")]
		Failed = 3
	}

	public enum ResponseMsgTypeMulti
    {
		[Description("All Files successfully generated")]
		Success = 1,
		[Description("Some of the files could not be generated")]
		PartialComplete = 2,
	}

	public enum StatusChangeResponse
    {
		[Description("Sample sucessfully discarded")]
		Success = 1,
		[Description("Could not discard Sample")]
		Failed = 2
	}

	public enum ExceptionStatusType
    {
		[Description("Could not generate HL7File")]
		FileException = 'F',
		[Description("Could not upload File to Blob Storage")]
		StorageException ='B',
		[Description("Could not send SMS")]
		SMSException ='S'
    }
}