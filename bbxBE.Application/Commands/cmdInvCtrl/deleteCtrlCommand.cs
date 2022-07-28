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

namespace bbxBE.Application.Commands.cmdInvCtrl
{
    public class DeleteInvCtrlCommand : IRequest<Response<long>>
    {
        public long ID { get; set; }

    }

    public class DeleteInvCtrlCommandHandler : IRequestHandler<DeleteInvCtrlCommand, Response<long>>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;
        private readonly IMapper _mapper;

        public DeleteInvCtrlCommandHandler(IInvCtrlRepositoryAsync InvCtrlRepository, IMapper mapper)
        {
            _InvCtrlRepository = InvCtrlRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteInvCtrlCommand request, CancellationToken cancellationToken)
        {
            await _InvCtrlRepository.DeleteInvCtrlAsync(request.ID);
            return new Response<long>(request.ID);
        }

      
    }
}
