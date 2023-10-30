using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
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

        public UpdateWhsTransferCommandHandler(
            IWhsTransferRepositoryAsync whsTransferRepositoryAsyncy
            )
        {
            _whsTransferRepositoryAsyncy = whsTransferRepositoryAsyncy;
        }

        public async Task<Response<WhsTransfer>> Handle(UpdateWhsTransferCommand request, CancellationToken cancellationToken)
        {
            var wh = await _whsTransferRepositoryAsyncy.UpdateWhsTransferAsynch(request, cancellationToken);
            return new Response<WhsTransfer>(wh);
        }
    }
}
