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

namespace bbxBE.Application.Commands.cmdCounter
{
    public class DeleteCounterCommand : IRequest<Response<long>>
    {
        public long ID { get; set; }

    }

    public class DeleteCounterCommandHandler : IRequestHandler<DeleteCounterCommand, Response<long>>
    {
        private readonly ICounterRepositoryAsync _CounterRepository;
        private readonly IMapper _mapper;

        public DeleteCounterCommandHandler(ICounterRepositoryAsync CounterRepository, IMapper mapper)
        {
            _CounterRepository = CounterRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteCounterCommand request, CancellationToken cancellationToken)
        {
            var pg = _mapper.Map<Counter>(request);
            await _CounterRepository.DeleteAsync(pg);
            return new Response<long>(pg.ID);
        }

      
    }
}
