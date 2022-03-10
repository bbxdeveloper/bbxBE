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
using static bbxBE.Common.NAV.NAV_enums;

namespace bxBE.Application.Commands.cmdProduct
{
    public class CreateProductCommand : IRequest<Response<Product>>
    {
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string ProductGroupCode { get; set; }
        public string OriginCode { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal UnitPrice1 { get; set; }
        public decimal UnitPrice2 { get; set; }
        public decimal LatestSupplyPrice { get; set; }
        public bool IsStock { get; set; }
        public decimal MinStock { get; set; }
        public decimal OrdUnit { get; set; }
        public decimal ProductFee { get; set; }
        public bool Active { get; set; }
        public string VTSZ { get; set; }
        public string EAN { get; set; }
    }

    public class CreateUSR_USERCommandHandler : IRequestHandler<CreateProductCommand, Response<Product>>
    {
        private readonly IProductRepositoryAsync _ProductRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateUSR_USERCommandHandler(IProductRepositoryAsync ProductRepository, IMapper mapper, IConfiguration configuration)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Product>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var prod = await bllProduct.CreateAsynch(request, _ProductRepository, _mapper, cancellationToken);
            return new Response<Product>(prod);
        }
    }
}
