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
    public class UpdateProductCommand : IRequest<Response<Product>>
    {
        public long ID { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public long ProductGroupID { get; set; }
        public long OriginID { get; set; }
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

    public class UpdateUSR_USERCommandHandler : IRequestHandler<UpdateProductCommand, Response<Product>>
    {
        private readonly IProductRepositoryAsync _ProductRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateUSR_USERCommandHandler(IProductRepositoryAsync ProductRepository, IMapper mapper, IConfiguration configuration)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Product>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var prod = _mapper.Map<Product>(request);
            prod.NatureIndicator = enCustlineNatureIndicatorType.PRODUCT.ToString();
            var pcCode = new ProductCode() { ProductID = prod.ID, ProductCodeCategory = enCustproductCodeCategory.OWN.ToString(), ProductCodeValue = request.ProductCode };
            var pcVTSZ = new ProductCode() { ProductID = prod.ID, ProductCodeCategory = enCustproductCodeCategory.VTSZ.ToString(), ProductCodeValue = request.VTSZ };
            ProductCode pcEAN = null;
            if (!string.IsNullOrWhiteSpace(request.EAN))
            {
                pcEAN = new ProductCode() { ProductID = prod.ID, ProductCodeCategory = enCustproductCodeCategory.EAN.ToString(), ProductCodeValue = request.EAN };
            }

            await _ProductRepository.UpdateProductAsync(prod, pcCode, pcVTSZ, pcEAN);
            return new Response<Product>(prod);
        }


    }
}
