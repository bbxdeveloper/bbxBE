using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qInvoice
{
    public class GetPendigDeliveryNotesSummary : IRequest<IEnumerable<Entity>>
    {
        [ColumnLabel("B/K")]
        [Description("Bejővő/Kimenő")]
        public bool Incoming { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string WarehouseCode { get; set; }

    }

    public class GetPendigDeliveryNotesSummaryHandler : IRequestHandler<GetPendigDeliveryNotesSummary, IEnumerable<Entity>>
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

        public async Task<IEnumerable<Entity>> Handle(GetPendigDeliveryNotesSummary request, CancellationToken cancellationToken)
        {

            var wh = await _WarehouseRepositoryAsync.GetWarehouseByCodeAsync(request.WarehouseCode);
            if (wh == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.WarehouseCode));
            }
            // query based on filter
            var data = await _invoiceRepository.GetPendigDeliveryNotesSummaryAsync(request.Incoming, wh.ID);

            // response wrapper
            return data;
        }
    }
}