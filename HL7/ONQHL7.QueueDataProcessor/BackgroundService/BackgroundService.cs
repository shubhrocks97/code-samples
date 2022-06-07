using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ONQHL7.QueueDataProcessor.BackgroundService
{
    /// <summary>
    /// Base class for implementing a long running <see cref="IHostedService"/>.
    /// </summary>
    public abstract class BackgroundService : IHostedService, IDisposable
	{
		private Task m_executingTask;
		private readonly CancellationTokenSource m_stoppingCts = new CancellationTokenSource();
		public virtual Task StartAsync(CancellationToken cancellationToken)
		{
			// Store the task we're executing
			m_executingTask = ExecuteAsync(m_stoppingCts.Token);

			// If the task is completed then return it,
			// this will bubble cancellation and failure to the caller
			if (m_executingTask.IsCompleted)
			{
				return m_executingTask;
			}
			// Otherwise it's running
			return Task.CompletedTask;
		}

		public virtual async Task StopAsync(CancellationToken cancellationToken)
		{
			// Stop called without start
			if (m_executingTask == null)
			{
				return;
			}

			try
			{
				// Signal cancellation to the executing method
				m_stoppingCts.Cancel();
			}
			finally
			{
				// Wait until the task completes or the stop token triggers
				await Task.WhenAny(m_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
			}
		}

		/// <summary>
		/// Execute the Main Task here
		/// </summary>
		/// <param name="stoppingToken"></param>
		/// <returns></returns>
		protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			do
			{
				await Process();

				await Task.Delay(5000, stoppingToken);

			} while (!stoppingToken.IsCancellationRequested);
		}

		protected abstract Task Process();

		public virtual void Dispose()
		{
			m_stoppingCts.Cancel();
		}
	}
}

