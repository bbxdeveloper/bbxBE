using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using MediatR;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdInvCtrlPeriod
{
    public class CloseInvCtrlPeriodCommand : IRequest<Response<bool>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long UserID { get; set; }
    }

    public class CloseInvCtrlPeriodCommandHandler : IRequestHandler<CloseInvCtrlPeriodCommand, Response<bool>>
    {
        private readonly IInvCtrlPeriodRepositoryAsync _invCtrlPeriodRepository;
        private readonly IMapper _mapper;

        public CloseInvCtrlPeriodCommandHandler(IInvCtrlPeriodRepositoryAsync InvCtrlPeriodRepository, IMapper mapper)
        {
            _invCtrlPeriodRepository = InvCtrlPeriodRepository;
            _mapper = mapper;
        }

        public async Task<Response<bool>> Handle(CloseInvCtrlPeriodCommand request, CancellationToken cancellationToken)
        {
            var res = await _invCtrlPeriodRepository.CloseAsync(request.ID, request.UserID);
            return new Response<bool>(res);
        }

    }
}
