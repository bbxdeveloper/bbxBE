using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IInvoiceRepositoryAsync : IGenericRepositoryAsync<Invoice>
    {
        Task<bool> IsUniqueInvoiceNumberAsync(string InvoiceNumber, long? ID = null);
        Task<bool> SeedDataAsync(int rowCount);
        Task<Invoice> AddInvoiceAsync(Invoice p_invoice, Dictionary<long, InvoiceLine> p_RelDNInvoiceLines);
        Task<Invoice> UpdateInvoiceAsync(Invoice p_invoice, ICollection<SummaryByVatRate> p_delSummaryByVatRates);
        Task<Entity> GetInvoiceAsync(long ID, bool FullData);
        Task<Entity> GetAggregateInvoiceAsync(long ID);
        Task<IEnumerable<Entity>> GetPendigDeliveryNotesSummaryAsync(bool incoming, long warehouseID, string currencyCode);
        Task<IEnumerable<Entity>> GetPendigDeliveryNotesItemsAsync(bool incoming, long warehouseID, long customerID, string currencyCode);
        Task<IEnumerable<Entity>> GetPendigDeliveryNotesAsync(bool incoming, long warehouseID, string currencyCode);
        Task<decimal> GetUnPaidAmountAsyn(long customerID);
        Task<Invoice> GetInvoiceRecordAsync(long ID, bool FullData = true);
        Task<Invoice> GetInvoiceRecordByInvoiceNumberAsync(string invoiceNumner, bool FullData = true);

        Task<Dictionary<long, Invoice>> GetInvoiceRecordsByInvoiceLinesAsync(List<long> LstInvoiceLineID);
        Task<List<Invoice>> GetCorrectionInvoiceRecordsByInvoiceNumber(string invoiceNumber);
        Task<List<Invoice>> GetCorrectionInvoiceRecordsByInvoiceID(long invoiceID);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvoiceAsync(QueryInvoice requestParameters);
        Task<(IList<GetCustomerInvoiceSummary> data, RecordsCount recordsCount)> QueryPagedCustomerInvoiceSummaryAsync(QueryCustomerInvoiceSummary requestParameters);
        Task<IList<GetInvoiceViewModel>> QueryForCSVInvoiceAsync(CSVInvoice requestParameter);

    }
}