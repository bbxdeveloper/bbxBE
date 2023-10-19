using AutoMapper;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class InvoiceLineRepositoryAsync : GenericRepositoryAsync<InvoiceLine>, IInvoiceLineRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private IDataShapeHelper<GetAggregateInvoiceDeliveryNoteViewModel> _dataShaperGetAggregateInvoiceDeliveryNoteModel;
        public InvoiceLineRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData
            ) : base(dbContext)
        {
            _dbContext = dbContext;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _dataShaperGetAggregateInvoiceDeliveryNoteModel = new DataShapeHelper<GetAggregateInvoiceDeliveryNoteViewModel>();
        }

        public async Task<List<InvoiceLine>> GetInvoiceLineRecordsByRelDeliveryNoteIDAsync(long InvoiceID, long RelDeliveryNoteInvoiceID)
        {
            List<InvoiceLine> invoiceLines;
            invoiceLines = await _dbContext.InvoiceLine.AsNoTracking()
                .Where(x => x.InvoiceID == InvoiceID && x.RelDeliveryNoteInvoiceID == RelDeliveryNoteInvoiceID)
                .ToListAsync();
            return invoiceLines;
        }

        public async Task<Entity> GetInvoiceLinesByRelDeliveryNoteIDAsync(long InvoiceID, long RelDeliveryNoteInvoiceID)
        {
            var listFieldsModel = _modelHelper.GetModelFields<GetAggregateInvoiceDeliveryNoteViewModel>();

            //      var invoiceLines = await GetInvoiceLineRecordsByRelDeliveryNoteIDAsync(InvoiceID, RelDeliveryNoteInvoiceID);

            List<InvoiceLine> invoiceLines;
            invoiceLines = await _dbContext.InvoiceLine.AsNoTracking()
                .Include(i => i.DeliveryNote).AsNoTracking()
                .Where(x => x.InvoiceID == InvoiceID)
                .ToListAsync();

            var deliveryNotesCount = invoiceLines.GroupBy(g => g.RelDeliveryNoteInvoiceID).Count();

            var deliveryNote = invoiceLines.Where(x => x.RelDeliveryNoteInvoiceID == RelDeliveryNoteInvoiceID)
                .GroupBy(g => new
                {
                    RelDeliveryNoteInvoiceID = g.RelDeliveryNoteInvoiceID,
                    RelDeliveryNoteNumber = g.RelDeliveryNoteNumber,
                    LineDeliveryDate = g.LineDeliveryDate,
                    DeliveryNoteDiscountPercent = g.DeliveryNote.InvoiceDiscountPercent
                }, (k, g) =>
                new GetAggregateInvoiceDeliveryNoteViewModel
                {
                    DeliveryNoteInvoiceID = k.RelDeliveryNoteInvoiceID.Value,
                    DeliveryNoteNumber = k.RelDeliveryNoteNumber,
                    DeliveryNoteDate = k.LineDeliveryDate,
                    DeliveryNoteNetAmount = Math.Round(g.Sum(s => s.LineNetAmount), 1),
                    DeliveryNoteNetAmountHUF = Math.Round(g.Sum(s => s.LineNetAmountHUF), 1),
                    DeliveryNoteDiscountPercent = k.DeliveryNoteDiscountPercent,
                    DeliveryNoteDiscountAmount = Math.Round(g.Sum(s => s.LineNetAmount - s.LineNetDiscountedAmount), 1),
                    DeliveryNoteDiscountAmountHUF = Math.Round(g.Sum(s => s.LineNetAmountHUF - s.LineNetDiscountedAmountHUF), 1),

                    DeliveryNoteDiscountedNetAmount = Math.Round(g.Sum(s => s.LineNetDiscountedAmount), 1),
                    DeliveryNoteDiscountedNetAmountHUF = Math.Round(g.Sum(s => s.LineNetDiscountedAmountHUF), 1),
                    InvoiceLines = _mapper.Map<List<InvoiceLine>, List<GetAggregateInvoiceDeliveryNoteViewModel.InvoiceLine>>(g.ToList()),
                    DeliveryNotesCount = deliveryNotesCount
                }).FirstOrDefault();        //a RelDeliveryNoteInvoiceID-vel csak egy deliveryNote-t kérdezünk ki

            // shape data
            var shapeData = _dataShaperGetAggregateInvoiceDeliveryNoteModel.ShapeData(deliveryNote, String.Join(",", listFieldsModel));

            return shapeData;
        }


        public async Task<Dictionary<long, InvoiceLine>> GetInvoiceLineRecordsAsync(List<long> LstInvoiceLineID)
        {
            Dictionary<long, InvoiceLine> items;

            items = await _dbContext.InvoiceLine.AsNoTracking()
                .Include(i => i.Invoice)
                .Where(x => LstInvoiceLineID.Any(a => a == x.ID) && !x.Invoice.Deleted && !x.Deleted)
                .ToDictionaryAsync(k => k.ID, i => i);
            return items;
        }
        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

    }
}