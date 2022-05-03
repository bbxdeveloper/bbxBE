//using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IInvoiceRepositoryAsync : IGenericRepositoryAsync<Invoice>
    {
        Task<bool> IsUniqueInvoiceNumberAsync(string InvoiceNumber, long? ID = null);
        Task<bool> SeedDataAsync(int rowCount);
        Task<Invoice> AddInvoiceAsync(Invoice p_invoice);
        Task<Invoice> UpdateInvoiceAsync(Invoice p_invoice, List<InvoiceLine> p_invoiceLines, List<SummaryByVatRate> p_summaryByVatRate, List<AdditionalInvoiceData> p_additionalInvoiceData, List<AdditionalInvoiceLineData> p_additionalInvoiceLineData);
        
        Task<Entity> GetInvoiceAsync(GetInvoice requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvoiceAsync(QueryInvoice requestParameters);
        
    }
}