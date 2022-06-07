using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ONQHL7.QueueDataProcessor.BackgroundService
{
    public abstract class ScopedProcessor : BackgroundService
	{
		private IServiceScopeFactory m_serviceScopeFactory;

		public ScopedProcessor(IServiceScopeFactory a_mServiceScopeFactory) : base()
		{
			m_serviceScopeFactory = a_mServiceScopeFactory;
		}
		protected override async Task Process()
		{
			using (var scope = m_serviceScopeFactory.CreateScope())
			{
				await ProcessInScope(scope.ServiceProvider);
			}
		}

		public abstract Task ProcessInScope(IServiceProvider scopeServiceProvider);
	}
}
