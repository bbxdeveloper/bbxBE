using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdProduct
{
    public class CreateProductCommand : IRequest<Response<Product>>
    {
        [ColumnLabel("Termékkód")]
        [Description("Termékkód")]
        public string ProductCode { get; set; }
        [ColumnLabel("Leírás")]
        [Description("LEírás/megnevezés")]
        public string Description { get; set; }
        [ColumnLabel("Termékcsoport")]
        [Description("Termékcsoport kód")]
        public string ProductGroupCode { get; set; }
        [ColumnLabel("Származási hely")]
        [Description("Származási hely kód")]
        public string OriginCode { get; set; }
        [ColumnLabel("Me.e.")]
        [Description("Mennyiségi egység kód")]
        public string UnitOfMeasure { get; set; }
        [ColumnLabel("Ár1")]
        [Description("Eladási ár1")]
        public decimal UnitPrice1 { get; set; }
        [ColumnLabel("Ár2")]
        [Description("Eladási ár2")]
        public decimal UnitPrice2 { get; set; }

        [ColumnLabel("Áfaleíró-kód")]
        [Description("Áfaleíró-kód")]
        public string VatRateCode { get; set; }

        [ColumnLabel("Utolsó beszerzés")]
        [Description("Utolsó beszerzés dátuma")]
        public decimal LatestSupplyPrice { get; set; }
        [ColumnLabel("Készletes?")]
        [Description("Készletes?")]
        public bool IsStock { get; set; }
        [ColumnLabel("Min.klt")]
        [Description("Minimum készlet")]
        public decimal MinStock { get; set; }
        [ColumnLabel("Rend.egys.")]
        [Description("Rendelési egység")]
        public decimal OrdUnit { get; set; }
        [ColumnLabel("Termékdíj")]
        [Description("Termékdíj")]
        public decimal ProductFee { get; set; }
        [ColumnLabel("Aktív?")]
        [Description("Aktív?")]
        public bool Active { get; set; }
        [ColumnLabel("Eng.tilt")]
        [Description("Engedmény adás tiltása")]
        public bool NoDiscount { get; set; }
        [ColumnLabel("VTSZ")]
        [Description("Vámtarifa-szám")]
        public string VTSZ { get; set; }
        [ColumnLabel("EAN")]
        [Description("Vonalkód")]
        public string EAN { get; set; }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Response<Product>>
    {
        private readonly IProductRepositoryAsync _productRepository;

        public CreateProductCommandHandler(IProductRepositoryAsync productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Response<Product>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var prod = await _productRepository.CreateAsynch(request, cancellationToken);
            return new Response<Product>(prod);
        }
    }
}
