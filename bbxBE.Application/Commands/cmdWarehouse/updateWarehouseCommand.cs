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
    public class UpdateWarehouseCommand : IRequest<Response<Warehouse>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }
        [ColumnLabel("Raktárkód")]
        [Description("Raktárkód")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Raktár megnevezés")]
        [Description("Raktár megnevezés")]
        public string WarehouseDescription { get; set; }

    }

    public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, Response<Warehouse>>
    {
        private readonly IWarehouseRepositoryAsync _WarehouseRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateWarehouseCommandHandler(IWarehouseRepositoryAsync WarehouseRepository, IMapper mapper, IConfiguration configuration)
        {
            _WarehouseRepository = WarehouseRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Warehouse>> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var pg = _mapper.Map<Warehouse>(request);

            await _WarehouseRepository.UpdateWarehouseAsync(pg);
            return new Response<Warehouse>(pg);
        }


    }
}
