using AutoMapper;
using MediatR;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Domain.Extensions;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Attributes;
using System.ComponentModel;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using System.Text;

namespace bbxBE.Application.Queries.qInvoice
{
    public class GetPendigDeliveryNotes :  IRequest<IEnumerable<Entity>>
    {
        [ColumnLabel("Ügyfél")]
        [Description("Ügyfél ID")]
        public long CustomerID { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Pénznem")]
        [Description("Pénznem")]
        public string CurrencyCode { get; set; }
    }

    public class GetPendigDeliveryNotesHandler : IRequestHandler<GetPendigDeliveryNotes, IEnumerable<Entity>>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IWarehouseRepositoryAsync _WarehouseRepositoryAsync;
        private readonly ICacheService<Product> _productcacheService;
        private readonly ICacheService<Customer> _customerCacheService;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;
//itt tartok        
        public GetPendigDeliveryNotesHandler(IInvoiceRepositoryAsync invoiceRepository,
                IWarehouseRepositoryAsync WarehouseRepositoryAsync,
                ICacheService<Product> productCacheService,
                 ICacheService<Customer> customerCacheService,
               IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceRepository = invoiceRepository;
            _WarehouseRepositoryAsync = WarehouseRepositoryAsync;
            _productcacheService = productCacheService;
            _customerCacheService = customerCacheService;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<IEnumerable<Entity>> Handle(GetPendigDeliveryNotes request, CancellationToken cancellationToken)
        {

            var cust = await _WarehouseRepositoryAsync.GetWarehouseByCodeAsync(request.WarehouseCode);
            if (wh == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.WarehouseCode));
            }
            var wh = await _WarehouseRepositoryAsync.GetWarehouseByCodeAsync(request.WarehouseCode);
            if (wh == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.WarehouseCode));
            }
            // query based on filter
            var data = await _invoiceRepository.GetPendigDeliveryNotesAsync(request.Incoming, wh.ID, request.CurrencyCode);

            // response wrapper
            return data;
        }
    }
}