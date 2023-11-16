using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Enums;
using bbxBE.Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.BackgroundServices
{
    public sealed class NAVBackgroundService : BackgroundService
    {
        private readonly NAVSettings _NAVSettings;
        private readonly ILogger<NAVBackgroundService> _logger;
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly INAVXChangeRepositoryAsync _NAVXChangeRepository;

        public NAVBackgroundService(IOptions<NAVSettings> NAVSettings, IInvoiceRepositoryAsync invoiceRepository, INAVXChangeRepositoryAsync NAVXChangeRepository,
                IConfiguration configuration,
                ILogger<NAVBackgroundService> logger)

        {
            _NAVSettings = NAVSettings.Value;
            _logger = logger;
            _invoiceRepository = invoiceRepository;
            _NAVXChangeRepository = NAVXChangeRepository;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var createdXChanges = await _NAVXChangeRepository.GetXChangeRecordsByStatus(enNAVStatus.CREATED, _NAVSettings.BatchRecordCnt);
                    _logger.LogWarning("{Jokex}");

                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }
        }
    }
}
