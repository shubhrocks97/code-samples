using Microsoft.Extensions.DependencyInjection;
using NCrontab;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ONQHL7.QueueDataProcessor.BackgroundService
{
    public abstract class ScheduledProcessor : ScopedProcessor
	{
		private CrontabSchedule m_schedule;
		private DateTime m_dtNextRun;

		protected abstract string Schedule { get; }

		public ScheduledProcessor(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
		{
			m_schedule = CrontabSchedule.Parse(Schedule);
			m_dtNextRun = m_schedule.GetNextOccurrence(DateTime.Now);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			do
			{
				var now = DateTime.Now;

				if (now > m_dtNextRun)
				{
					await Process();

					m_dtNextRun = m_schedule.GetNextOccurrence(DateTime.Now);
				}

				await Task.Delay(5000, stoppingToken); // 5 seconds delay

			} while (!stoppingToken.IsCancellationRequested);
		}
	}
}
