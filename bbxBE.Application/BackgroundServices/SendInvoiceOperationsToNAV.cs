using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.BackgroundServices
{
    public sealed class SendInvoiceOperationsToNAV : BackgroundService
    {
        private readonly NAVSettings _NAVSettings;
        private readonly ILogger<SendInvoiceOperationsToNAV> _logger;
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly INAVXChangeRepositoryAsync _NAVXChangeRepository;

        public SendInvoiceOperationsToNAV(IOptions<NAVSettings> NAVSettings, IInvoiceRepositoryAsync invoiceRepository, INAVXChangeRepositoryAsync NAVXChangeRepository,
                IConfiguration configuration,
                ILogger<SendInvoiceOperationsToNAV> logger)

        {
            _NAVSettings = NAVSettings.Value;
            _logger = logger;
            _invoiceRepository = invoiceRepository;
            _NAVXChangeRepository = NAVXChangeRepository;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_NAVSettings.SendInvoicesToNAV)
                return;

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var createdXChanges = await _NAVXChangeRepository.GetXChangeRecordsByStatus(enNAVStatus.CREATED, _NAVSettings.BatchRecordCnt);

                    var bllNavObj = new bllNAV(_NAVSettings, _logger);


                    createdXChanges.Where(w => w.Operation == enNAVOperation.MANAGEINVOICE.ToString()).ToList().ForEach(async item =>
                    {
                        _logger.LogInformation(string.Format(bbxBEConsts.NAV_SENDINFO1, item.InvoiceNumber, item.Operation));
                        item = bllNavObj.ManageInvoiceByXChange(item);
                        _logger.LogInformation(string.Format(bbxBEConsts.NAV_SENDINFO2, item.InvoiceNumber, item.Operation, item.Status));
                        await _NAVXChangeRepository.UpdateNAVXChangeAsync(item);
                    });

                    createdXChanges.Where(w => w.Operation == enNAVOperation.MANAGEANNULMENT.ToString()).ToList().ForEach(async item =>
                    {

                        _logger.LogInformation(string.Format(bbxBEConsts.NAV_SENDINFO1, item.InvoiceNumber, item.Operation));
                        item = bllNavObj.ManageAnnulmentByXChange(item);
                        _logger.LogInformation(string.Format(bbxBEConsts.NAV_SENDINFO2, item.InvoiceNumber, item.Operation, item.Status));
                        await _NAVXChangeRepository.UpdateNAVXChangeAsync(item);
                    });
                    await Task.Delay(TimeSpan.FromMinutes(_NAVSettings.ServiceRunIntervalMin), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, String.Format(bbxBEConsts.NAV_SENDINVOICETONAV_ERR, ex.Message), ex.Message);
            }
        }
    }
}
