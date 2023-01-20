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
    public class UpdateStockLocationCommand : IRequest<Response<Stock>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

            
        [ColumnLabel("Elhelyezés ID")]
        [Description("Elhelyezés ID")]
        public long? LocationID { get; set; }
    }

    public class UpdateStockLocationCommandHandler : IRequestHandler<UpdateStockLocationCommand, Response<Stock>>
    {
        private readonly IStockRepositoryAsync _StockRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateStockLocationCommandHandler(IStockRepositoryAsync stockRepositoryy, IMapper mapper, IConfiguration configuration)
        {
            _StockRepository = stockRepositoryy;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Stock>> Handle(UpdateStockLocationCommand request, CancellationToken cancellationToken)
        {
            var stock = await _StockRepository.UpdateStockLocationAsync(request.ID, request.LocationID);
            return new Response<Stock>(stock);
        }

    }
}
