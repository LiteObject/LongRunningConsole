namespace LongRunningConsole
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The my service.
    /// </summary>
    public class MyService : IHostedService, IDisposable
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The _timer.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MyService(ILogger<MyService> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// The start async.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{DateTime.Now} Timed Background Service is starting.");
            this.timer = new Timer(this.DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// The stop async.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{DateTime.Now} Timed Background Service is stopping.");
            this.timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.timer?.Dispose();
        }
        
        /// <summary>
        /// The do work.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        private void DoWork(object state)
        {
            Console.WriteLine($"{DateTime.Now} Timed Background Service is working.");
        }
    }
}
