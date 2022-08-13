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
    public class GetInvCtrlICPRecord:  IRequest<InvCtrl>
    {
        public long InvCtlPeriodID { get; set; }
        public long ProductID { get; set; }
    }

    public class GetInvCtrlICPRecordHandler : IRequestHandler<GetInvCtrlICPRecord, InvCtrl>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetInvCtrlICPRecordHandler(IInvCtrlRepositoryAsync InvCtrlRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _InvCtrlRepository = InvCtrlRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<InvCtrl> Handle(GetInvCtrlICPRecord request, CancellationToken cancellationToken)
        {
            var record = await _InvCtrlRepository.GetInvCtrlICPRecordAsync(request.InvCtlPeriodID, request.ProductID);
            if( record == null ) //Jeremi kérése
            {
                record = new InvCtrl();
            }
            return record;
        }

    }
}