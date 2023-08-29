using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qInvCtrl
{
    public class CSVInvCtrl : IRequest<FileStreamResult>
    {
        [ColumnLabel("Leltáridőszak ID")]
        [Description("Leltáridőszak ID")]
        public long? InvCtrlPeriodID { get; set; }          //Leltáridőszaki leltár esetén értlemezett

        [ColumnLabel("Kezdődátum")]
        [Description("Kezdődátum")]
        public DateTime? DateFrom { get; set; }             //Folyamatos leltár esetén értelmezett

        [ColumnLabel("Végdátum")]
        [Description("Végdátum")]
        public DateTime? DateTo { get; set; }               //Folyamatos leltár esetén értelmezett

        [ColumnLabel("Keresett adat")]
        [Description("Keresett adat")]
        public string SearchString { get; set; }

        [ColumnLabel("Hiány/többlet")]
        [Description("Hiány/többlet lekérdezése")]
        public bool? ShowDeficit { get; set; }              //null=minden adat, True=leltárhiány, False=leltártöbblet
    }

    public class CSVInvCtrlHandler : IRequestHandler<CSVInvCtrl, FileStreamResult>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public CSVInvCtrlHandler(IInvCtrlRepositoryAsync InvCtrlRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _InvCtrlRepository = InvCtrlRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<FileStreamResult> Handle(CSVInvCtrl request, CancellationToken cancellationToken)
        {
            QueryInvCtrl query = new QueryInvCtrl()
            {
                InvCtrlPeriodID = request.InvCtrlPeriodID,
                SearchString = request.SearchString,
                ShowDeficit = request.ShowDeficit,
                PageSize = int.MaxValue
            };
            var res = await _InvCtrlRepository.QueryPagedInvCtrlViewModelAsync(query);


            string csv = String.Join(Environment.NewLine,
            res.data.Select(x =>
                        x.ID.ToString() + bbxBEConsts.DEF_CSVSEP +
                        x.InvCtrlType + bbxBEConsts.DEF_CSVSEP +
                        x.InvCtrlTypeX + bbxBEConsts.DEF_CSVSEP +
                        x.WarehouseID.ToString() + bbxBEConsts.DEF_CSVSEP +
                        x.Warehouse + bbxBEConsts.DEF_CSVSEP +
                        x.InvCtlPeriod + bbxBEConsts.DEF_CSVSEP +
                        x.ProductCode + bbxBEConsts.DEF_CSVSEP +
                        x.Product + bbxBEConsts.DEF_CSVSEP +
                        x.InvCtrlDate.ToString(bbxBEConsts.DEF_DATEFORMAT) + bbxBEConsts.DEF_CSVSEP +
                        x.ORealQty.ToString() + bbxBEConsts.DEF_CSVSEP +
                        x.NRealQty.ToString() + bbxBEConsts.DEF_CSVSEP +
                        x.AvgCost.ToString() + bbxBEConsts.DEF_CSVSEP +
                        x.ORealAmount.ToString() + bbxBEConsts.DEF_CSVSEP +
                        x.NRealAmount.ToString() + bbxBEConsts.DEF_CSVSEP +
                        x.UserName + Environment.NewLine
            ).ToArray());

            var csvHeader = "ID;Típus;Típus megnevezés;Raktár ID;Raktár;Leltáridőszak;Termékkód;Termék;Leltározás dátuma;Eredeti mennyiség;Új mennyiség;ELÁBÉ;Eredeti érték;Új érték;Felhasználó";

            Random rnd = new Random();
            string fileName = $"InvCtrl_{rnd.Next()}.csv";
            var sbContent = new StringBuilder();

            sbContent.AppendLine(csvHeader);
            sbContent.Append(csv);

            var enc = Encoding.GetEncoding(bbxBEConsts.DEF_ENCODING);
            Stream stream = new MemoryStream(enc.GetBytes(sbContent.ToString()));
            var fsr = new FileStreamResult(stream, $"application/csv") { FileDownloadName = fileName };

            return fsr;

        }
    }
}