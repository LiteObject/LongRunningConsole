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
    /// Original Article: https://github.com/stevejgordon/GenericHostExample/blob/master/GenericHostExample/Program.cs
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
                            services.AddHostedService<MyService>();
                        })
                .ConfigureLogging(
                    (hostContext, services) =>
                        {
                            services.AddConsole();
                        })
                .RunConsoleAsync();
            
            watch.Stop();
            Print($"\n\nPress any key to exit. Elapsed: {watch.Elapsed}");
        }

        /// <summary>
        /// The print.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private static void Print(string message)
        {
            Console.WriteLine(message);
        }
    }
}
