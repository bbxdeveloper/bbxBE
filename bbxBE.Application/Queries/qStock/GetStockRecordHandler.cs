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

namespace bbxBE.Application.Queries.qStock
{
    public class GetStockRecord : IRequest<Stock>
    {
        public long WarehouseID { get; set; }
        public long ProductID { get; set; }
    }

    public class GetStockRecordHandler : IRequestHandler<GetStockRecord, Stock>
    {
        private readonly IStockRepositoryAsync _StockRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetStockRecordHandler(IStockRepositoryAsync StockRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _StockRepository = StockRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Stock> Handle(GetStockRecord request, CancellationToken cancellationToken)
        {
            var entity = await _StockRepository.GetStockRecordAsync(request);
            return entity;
        }

       }
}