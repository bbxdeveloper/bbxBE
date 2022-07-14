using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using bbxBE.Application.Wrappers;
using System.ComponentModel;
using bbxBE.Common.Attributes;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.BLL;
using System.Reflection;
using System.IO;
using bbxBE.Common;

namespace bbxBE.Application.Commands.cmdOffer
{
    public class _EmailAddress : EmailAddress
    {
        [ColumnLabel("Email cím megnevezése")]
        [Description("Email cím megnevezése")]
        public string Name { get; set; }

        [ColumnLabel("Email cím")]
        [Description("Email cím")]
        public string Email { get; set; }
    }

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


                var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                var client = new SendGridClient(apiKey);
                var from = request.From;
                var subject = request.Subject;
                var to = request.To;
                var plainTextContent = request.Body_plain_text;
                var htmlContent = request.Body_html_text;


                PrintOfferCommand po = new PrintOfferCommand() { ID = request.OfferID, baseURL = request.baseURL };
                var reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.Offer.trdx", Assembly.GetExecutingAssembly());
                var res = await bllOffer.CreateOfferReportAsynch(_offerRepository, reportTRDX, po, cancellationToken);

                var att = new Attachment()
                {
                    Filename = res.FileDownloadName,
                    Content = Utils.ConvertStreamToBase64(res.FileStream),
                    Type = "application/pdf",
                    ContentId = "ContentId"
                };

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                msg.Attachments = new System.Collections.Generic.List<Attachment>();
                msg.Attachments.Add(att);


                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

                return new Response<string>
                {
                    Succeeded = true,
                    Message = response.StatusCode.ToString()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


 
    }
}
