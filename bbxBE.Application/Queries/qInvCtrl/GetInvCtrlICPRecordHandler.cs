using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qInvCtrl
{
    public class GetInvCtrlICPRecord : IRequest<InvCtrl>
    {
        [ColumnLabel("Leltáridőszak ID")]
        [Description("Leltáridőszak ID")]
        public long InvCtlPeriodID { get; set; }

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
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
            if (record == null) //Jeremi kérése
            {
                record = new InvCtrl();
            }
            return record;
        }

    }
}