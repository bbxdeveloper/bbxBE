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
    public class DeleteInvCtrlPeriodCommand : IRequest<Response<long>>
    {
        public long ID { get; set; }

    }

    public class DeleteInvCtrlPeriodCommandHandler : IRequestHandler<DeleteInvCtrlPeriodCommand, Response<long>>
    {
        private readonly IInvCtrlPeriodRepositoryAsync _invCtrlPeriodRepository;
        private readonly IMapper _mapper;

        public DeleteInvCtrlPeriodCommandHandler(IInvCtrlPeriodRepositoryAsync InvCtrlPeriodRepository, IMapper mapper)
        {
            _invCtrlPeriodRepository = InvCtrlPeriodRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteInvCtrlPeriodCommand request, CancellationToken cancellationToken)
        {
            await _invCtrlPeriodRepository.DeleteInvCtrlPeriodAsync(request.ID);
            return new Response<long>(request.ID);
        }

      
    }
}
