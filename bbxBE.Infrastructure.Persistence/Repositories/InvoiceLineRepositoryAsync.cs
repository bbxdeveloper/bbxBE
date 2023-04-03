using LinqKit;
using Microsoft.EntityFrameworkCore;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using bbxBE.Infrastructure.Persistence.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System;
using AutoMapper;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using bbxBE.Common.Attributes;
using System.ComponentModel;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using bbxBE.Common.Enums;
using bbxBE.Application.Interfaces.Queries;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class InvoiceLineRepositoryAsync : GenericRepositoryAsync<InvoiceLine>, IInvoiceLineRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private IDataShapeHelper<GetAggregateInvoiceDeliveryNoteViewModel> _dataShaperGetAggregateInvoiceDeliveryNoteModel;
        public InvoiceLineRepositoryAsync(ApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            IDataShapeHelper<GetAggregateInvoiceDeliveryNoteViewModel> dataShaperGetAggregateInvoiceDeliveryNoteModel
            ) : base(dbContext)
        {
            _dbContext = dbContext;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _dataShaperGetAggregateInvoiceDeliveryNoteModel = dataShaperGetAggregateInvoiceDeliveryNoteModel;
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
                .Where(x => x.InvoiceID == InvoiceID)
                .ToListAsync();

            var deliveryNotesCount = invoiceLines.GroupBy( g=>g.RelDeliveryNoteInvoiceID).Count();

            var deliveryNote = invoiceLines.Where(x=>x.RelDeliveryNoteInvoiceID == RelDeliveryNoteInvoiceID)
                .GroupBy(g => new
                {
                    RelDeliveryNoteInvoiceID = g.RelDeliveryNoteInvoiceID,
                    RelDeliveryNoteNumber = g.RelDeliveryNoteNumber,
                    LineDeliveryDate = g.LineDeliveryDate
                }, (k, g) =>
                new GetAggregateInvoiceDeliveryNoteViewModel
                {
                    DeliveryNoteInvoiceID = k.RelDeliveryNoteInvoiceID.Value,
                    DeliveryNoteNumber = k.RelDeliveryNoteNumber,
                    DeliveryNoteDate = k.LineDeliveryDate,
                    DeliveryNoteNetAmount = Math.Round(g.Sum(s => s.LineNetAmount), 1),
                    DeliveryNoteNetAmountHUF = Math.Round(g.Sum(s => s.LineNetAmountHUF), 1),
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