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

namespace bbxBE.Application.Commands.cmdOrigin
{
    public class DeleteOriginCommand : IRequest<Response<long>>
    {
        public long ID { get; set; }

    }

    public class DeleteOriginCommandHandler : IRequestHandler<DeleteOriginCommand, Response<long>>
    {
        private readonly IOriginRepositoryAsync _OriginRepository;
        private readonly IMapper _mapper;

        public DeleteOriginCommandHandler(IOriginRepositoryAsync OriginRepository, IMapper mapper)
        {
            _OriginRepository = OriginRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteOriginCommand request, CancellationToken cancellationToken)
        {
            var cust = _mapper.Map<Origin>(request);
            await _OriginRepository.DeleteAsync(cust);
            return new Response<long>(cust.ID);
        }

      
    }
}
