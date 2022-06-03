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

namespace bxBE.Application.Commands.cmdEmail
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

    public class sendEmailCommand : IRequest<Response<string>>
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
    }

    public class SendEmailCommandHandler : IRequestHandler<sendEmailCommand, Response<string>>
    {
        private readonly IConfiguration _configuration;

        public SendEmailCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Response<string>> Handle(sendEmailCommand request, CancellationToken cancellationToken)
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
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
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
