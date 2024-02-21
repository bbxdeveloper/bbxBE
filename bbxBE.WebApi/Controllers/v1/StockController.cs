using bbxBE.Application.Queries.qStock;
using bxBE.Application.Commands.cmdLocation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    public class StockController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        public StockController(IWebHostEnvironment env, IConfiguration conf)
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

        [HttpGet("productstocks")]
        public async Task<IActionResult> GetProductStocks([FromQuery] GetProductStocks req)
        {
            return Ok(await Mediator.Send(req));
        }

        [HttpGet("productstockrecords")]
        public async Task<IActionResult> GetProductStocksRecord([FromQuery] GetProductStocksRecord req)
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

        [HttpPut("updatelocation")]
        public async Task<IActionResult> UpdateLocation(UpdateStockLocationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
