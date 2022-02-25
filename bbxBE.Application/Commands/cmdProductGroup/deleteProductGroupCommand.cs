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

namespace bbxBE.Application.Commands.cmdProductGroup
{
    public class DeleteProductGroupCommand : IRequest<Response<long>>
    {
        public long ID { get; set; }

    }

    public class DeleteProductGroupCommandHandler : IRequestHandler<DeleteProductGroupCommand, Response<long>>
    {
        private readonly IProductGroupRepositoryAsync _ProductGroupRepository;
        private readonly IMapper _mapper;

        public DeleteProductGroupCommandHandler(IProductGroupRepositoryAsync ProductGroupRepository, IMapper mapper)
        {
            _ProductGroupRepository = ProductGroupRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteProductGroupCommand request, CancellationToken cancellationToken)
        {
            var cust = _mapper.Map<ProductGroup>(request);
            await _ProductGroupRepository.DeleteAsync(cust);
            return new Response<long>(cust.ID);
        }

      
    }
}
