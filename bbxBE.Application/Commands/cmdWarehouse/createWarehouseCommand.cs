using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdWarehouse
{
    public class CreateWarehouseCommand : IRequest<Response<Warehouse>>
    {
        [ColumnLabel("Raktárkód")]
        [Description("Raktárkód")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Raktár megnevezés")]
        [Description("Raktár megnevezés")]
        public string WarehouseDescription { get; set; }

    }

    public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, Response<Warehouse>>
    {
        private readonly IWarehouseRepositoryAsync _WarehouseRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateWarehouseCommandHandler(IWarehouseRepositoryAsync WarehouseRepository, IMapper mapper, IConfiguration configuration)
        {
            _WarehouseRepository = WarehouseRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Warehouse>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var wh = _mapper.Map<Warehouse>(request);

            await _WarehouseRepository.AddWarehouseAsync(wh);
            return new Response<Warehouse>(wh);
        }


    }
}
