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
    public class UpdateCounterCommand : IRequest<Response<Counter>>
    {
        public long ID { get; set; }
        public string CounterCode { get; set; }
        public string CounterDescription { get; set; }
        public string WarehouseCode { get; set; }
        public string Prefix { get; set; }
        public long CurrentNumber { get; set; }
        public int NumbepartLength { get; set; }
        public string Suffix { get; set; }
    }

    public class UpdateCounterCommandHandler : IRequestHandler<UpdateCounterCommand, Response<Counter>>
    {
        private readonly ICounterRepositoryAsync _CounterRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateCounterCommandHandler(ICounterRepositoryAsync CounterRepository, IMapper mapper, IConfiguration configuration)
        {
            _CounterRepository = CounterRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Counter>> Handle(UpdateCounterCommand request, CancellationToken cancellationToken)
        {
            var cnt  = _mapper.Map<Counter>(request);

            cnt = await _CounterRepository.UpdateCounterAsync(cnt, request.WarehouseCode);
            return new Response<Counter>(cnt);
        }


    }
}
