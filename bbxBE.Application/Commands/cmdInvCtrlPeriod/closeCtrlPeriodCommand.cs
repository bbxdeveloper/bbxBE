using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Consts;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdInvCtrlPeriod
{
    public class CloseInvCtrlPeriodCommand : IRequest<Response<bool>>
    {
        public long ID { get; set; }

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
            var res = await _invCtrlPeriodRepository.CloseAsync(request.ID);
            return new Response<bool>(res);
        }

    }
}
