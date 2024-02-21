using bxBE.Application.Commands.cmdEmail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]

#if (!DEBUG)
    [Authorize]
#else
        [AllowAnonymous]
#endif
    public class InfrastructureController : BaseApiController
    {

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IHttpContextAccessor _context;

        public InfrastructureController(
           IWebHostEnvironment env,
           IConfiguration conf,
            IHttpContextAccessor context)
        {
            _env = env;
            _conf = conf;
            _context = context;
        }


        /// <summary>
        /// POST - Send email with: @from, @to, @subject, @plainTextBody, @htmlBody parameters
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail(sendEmailCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


    }
}
