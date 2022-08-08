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

namespace bbxBE.Application.Queries.qInvCtrl
{
    public class GetInvCtrl:  IRequest<Entity>
    {
        public long ID { get; set; }
    }

    public class GetInvCtrlHandler : IRequestHandler<GetInvCtrl, Entity>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetInvCtrlHandler(IInvCtrlRepositoryAsync InvCtrlRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _InvCtrlRepository = InvCtrlRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetInvCtrl request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
          

            // query based on filter
            var entity = await _InvCtrlRepository.GetInvCtrl(validFilter);
            var data = entity.MapItemFieldsByMapToAnnotation<GetInvCtrlViewModel>();

            // response wrapper
            return data;
        }
    }
}