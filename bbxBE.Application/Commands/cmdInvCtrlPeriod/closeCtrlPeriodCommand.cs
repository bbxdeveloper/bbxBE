using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdInvCtrlPeriod
{
    public class CloseInvCtrlPeriodCommand : IRequest<Response<long>>
    {
        public long ID { get; set; }

    }

    public class CloseInvCtrlPeriodCommandHandler : IRequestHandler<CloseInvCtrlPeriodCommand, Response<long>>
    {
        private readonly IInvCtrlPeriodRepositoryAsync _invCtrlPeriodRepository;
        private readonly IMapper _mapper;

        public CloseInvCtrlPeriodCommandHandler(IInvCtrlPeriodRepositoryAsync InvCtrlPeriodRepository, IMapper mapper)
        {
            _invCtrlPeriodRepository = InvCtrlPeriodRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(CloseInvCtrlPeriodCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Not implemented: Close");
            return new Response<long>(request.ID);
        }

    }
}
