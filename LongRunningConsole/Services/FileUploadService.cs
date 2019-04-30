using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LongRunningConsole.Library;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace LongRunningConsole.Services
{
    public class FileUploadService : IHostedService, IDisposable
    {

        /// <summary>
        /// The azure blob storage connection string.
        /// </summary>
        private static string storageConnectionString = "";

        /// <summary>
        /// The blob container name.
        /// </summary>
        private static string blobContainerName = "";

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The _timer.
        /// </summary>
        private Timer _timer;
        
        /// <inheritdoc />
        public FileUploadService(ILogger<FileUploadService> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._logger.LogTrace("FileUploadService object has been instantiated.");
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTime.Now}] {nameof(FileUploadService)} background Service is starting.");
            this._timer = new Timer(this.LoadFile, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTime.Now}]  {nameof(FileUploadService)} background Service is stopping.");
            this._timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this._timer?.Dispose();
        }

        private void LoadFile(object state)
        {
            Console.WriteLine($"{DateTime.Now} LoadFile(..) method has been invoked.");
            var fileName = "current.xlsx";

            var stream = CloudHelper.ReadBlobAsync(storageConnectionString, blobContainerName, fileName).Result;
            stream.Seek(0, SeekOrigin.Begin);

            using (var excelPackage = new ExcelPackage(stream))
            {
                var excelWorkSheets = excelPackage.Workbook.Worksheets;

                foreach (var worksheet in excelWorkSheets)
                {
                    Console.WriteLine($"\t - Processing worksheet: {worksheet.Name}");
                    ProcessWorksheet(worksheet);
                }
            }
        }

        private void ProcessWorksheet(ExcelWorksheet worksheet)
        {
            if (worksheet != null)
            {

                var x = worksheet.Cells;
                
                for (var curRow = 1; curRow <= worksheet.Dimension.Rows; curRow++)
                {
                    for (var curColumn = 1; curColumn <= worksheet.Dimension.Columns; curColumn++)
                    {
                        try
                        {
                            var curCellStr = worksheet.Cells[curRow, curColumn].Value?.ToString() ?? string.Empty;

                            if (string.IsNullOrWhiteSpace(curCellStr))
                            {
                                this._logger.LogInformation($"curCellStr (R:{curRow}, C:{curColumn}) is null or empty");
                                continue;
                            }

                            this._logger.LogInformation($"Processing cell (R:{curRow}, C:{curColumn}): {curCellStr}");
                        }
                        catch (Exception e)
                        {
                            this._logger.LogError(e, e.Message);
                            continue;
                        }
                    }
                }
            }
        }
    }
}
