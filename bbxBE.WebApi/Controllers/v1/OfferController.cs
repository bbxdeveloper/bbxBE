using bbxBE.Application.Commands.cmdOffer;
using bbxBE.Application.Queries.qOffer;
using bbxBE.Common;
using bxBE.Application.Commands.cmdOffer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    //   [Authorize]
    public class OfferController : BaseApiController
    {

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IHttpContextAccessor _context;

        public OfferController(
           IWebHostEnvironment env,
           IConfiguration conf,
            IHttpContextAccessor context)
        {
            _env = env;
            _conf = conf;
            _context = context;
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetOffer request)
        {
            return Ok(await Mediator.Send(request));
        }


        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateOfferCommand command)
        {

            if (command.UserID == null || command.UserID == 0)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                command.UserID = Utils.GetUserIDFromClaimsIdentity(identity);
            }
            return Ok(await Mediator.Send(command));
        }


        [HttpPut]
        //       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateOfferCommand command)
        {
            if (command.UserID == null || command.UserID == 0)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                command.UserID = Utils.GetUserIDFromClaimsIdentity(identity);
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteOfferCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QueryOffer request)
        {
            return Ok(await Mediator.Send(request));
        }

        /// <summary>
        /// Get Offer report in PDF format
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("print")]
        public async Task<IActionResult> Print(PrintOfferCommand command)
        {

            command.baseURL = $"{_context.HttpContext.Request.Scheme.ToString()}://{_context.HttpContext.Request.Host.ToString()}";
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(result.FileStream, "application/octet-stream", result.FileDownloadName); // returns a FileStreamResult
        }

        [HttpGet("csv")]
        public async Task<IActionResult> Csv([FromQuery] GetOfferCSV command)
        {

            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(result.FileStream, "application/octet-stream", result.FileDownloadName); // returns a FileStreamResult
        }

        /// <summary>
        /// POST - Send email with: @from, @to, @subject, @plainTextBody, @htmlBody parameters
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("SendOfferEmail")]
        public async Task<IActionResult> SendOfferEmail(sendOfferEmailCommand command)
        {
            command.baseURL = $"{_context.HttpContext.Request.Scheme.ToString()}://{_context.HttpContext.Request.Host.ToString()}";
            return Ok(await Mediator.Send(command));
        }
    }
}
