using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdCounter
{
    public class CreateCounterCommand : IRequest<Response<Counter>>
    {
        [ColumnLabel("Kód")]
        [Description("Bizonylati tömb kód")]
        public string CounterCode { get; set; }
        [ColumnLabel("Leírás")]
        [Description("Bizonylati tömb leírás")]
        public string CounterDescription { get; set; }
        [ColumnLabel("Raktárkód")]
        [Description("Raktárkód")]
        public string WarehouseCode { get; set; }
        [ColumnLabel("Előtag")]
        [Description("Előtag")]
        public string Prefix { get; set; }
        [ColumnLabel("Aktuális érték")]
        [Description("Számláló aktuális értéke")]
        public long CurrentNumber { get; set; }
        [ColumnLabel("Számláló mérete")]
        [Description("Számláló helyiértékének mérete")]
        public int NumbepartLength { get; set; }
        [ColumnLabel("Lezáró")]
        [Description("Lezáró karakter")]
        public string Suffix { get; set; }

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
            var cnt = _mapper.Map<Counter>(request);

            cnt = await _CounterRepository.AddCounterAsync(cnt, request.WarehouseCode);
            return new Response<Counter>(cnt);
        }


    }
}
