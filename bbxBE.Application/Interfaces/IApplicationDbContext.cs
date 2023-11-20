using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces
{
    public interface IApplicationDbContext : IDbContext
    {
        DbSet<AdditionalInvoiceData> AdditionalInvoiceData { get; set; }
        DbSet<AdditionalInvoiceLineData> AdditionalInvoiceLineData { get; set; }
        DbSet<Counter> Counter { get; set; }
        DbSet<CustDiscount> CustDiscount { get; set; }
        DbSet<Customer> Customer { get; set; }
        DbSet<InvCtrl> InvCtrl { get; set; }
        DbSet<InvCtrlPeriod> InvCtrlPeriod { get; set; }
        DbSet<Invoice> Invoice { get; set; }
        DbSet<InvoiceLine> InvoiceLine { get; set; }
        DbSet<Offer> Offer { get; set; }
        DbSet<OfferLine> OfferLine { get; set; }
        DbSet<Origin> Origin { get; set; }
        DbSet<Product> Product { get; set; }
        DbSet<ProductCode> ProductCode { get; set; }
        DbSet<ProductGroup> ProductGroup { get; set; }
        DbSet<Stock> Stock { get; set; }
        DbSet<StockCard> StockCard { get; set; }
        DbSet<SummaryByVatRate> SummaryByVatRate { get; set; }
        DbSet<Users> Users { get; set; }
        DbSet<VatRate> VatRate { get; set; }
        DbSet<Warehouse> Warehouse { get; set; }
        DbSet<WhsTransfer> WhsTransfer { get; set; }
        DbSet<WhsTransferLine> WhsTransferLine { get; set; }
        DbSet<Zip> Zip { get; set; }
        DbSet<Location> Location { get; set; }
        DbSet<NAVXChange> NAVXChange { get; set; }
        DbSet<NAVXResult> NAVXResult { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}