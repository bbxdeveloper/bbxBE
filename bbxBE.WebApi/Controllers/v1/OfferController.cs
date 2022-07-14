using bbxBE.Application.Commands.cmdOffer;
using bbxBE.Application.Commands.cmdUSR_USER;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Queries.qEnum;
using bbxBE.Application.Queries.qOffer;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdOffer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using static bbxBE.Common.NAV.NAV_enums;

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
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetOffer filter)
        {
            return Ok(await Mediator.Send(filter));
        }


        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateOfferCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        [HttpPut]
        //       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateOfferCommand command)
        {
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
        public async Task<IActionResult> Query([FromQuery] QueryOffer filter)
        {
            return Ok(await Mediator.Send(filter));
        }

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
