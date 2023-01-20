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
    public class updateStockLocationCommand : IRequest<Response<Stock>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

            
        [ColumnLabel("Elhelyezés ID")]
        [Description("Elhelyezés ID")]
        public long? LocationID { get; set; }
    }

    public class updateStockLocationCommandHandler : IRequestHandler<updateStockLocationCommand, Response<Stock>>
    {
        private readonly ILocationRepositoryAsync _LocationRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public updateStockLocationCommandHandler(ILocationRepositoryAsync LocationRepository, IMapper mapper, IConfiguration configuration)
        {
            _LocationRepository = LocationRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Location>> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
        {
            var cust = _mapper.Map<Location>(request);

            await _LocationRepository.UpdateLocationAsync(cust);
            return new Response<Location>(cust);
        }


    }
}
