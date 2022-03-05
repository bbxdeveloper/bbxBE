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

namespace bxBE.Application.Commands.cmdWarehouse
{
    public class UpdateWarehouseCommand : IRequest<Response<Warehouse>>
    {
        public long ID { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseDescription { get; set; }
    }

    public class UpdateUSR_USERCommandHandler : IRequestHandler<UpdateWarehouseCommand, Response<Warehouse>>
    {
        private readonly IWarehouseRepositoryAsync _WarehouseRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateUSR_USERCommandHandler(IWarehouseRepositoryAsync WarehouseRepository, IMapper mapper, IConfiguration configuration)
        {
            _WarehouseRepository = WarehouseRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Warehouse>> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var pg = _mapper.Map<Warehouse>(request);

            await _WarehouseRepository.UpdateAsync(pg);
            return new Response<Warehouse>(pg);
        }


    }
}
