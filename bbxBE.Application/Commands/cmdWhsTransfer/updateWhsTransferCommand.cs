using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdWhsTransfer
{
    public class UpdateWhsTransferCommand : IRequest<Response<WhsTransfer>>
    {

        [Description("Raktárközi átadás-sor")]
        public class WhsTransferLine
        {
            [ColumnLabel("Sor száma")]
            [Description("Sor száma")]
            public short WhsTransferLineNumber { get; set; }

            [ColumnLabel("Termékkód")]
            [Description("Termékkód")]
            public string ProductCode { get; set; }

            [ColumnLabel("Mennyiség")]
            [Description("Mennyiség")]
            public decimal Quantity { get; set; }
            [ColumnLabel("Me")]
            [Description("Mennyiségi egység")]
            public string UnitOfMeasure { get; set; }

            [ColumnLabel("Aktuális beszerzési egységár")]
            [Description("Aktuális beszerzési egységár")]

            public decimal CurrAvgCost { get; set; }
        }

        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

        [ColumnLabel("Kiadás raktár kód")]
        [Description("Kiadás raktár kód")]
        public string FromWarehouseCode { get; set; }

        [ColumnLabel("Bevétel raktár kód")]
        [Description("Bevétel raktár kód")]
        public string ToWarehouseCode { get; set; }

        [ColumnLabel("Dátum")]
        [Description("Dátum")]
        public DateTime TransferDate { get; set; }

        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Notice { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;


        [ColumnLabel("Bizonylatsorok")]
        [Description("Bizonylatsorok")]
        public List<WhsTransferLine> WhsTransferLines { get; set; }

    }


    public class UpdateWhsTransferCommandHandler : IRequestHandler<UpdateWhsTransferCommand, Response<WhsTransfer>>
    {
        private readonly IWhsTransferRepositoryAsync _whsTransferRepositoryAsyncy;
        private readonly IWarehouseRepositoryAsync _warehouseRepositoryAsync;
        private readonly ICounterRepositoryAsync _counterRepository;
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateWhsTransferCommandHandler(
            IWhsTransferRepositoryAsync whsTransferRepositoryAsyncy,
            IWarehouseRepositoryAsync warehouseRepositoryAsync,
            ICounterRepositoryAsync counterRepository,
            IProductRepositoryAsync productRepository,
            IMapper mapper, IConfiguration configuration)
        {
            _whsTransferRepositoryAsyncy = whsTransferRepositoryAsyncy;
            _warehouseRepositoryAsync = warehouseRepositoryAsync;
            _counterRepository = counterRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<WhsTransfer>> Handle(UpdateWhsTransferCommand request, CancellationToken cancellationToken)
        {
            var wh = await bllWhsTransfer.UpdateWhsTransferAsynch(request, _mapper,
                    _whsTransferRepositoryAsyncy, _warehouseRepositoryAsync, _counterRepository, _productRepository,
                    cancellationToken);
            return new Response<WhsTransfer>(wh);
        }

    }
}
