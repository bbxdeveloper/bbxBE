using bbxBE.Application.Commands.cmdWhsTransfer;
using bbxBE.Application.Queries.qEnum;
using bbxBE.Application.Queries.qWhsTransfer;
using bbxBE.Common;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bxBE.Application.Commands.cmdWhsTransfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
#if (!DEBUG)
    [Authorize]
#else
    [AllowAnonymous]
#endif
    public class WhsTransferController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IHttpContextAccessor _context;
        public WhsTransferController(IWebHostEnvironment env, IConfiguration conf, IHttpContextAccessor context)
        {
            _env = env;
            _conf = conf;
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetWhsTransfer request)
        {
            return Ok(await Mediator.Send(request));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QueryWhsTransfer request)
        {
            return Ok(await Mediator.Send(request));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateWhsTransferCommand command)
        {
            if (command.UserID == null || command.UserID == 0)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                command.UserID = Utils.GetUserIDFromClaimsIdentity(identity);
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateWhsTransferCommand command)
        {
            if (command.UserID == null || command.UserID == 0)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                command.UserID = Utils.GetUserIDFromClaimsIdentity(identity);
            }
            return Ok(await Mediator.Send(command));
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteWhsTransferCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [HttpGet("whstransferstatus")]
        public async Task<IActionResult> GetWhsTransferStatus()
        {
            var req = new GetEnum() { type = typeof(enWhsTransferStatus) };

            return Ok(await Mediator.Send(req));
        }

        /// <summary>
        /// Get whsTrasfer report in PDF format
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("print")]
        public async Task<IActionResult> Print(PrintWhsTransferCommand command)
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

        [HttpPatch("process")]
        public async Task<IActionResult> Process([FromQuery] ProcessWhsTransferCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
