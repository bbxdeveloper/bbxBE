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
using System.Linq;

namespace bbxBE.Application.Queries.qInvoice
{
    public class GetPendigDeliveryNotesItems :  IRequest<IEnumerable<Entity>>
    {
        [ColumnLabel("B/K")]
        [Description("Bejővő/Kimenő")]
        public bool Incoming { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Ügyfél")]
        [Description("Ügyfél ID")]
        public long CustomerID { get; set; }

        [ColumnLabel("Pénznem")]
        [Description("Pénznem")]
        public string CurrencyCode { get; set; }
    }

    public class GetPendigDeliveryNotesItemsHandler : IRequestHandler<GetPendigDeliveryNotesItems, IEnumerable<Entity>>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IWarehouseRepositoryAsync _WarehouseRepositoryAsync;
        private readonly ICacheService<Product> _productcacheService;
        private readonly ICacheService<Customer> _customerCacheService;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;
        public GetPendigDeliveryNotesItemsHandler(IInvoiceRepositoryAsync invoiceRepository,
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

        public async Task<IEnumerable<Entity>> Handle(GetPendigDeliveryNotesItems request, CancellationToken cancellationToken)
        {

            var cust = _customerCacheService.QueryCache().Where(w => w.ID == request.CustomerID).FirstOrDefault();
            if (cust == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_CUSTOMERNOTFOUND, request.CustomerID));
            }
            var wh = await _WarehouseRepositoryAsync.GetWarehouseByCodeAsync(request.WarehouseCode);
            if (wh == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.WarehouseCode));
            }
            // query based on filter
            var data = await _invoiceRepository.GetPendigDeliveryNotesItemsAsync(request.Incoming, wh.ID, cust.ID, request.CurrencyCode);

            // response wrapper
            return data;
        }
    }
}