using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Extensions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qWhsTransfer
{
    public class GetWhsTransfer : IRequest<Entity>
    {
        public long ID { get; set; }
        //      public string Fields { get; set; }
    }

    public class GetWhsTransferHandler : IRequestHandler<GetWhsTransfer, Entity>
    {
        private readonly IWhsTransferRepositoryAsync _WhsTransferRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetWhsTransferHandler(IWhsTransferRepositoryAsync WhsTransferRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _WhsTransferRepository = WhsTransferRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetWhsTransfer request, CancellationToken cancellationToken)
        {
            var entity = await _WhsTransferRepository.GetWhsTransferAsync(request.ID);
            var data = entity.MapItemFieldsByMapToAnnotation<GetWhsTransferViewModel>();

            // response wrapper
            return data;
        }
    }
}