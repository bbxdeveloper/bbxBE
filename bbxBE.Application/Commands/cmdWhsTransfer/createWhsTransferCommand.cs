using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdWarehouse
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
        public long ToWarehouseCode { get; set; }

        [ColumnLabel("Dátum")]
        [Description("Dátum")]
        public long TransferDate { get; set; }


        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;



        [ColumnLabel("Bizonylatsorok")]
        [Description("Bizonylatsorok")]
        public virtual ICollection<WhsTransferLine> WhsTransferLines { get; set; }

    }

    public class CreateWhsTransferCommandHandler : IRequestHandler<CreateWhsTransferCommand, Response<WhsTransfer>>
    {
        private readonly IWhsTransferRepositoryAsync _IWhsTransferRepositoryAsyncy;
        private readonly IWarehouseRepositoryAsync _WarehouseRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateWhsTransferCommandHandler(IWarehouseRepositoryAsync WarehouseRepository, IMapper mapper, IConfiguration configuration)
        {
            _WarehouseRepository = WarehouseRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<WhsTransfer>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var wh = _mapper.Map<Warehouse>(request);

            await _WarehouseRepository.AddWarehouseAsync(wh);
            return new Response<WhsTransfer>(wh);
        }


    }
}
