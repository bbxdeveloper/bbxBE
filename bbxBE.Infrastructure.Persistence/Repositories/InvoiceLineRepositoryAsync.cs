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
        private IDataShapeHelper<InvoiceLine> _dataShaperGetAggregateInvoiceDeliveryNoteModel;
        public InvoiceLineRepositoryAsync(ApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            IDataShapeHelper<InvoiceLine> dataShaperGetAggregateInvoiceDeliveryNoteModel
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

        public async Task<List<Entity>> GetInvoiceLinesByRelDeliveryNoteIDAsync(long InvoiceID, long RelDeliveryNoteInvoiceID)
        {
            var listFieldsModel = _modelHelper.GetModelFields<GetAggregateInvoiceDeliveryNoteViewModel>();
            var res = new List<Entity>();
            var invoiceLines = await GetInvoiceLineRecordsByRelDeliveryNoteIDAsync(InvoiceID, RelDeliveryNoteInvoiceID);
            invoiceLines.ForEach(
                i =>
                {
                res.Add(
                    _dataShaperGetAggregateInvoiceDeliveryNoteModel.ShapeData(i, String.Join(",", listFieldsModel))
                    );
            });

            return res;
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