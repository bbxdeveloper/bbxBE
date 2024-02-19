using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common;
using bbxBE.Common.Attributes;
using bxBE.Application.Commands.cmdEmail;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdOffer
{

    public class sendOfferEmailCommand : IRequest<Response<string>>
    {
        [ColumnLabel("Email - küldő")]
        [Description("Email - küldő")]
        public _EmailAddress From { get; set; }

        [ColumnLabel("Email - címzett")]
        [Description("Email - címzett")]
        public _EmailAddress To { get; set; }

        [ColumnLabel("Email - plain text")]
        [Description("Email - plain text")]
        public string Body_plain_text { get; set; }

        [ColumnLabel("Email - html text")]
        [Description("Email - html text")]
        public string Body_html_text { get; set; }

        [ColumnLabel("Email - tárgya")]
        [Description("Email - tárgya")]
        public string Subject { get; set; }

        [ColumnLabel("Árajánlat ID")]
        [Description("Árajánlat ID")]
        public long OfferID { get; set; }

        [JsonIgnore]
        [ColumnLabel("JWT")]
        [Description("JWT")]
        public string JWT;

        [JsonIgnore]
        [ColumnLabel("Backend URL")]
        [Description("Backend URL")]
        public string baseURL;

    }

    public class sendOfferEmailCommandHandler : IRequestHandler<sendOfferEmailCommand, Response<string>>
    {
        private readonly IOfferRepositoryAsync _offerRepository;
        private readonly IConfiguration _configuration;

        public sendOfferEmailCommandHandler(IOfferRepositoryAsync offerRepository, IConfiguration configuration)
        {
            _offerRepository = offerRepository;
            _configuration = configuration;
        }

        public async Task<Response<string>> Handle(sendOfferEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                PrintOfferCommand po = new PrintOfferCommand() { ID = request.OfferID, baseURL = request.baseURL, JWT = request.JWT };
                var reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.Offer.trdx", Assembly.GetExecutingAssembly());
                var res = await bllOffer.CreateOfferReportAsynch(_offerRepository, reportTRDX, po, cancellationToken);

                var att = new Attachment()
                {
                    Filename = res.FileDownloadName,
                    Content = Utils.ConvertStreamToBase64(res.FileStream),
                    Type = "application/pdf",
                    ContentId = "ContentId"
                };
                var emailCommand = new sendEmailCommand()
                {
                    From = request.From,
                    To = request.To,
                    Body_plain_text = request.Body_plain_text,
                    Body_html_text = request.Body_html_text,
                    Subject = request.Subject,
                    Attachments = new System.Collections.Generic.List<Attachment>() { att }
                };

                var result = await bllSendgrid.SendEmailAsync(emailCommand, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
