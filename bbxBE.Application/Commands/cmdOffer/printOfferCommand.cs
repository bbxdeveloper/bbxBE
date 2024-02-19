using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common;
using bbxBE.Common.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdOffer
{
    public class PrintOfferCommand : IRequest<FileStreamResult>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

        [ColumnLabel("Példány")]
        [Description("Példány")]
        public int Copies { get; set; } = 1;

        [JsonIgnore]
        [ColumnLabel("JWT")]
        [Description("JWT")]
        public string JWT;

        [JsonIgnore]
        [ColumnLabel("Backend URL")]
        [Description("Backend URL")]
        public string baseURL;

    }

    public class PrintOfferCommandHandler : IRequestHandler<PrintOfferCommand, FileStreamResult>
    {
        private readonly IOfferRepositoryAsync _offerRepository;
        private readonly IMapper _mapper;

        public PrintOfferCommandHandler(IOfferRepositoryAsync offerRepository, IMapper mapper)
        {
            _offerRepository = offerRepository;
            _mapper = mapper;
        }

        public async Task<FileStreamResult> Handle(PrintOfferCommand request, CancellationToken cancellationToken)
        {
            var reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.Offer.trdx", Assembly.GetExecutingAssembly());
            var res = await bllOffer.CreateOfferReportAsynch(_offerRepository, reportTRDX, request, cancellationToken);

            return res;
        }


    }
}
