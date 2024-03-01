using bbxBE.Application.Commands.cmdInvCtrlPeriod;
using bbxBE.Application.Queries.qInvCtrlPeriod;
using bbxBE.Common;
using bbxBE.Common.Consts;
using bxBE.Application.Commands.cmdInvCtrlPeriod;
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
    public class InvCtrlPeriodController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IHttpContextAccessor _context;

        public InvCtrlPeriodController(IWebHostEnvironment env, IConfiguration conf, IHttpContextAccessor context)
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
        public async Task<IActionResult> Get([FromQuery] GetInvCtrlPeriod filter)
        {
            return Ok(await Mediator.Send(filter));
        }



        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QueryInvCtrlPeriod filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateInvCtrlPeriodCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        [HttpPut]
        //       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateInvCtrlPeriodCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // GET: USRController/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteInvCtrlPeriodCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        //       [ValidateAntiForgeryToken]
        [HttpPatch("close")]
        public async Task<IActionResult> Close([FromQuery] CloseInvCtrlPeriodCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("report")]
        public async Task<IActionResult> Print(PrintInvCtrlPeriodCommand command)
        {

            command.JWT = Utils.getJWT(_context.HttpContext);
            var baseUrl = _conf[bbxBEConsts.CONF_BASEURL];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = $"{_context.HttpContext.Request.Scheme.ToString()}://{_context.HttpContext.Request.Host.ToString()}";
            }
            command.baseURL = baseUrl;
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(result.FileStream, "application/octet-stream", result.FileDownloadName); // returns a FileStreamResult
        }
    }
}
