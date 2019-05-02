using LongRunningConsole.Services;

namespace LongRunningConsole
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The program.
    /// Original Article: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task Main(string[] args)
        {
            var watch = new Stopwatch();
            watch.Start();

            await new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: true);
                        /*config.AddEnvironmentVariables();

                        if (args != null)
                        {
                            config.AddCommandLine(args);
                        }*/
                    })
                .ConfigureServices(
                    (hostContext, services) =>
                        {
                            services.AddOptions();
                            services.AddLogging();
                            services.Configure<AppConfig>(hostContext.Configuration.GetSection("AppConfig"));
                            services.AddHostedService<MyService>();
                            // services.AddHostedService<FileUploadService>();
                            /* Add other services here as needed */
                        })
                .ConfigureLogging(
                    (hostContext, logging) =>
                    {
                        logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                            logging.AddConsole();
                        })
                .RunConsoleAsync();
            
            watch.Stop();
            Console.WriteLine($"\n\nPress any key to exit. Elapsed: {watch.Elapsed}");
        }
    }
}
