using bbxBE.Application.BLL;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;



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


        [ColumnLabel("Email - csatolmányok")]
        [Description("Email - csatolmányok")]
        public System.Collections.Generic.List<Attachment> Attachments { get; set; }
    }


    /*
        CNAME  HOST                                        TYPE

        CNAME: em238.relaxvill.hu                u27222801.wl052.sendgrid.net
        CNAME: s1._domainkey.relaxvill.hu        s1.domainkey.u27222801.wl052.sendgrid.net
        CNAME: s2._domainkey.relaxvill.hu        s2.domainkey.u27222801.wl052.sendgrid.net
    */
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
                return await bllSendgrid.SendEmailAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
