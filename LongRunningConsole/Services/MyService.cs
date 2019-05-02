using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LongRunningConsole.Services
{
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

        private readonly IOptions<AppConfig> _appConfig;

        private readonly ServiceConfig serviceConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MyService(ILogger<MyService> logger, IOptions<AppConfig> appConfig)
        {
            this.logger = logger?? throw new ArgumentNullException(nameof(logger));
            this._appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));

            serviceConfig = this._appConfig.Value.Services.OfType<ServiceConfig>().FirstOrDefault(s => s.ServiceName.Equals(nameof(MyService)));

            if (serviceConfig is null) {
                throw new ApplicationException($"{nameof(serviceConfig)} cannot be null.");
            }
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
            Console.WriteLine($"[{DateTime.Now}] {nameof(MyService)} background Service is starting.");
            this.timer = new Timer(this.DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(serviceConfig.Frequency));
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
            Console.WriteLine($"[{DateTime.Now}]  {nameof(MyService)} background Service is stopping.");
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
        /// The state object is useful for providing the additional information required for the Timer operation.
        /// </param>
        private void DoWork(object state)
        {
            Console.WriteLine($"{DateTime.Now} {nameof(MyService)} background service is working...");
        }
    }
}
