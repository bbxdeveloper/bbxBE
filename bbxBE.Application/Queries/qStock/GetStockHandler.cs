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
    public class GetStock : IRequest<Entity>
    {
        public long ID { get; set; }
    }

    public class GetStockHandler : IRequestHandler<GetStock, Entity>
    {
        private readonly IStockRepositoryAsync _StockRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetStockHandler(IStockRepositoryAsync StockRepository, IMapper mapper, IModelHelper modelHelper)
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
        public async Task<Entity> Handle(GetStock request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;


            // query based on filter
            var entity = await _StockRepository.GetStockAsync(validFilter);
            var data = entity.MapItemFieldsByMapToAnnotation<GetStockViewModel>();

            // response wrapper
            return data;
        }
    }
}