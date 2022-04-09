using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdProduct;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static bbxBE.Common.NAV.NAV_enums;

namespace bbxBE.Application.BLL
{
    public static class bllProduct
    {

        public static async Task<Product> CreateAsynch(CreateProductCommand request,
                    IProductRepositoryAsync _ProductRepository, IMapper _mapper, CancellationToken cancellationToken)
        {
            var prod = _mapper.Map<Product>(request);
           
            prod.NatureIndicator = enCustlineNatureIndicatorType.PRODUCT.ToString();
            
            prod.ProductCodes = new List<ProductCode>(); 

            var pcCode = new ProductCode() { ProductCodeCategory = enCustproductCodeCategory.OWN.ToString(), ProductCodeValue = request.ProductCode };
            prod.ProductCodes.Add(pcCode);
            var pcVTSZ = new ProductCode() { ProductCodeCategory = enCustproductCodeCategory.VTSZ.ToString(), ProductCodeValue = request.VTSZ };
            prod.ProductCodes.Add(pcVTSZ);
            ProductCode pcEAN = null;
            if (!string.IsNullOrWhiteSpace(request.EAN))
            {
                pcEAN = new ProductCode() { ProductCodeCategory = enCustproductCodeCategory.EAN.ToString(), ProductCodeValue = request.EAN };
                prod.ProductCodes.Add(pcEAN);
            }

            prod = await _ProductRepository.AddProductAsync(prod,request.ProductGroupCode, request.OriginCode, request.VatRateCode);
            return prod;
        }


        public static async Task<Product> UpdateAsynch(UpdateProductCommand request, IProductRepositoryAsync _ProductRepository,
                    IMapper _mapper, CancellationToken cancellationToken)
        {
            var prod = _mapper.Map<Product>(request);
            prod.NatureIndicator = enCustlineNatureIndicatorType.PRODUCT.ToString();
            var pcCode = new ProductCode() { ProductID = prod.ID, ProductCodeCategory = enCustproductCodeCategory.OWN.ToString(), ProductCodeValue = request.ProductCode };
            prod.ProductCodes = new List<ProductCode>();
            prod.ProductCodes.Add(pcCode);
            var pcVTSZ = new ProductCode() { ProductID = prod.ID, ProductCodeCategory = enCustproductCodeCategory.VTSZ.ToString(), ProductCodeValue = request.VTSZ };
            prod.ProductCodes.Add(pcVTSZ);

            ProductCode pcEAN = null;
            if (!string.IsNullOrWhiteSpace(request.EAN))
            {
                pcEAN = new ProductCode() { ProductID = prod.ID, ProductCodeCategory = enCustproductCodeCategory.EAN.ToString(), ProductCodeValue = request.EAN };
                prod.ProductCodes.Add(pcEAN);
            }
            await _ProductRepository.UpdateProductAsync(prod, request.ProductGroupCode, request.OriginCode, request.VatRateCode);
            return prod;
        }
    }
}
