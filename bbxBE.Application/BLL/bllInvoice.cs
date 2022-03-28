using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdInvoice;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.BLL
{

      public static class bllInvoice
    {
        public static async Task<Invoice> CreateAsynch(createOutgoingInvoiceCommand request,
                   IInvoiceRepositoryAsync _InvoiceRepository, IMapper _mapper, CancellationToken cancellationToken)
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



            prod = await _ProductRepository.AddProductAsync(prod, pcCode, pcVTSZ, pcEAN, request.ProductGroupCode, request.OriginCode, request.VatRateCode);
            return prod;
        }
    }
}
