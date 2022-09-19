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

namespace bbxBE.Application.Queries.qStockCard
{
    public class GetStockCard:  IRequest<Entity>
    {
        public long ID { get; set; }
    }

    public class GetStockCardHandler : IRequestHandler<GetStockCard, Entity>
    {
        private readonly IStockCardRepositoryAsync _StockCardRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public   GetStockCardHandler(IStockCardRepositoryAsync StockCardRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _StockCardRepository = StockCardRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetStockCard request, CancellationToken cancellationToken)
        {
            var entity = await _StockCardRepository.GetStockCardAsync(request.ID);
            var data = entity.MapItemFieldsByMapToAnnotation<GetStockCardViewModel>();

            // response wrapper
            return data;
        }
    }
}