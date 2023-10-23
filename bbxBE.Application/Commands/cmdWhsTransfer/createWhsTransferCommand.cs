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
    public class CreateWhsTransferCommand : IRequest<Response<WhsTransfer>>
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


        [ColumnLabel("Kiadás raktár kód")]
        [Description("Kiadás raktár kód")]
        public string FromWarehouseCode { get; set; }

        [ColumnLabel("Bevétel raktár kód")]
        [Description("Bevétel raktár kód")]
        public string ToWarehouseCode { get; set; }

        [ColumnLabel("Kiadás dátuma")]
        [Description("Kiadás dátuma")]
        public DateTime TransferDate { get; set; }

        [ColumnLabel("Bevétel dátuma")]
        [Description("Bevétel dátuma")]
        public DateTime? TransferDateIn { get; set; }

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


    public class CreateWhsTransferCommandHandler : IRequestHandler<CreateWhsTransferCommand, Response<WhsTransfer>>
    {
        private readonly IWhsTransferRepositoryAsync _whsTransferRepositoryAsync;

        public CreateWhsTransferCommandHandler(
            IWhsTransferRepositoryAsync whsTransferRepositoryAsync
            )
        {
            _whsTransferRepositoryAsync = whsTransferRepositoryAsync;
        }

        public async Task<Response<WhsTransfer>> Handle(CreateWhsTransferCommand request, CancellationToken cancellationToken)
        {
            var wh = await _whsTransferRepositoryAsync.CreateWhsTransferAsynch(request,
                    cancellationToken);
            return new Response<WhsTransfer>(wh);
        }

    }
}
