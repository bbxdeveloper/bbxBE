using bbxBE.Application.Commands.cmdWhsTransfer;
using bbxBE.Application.Queries.qWhsTransfer;
using bbxBE.Common;
using bxBE.Application.Commands.cmdWarehouse;
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
    public class WhsTransferController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IHttpContextAccessor _context;
        public WhsTransferController(IWebHostEnvironment env, IConfiguration conf, IHttpContextAccessor context)
        {
            _env = env;
            _conf = conf;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetWhsTransfer request)
        {
            return Ok(await Mediator.Send(request));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
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

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteWhsTransferCommand request)
        {
            return Ok(await Mediator.Send(request));
        }
    }
}
