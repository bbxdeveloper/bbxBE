using bbxBE.Application.Commands.cmdUser;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Queries.qStock;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
 //   [Authorize]
    public class StockController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        public StockController( IWebHostEnvironment env, IConfiguration conf)
        {
            _env = env;
            _conf = conf;
        }


        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetStock filter)
        {
            return Ok(await Mediator.Send(filter));
        }


        [HttpGet("record")]
        public async Task<IActionResult> GetRecord([FromQuery] GetStockRecord req)
        {
            return Ok(await Mediator.Send(req));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QueryStock filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        [HttpGet("queryinvctrlabsent")]
        public async Task<IActionResult> QueryInvCtrlStockAbsent([FromQuery] QueryInvCtrlStockAbsent filter)
        {
            return Ok(await Mediator.Send(filter));
        }
    }
}
