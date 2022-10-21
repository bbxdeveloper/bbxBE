using AutoMapper;
using AutoMapper.Configuration.Conventions;
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

namespace bbxBE.Application.Commands.cmdWarehouse
{
    public class DeleteWarehouseCommand : IRequest<Response<long>>
    {
        public long ID { get; set; }

    }

    public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, Response<long>>
    {
        private readonly IWarehouseRepositoryAsync _WarehouseRepository;
        private readonly IMapper _mapper;

        public DeleteWarehouseCommandHandler(IWarehouseRepositoryAsync WarehouseRepository, IMapper mapper)
        {
            _WarehouseRepository = WarehouseRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
        {
            var pg = _mapper.Map<Warehouse>(request);
            await _WarehouseRepository.RemoveAsync(pg);
            return new Response<long>(pg.ID);
        }

      
    }
}
