using Microsoft.Extensions.DependencyInjection;
using ONQHL7.QueueDataProcessor.BackgroundService;
using ONQHL7.QueueDataProcessor.Services;
using System;
using System.Threading.Tasks;

namespace ONQHL7.QueueDataProcessor.ScheduledTask
{
    public class ScheduledProcessQueueData : ScheduledProcessor
	{
		public ScheduledProcessQueueData(IServiceScopeFactory a_mServiceScopeFactory) : base(a_mServiceScopeFactory) { }
		protected override string Schedule => "*/5 * * * *"; // every 5 min 
		//protected override string Schedule => "0 0 * * *"; // every day at midnight (mm hh d m dw)

		public override Task ProcessInScope(IServiceProvider a_mScopeServiceProvider)
		{
            IProcessQueueData m_processQueueData = a_mScopeServiceProvider.GetRequiredService<IProcessQueueData>();
            IProcessMessageQueueData m_processMessageQueueData = a_mScopeServiceProvider.GetRequiredService<IProcessMessageQueueData>();
            m_processQueueData.SendQueueDataToPlugin();
            m_processMessageQueueData.SendMessageQueueDataToPlugin();
            return Task.CompletedTask;
		}
	} 
}
