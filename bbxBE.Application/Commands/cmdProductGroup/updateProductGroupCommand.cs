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

namespace bxBE.Application.Commands.cmdProductGroup
{
    public class UpdateProductGroupCommand : IRequest<Response<ProductGroup>>
    {
        public long ID { get; set; }
        public string ProductGroupCode { get; set; }
        public string ProductGroupDescription { get; set; }
    }

    public class UpdateProductGroupCommandHandler : IRequestHandler<UpdateProductGroupCommand, Response<ProductGroup>>
    {
        private readonly IProductGroupRepositoryAsync _ProductGroupRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateProductGroupCommandHandler(IProductGroupRepositoryAsync ProductGroupRepository, IMapper mapper, IConfiguration configuration)
        {
            _ProductGroupRepository = ProductGroupRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<ProductGroup>> Handle(UpdateProductGroupCommand request, CancellationToken cancellationToken)
        {
            var pg = _mapper.Map<ProductGroup>(request);
            await _ProductGroupRepository.UpdateProductGroupAsync(pg);
            return new Response<ProductGroup>(pg);
        }


    }
}
