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

            var pcCode = new ProductCode() { ProductCodeCategory = enCustproductCodeCategory.OWN.ToString(), ProductCodeValue = request.ProductCode };
            var pcVTSZ = new ProductCode() { ProductCodeCategory = enCustproductCodeCategory.VTSZ.ToString(), ProductCodeValue = request.VTSZ };
            ProductCode pcEAN = null;
            if (!string.IsNullOrWhiteSpace(request.EAN))
            {
                pcEAN = new ProductCode() { ProductCodeCategory = enCustproductCodeCategory.EAN.ToString(), ProductCodeValue = request.EAN };
            }

            prod = await _ProductRepository.AddProductAsync(prod, pcCode, pcVTSZ, pcEAN, request.ProductGroupCode, request.OriginCode);
            return prod;
        }


        public static async Task<Product> UpdateAsynch(UpdateProductCommand request, IProductRepositoryAsync _ProductRepository,
                    IMapper _mapper, CancellationToken cancellationToken)
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

            await _ProductRepository.UpdateProductAsync(prod, pcCode, pcVTSZ, pcEAN, request.ProductGroupCode, request.OriginCode);
            return prod;
        }
    }
}
