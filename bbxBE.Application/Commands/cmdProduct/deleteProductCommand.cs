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

namespace bbxBE.Application.Commands.cmdProduct
{
    public class DeleteProductCommand : IRequest<Response<long>>
    {
        public long ID { get; set; }

    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Response<long>>
    {
        private readonly IProductRepositoryAsync _ProductRepository;
        private readonly IMapper _mapper;

        public DeleteProductCommandHandler(IProductRepositoryAsync ProductRepository, IMapper mapper)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var cust = _mapper.Map<Product>(request);
            await _ProductRepository.DeleteAsync(cust);
            return new Response<long>(cust.ID);
        }

      
    }
}
