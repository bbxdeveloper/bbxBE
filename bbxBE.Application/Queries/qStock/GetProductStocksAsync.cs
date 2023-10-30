using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qStock
{
    public class GetProductStocks : IRequest<IEnumerable<Entity>>
    {
        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long ProductID { get; set; }
    }

    public class GetProductStocksHandler : IRequestHandler<GetProductStocks, IEnumerable<Entity>>
    {
        private readonly IStockRepositoryAsync _StockRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetProductStocksHandler(IStockRepositoryAsync StockRepository, IMapper mapper, IModelHelper modelHelper)
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
        public async Task<IEnumerable<Entity>> Handle(GetProductStocks request, CancellationToken cancellationToken)
        {
            var data = await _StockRepository.GetProductStocksAsync(request.ProductID);
            return data;
        }

    }
}