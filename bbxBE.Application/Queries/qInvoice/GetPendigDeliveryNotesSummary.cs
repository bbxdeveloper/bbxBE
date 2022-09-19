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

namespace bbxBE.Application.Queries.qInvoice
{
    public class GetPendigDeliveryNotesSummary :  IRequest<Entity>
    {
        [ColumnLabel("B/K")]
        [Description("Bejővő/Kimenő")]
        public bool Incoming { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Pénznem")]
        [Description("Pénznem")]
        public string CurrencyCode { get; set; }
    }

    public class GetPendigDeliveryNotesSummaryHandler : IRequestHandler<GetPendigDeliveryNotesSummary, Entity>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IWarehouseRepositoryAsync _WarehouseRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetPendigDeliveryNotesSummaryHandler(IInvoiceRepositoryAsync invoiceRepository,
                IWarehouseRepositoryAsync WarehouseRepositoryAsync, 
                IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceRepository = invoiceRepository;
            _WarehouseRepositoryAsync = WarehouseRepositoryAsync;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetPendigDeliveryNotesSummary request, CancellationToken cancellationToken)
        {

            var wh = await _WarehouseRepositoryAsync.GetWarehouseByCodeAsync(request.WarehouseCode);
            if (wh == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.WarehouseCode));
            }
            // query based on filter
            var entity = await _invoiceRepository.GetPendigDeliveryNotesSummareAsync(request);
            var data = entity.MapItemFieldsByMapToAnnotation<GetInvoiceViewModel>();

            // response wrapper
            return data;
        }
    }
}