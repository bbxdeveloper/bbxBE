using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Consts;
using bbxBE.Application.Exceptions;
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
    public class GetNextNumberCommand : IRequest<Response<string>>
    {
        [ColumnLabel("Kód")]
        [Description("Bizonylati tömb kód")]
        public string CounterCode { get; set; }
        
        [ColumnLabel("Raktárkód")]
        [Description("Raktárkód")]
        public string WarehouseCode { get; set; }
    }

    public class GetNextNumberCommandHandler : IRequestHandler<GetNextNumberCommand, Response<string>>
    {
        private readonly ICounterRepositoryAsync _CounterRepository;
        private readonly IWarehouseRepositoryAsync _WarehouseRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetNextNumberCommandHandler(ICounterRepositoryAsync CounterRepository,
            IWarehouseRepositoryAsync WarehouseRepositoryAsync,
            IMapper mapper, IConfiguration configuration)
        {
            _CounterRepository = CounterRepository;
            _WarehouseRepositoryAsync = WarehouseRepositoryAsync;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<string>> Handle(GetNextNumberCommand request, CancellationToken cancellationToken)
        {
            var wh = await _WarehouseRepositoryAsync.GetWarehouseByCodeAsync(request.WarehouseCode);
            if(wh == null) 
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.WarehouseCode));
            }

            var next = await _CounterRepository.GetNextValueAsync(request.CounterCode, wh.ID);
            return new Response<string>(next);
        }


    }
}
