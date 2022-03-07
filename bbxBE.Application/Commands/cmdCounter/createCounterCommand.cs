using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Consts;
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

namespace bxBE.Application.Commands.cmdCounter
{
    public class CreateCounterCommand : IRequest<Response<Counter>>
    {
        public string CounterCode { get; set; }
        public string CounterDescription { get; set; }

    }

    public class CreateCounterCommandHandler : IRequestHandler<CreateCounterCommand, Response<Counter>>
    {
        private readonly ICounterRepositoryAsync _CounterRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateCounterCommandHandler(ICounterRepositoryAsync CounterRepository, IMapper mapper, IConfiguration configuration)
        {
            _CounterRepository = CounterRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Counter>> Handle(CreateCounterCommand request, CancellationToken cancellationToken)
        {
            var pg = _mapper.Map<Counter>(request);

            await _CounterRepository.AddAsync(pg);
            return new Response<Counter>(pg);
        }


    }
}
