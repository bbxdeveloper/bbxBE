using bbxBE.Application.Wrappers;
using bxBE.Application.Commands.cmdEmail;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.BLL
{
    public static class bllSendgrid
    {
        public static async Task<Response<string>> SendEmailAsync(sendEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    var client = new SendGridClient(apiKey);
                    var from = request.From;
                    var subject = request.Subject;
                    var to = request.To;
                    var plainTextContent = request.Body_plain_text;
                    var htmlContent = request.Body_html_text;
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                    if (request.Attachments != null && request.Attachments.Any())
                    {
                        msg.Attachments = request.Attachments;
                    }

                    var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

                    var body = await response.Body.ReadAsStreamAsync();
                    StreamReader reader = new StreamReader(body);
                    string bodyText = reader.ReadToEnd();

                    return new Response<string>
                    {
                        Succeeded = response.IsSuccessStatusCode,
                        Message = response.StatusCode.ToString(),
                        Data = bodyText
                    };
                }
                else
                {
                    return new Response<string>() { Succeeded = false, Message = "SENDGRID_API_KEY is not defined!" };
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
