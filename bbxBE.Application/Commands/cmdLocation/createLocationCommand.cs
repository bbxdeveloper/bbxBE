using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Common.Consts;
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

namespace bxBE.Application.Commands.cmdLocation
{
    public class CreateLocationCommand : IRequest<Response<Location>>
    {
        [ColumnLabel("Kód")]
        [Description("Kód")]
        public string LocationCode { get; set; }
        [ColumnLabel("Leírás")]
        [Description("Leírás")]
        public string LocationDescription { get; set; }

    }

    public class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, Response<Location>>
    {
        private readonly ILocationRepositoryAsync _LocationRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateLocationCommandHandler(ILocationRepositoryAsync LocationRepository, IMapper mapper, IConfiguration configuration)
        {
            _LocationRepository = LocationRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Location>> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        {
            var cust = _mapper.Map<Location>(request);

            await _LocationRepository.AddAsync(cust);
            return new Response<Location>(cust);
        }


    }
}
