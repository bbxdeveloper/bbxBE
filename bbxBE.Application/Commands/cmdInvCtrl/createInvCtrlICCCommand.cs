using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdInvCtrl
{


    [Description("Leltári tétel")]
    public class createInvCtrlICCCommand : IRequest<Response<List<InvCtrl>>>
    {

        public class InvCtrlItem
        {


            [ColumnLabel("Raktár ID")]
            [Description("Raktár ID")]
            public long WarehouseID { get; set; }


            [ColumnLabel("Termék ID")]
            [Description("Termék ID")]
            public long ProductID { get; set; }

            [ColumnLabel("Leltározás dátuma")]
            [Description("Leltározás dátuma")]
            public DateTime InvCtrlDate { get; set; }

            [ColumnLabel("Új valós")]
            [Description("Új valós mennyiség")]
            public decimal NRealQty { get; set; }

            [ColumnLabel("Felhasználó ID")]
            [Description("Felhasználó ID")]
            public long? UserID { get; set; } = 0;

        }

        public List<InvCtrlItem> Items { get; set; } = new List<InvCtrlItem>();

    }

    public class CreateInvCtrlICCCommandHandler : IRequestHandler<createInvCtrlICCCommand, Response<List<InvCtrl>>>
    {
        private readonly IInvCtrlRepositoryAsync _invCtrlRepository;
        private readonly IWarehouseRepositoryAsync _warehouseRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateInvCtrlICCCommandHandler(IInvCtrlRepositoryAsync invCtrlRepository,
                        IWarehouseRepositoryAsync warehouseRepository,
                        IMapper mapper, IConfiguration configuration)
        {
            _invCtrlRepository = invCtrlRepository;
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<List<InvCtrl>>> Handle(createInvCtrlICCCommand request, CancellationToken cancellationToken)
        {
            if (request == null || request.Items.Count == 0)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_NOINPUTDATA));

            }

            var InvCtrlItems = new List<InvCtrl>();
            //mindegyik tételnek ugyan az a warehouse-ja
            var wh = await _warehouseRepository.GetWarehouseRecordAsync(request.Items.First().WarehouseID);
            if (wh == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.Items.First().WarehouseID));
            }

            request.Items.ForEach(i =>
                {
                    var InvCtrl = _mapper.Map<InvCtrl>(i);
                    InvCtrl.InvCtrlType = enInvCtrlType.ICC.ToString();
                    InvCtrl.WarehouseID = wh.ID;
                    InvCtrlItems.Add(InvCtrl);
                }
            );

            var XRel = wh.WarehouseCode + "-" + wh.WarehouseDescription + " " + request.Items.First().InvCtrlDate.ToString(bbxBEConsts.DEF_DATEFORMAT);

            await _invCtrlRepository.AddRangeInvCtrlICCAsync(InvCtrlItems, XRel);
            return new Response<List<InvCtrl>>(InvCtrlItems);
        }
    }
}