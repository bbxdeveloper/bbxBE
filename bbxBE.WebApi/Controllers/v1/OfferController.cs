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
        public OfferController(
           IWebHostEnvironment env,
           IConfiguration conf)
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
        public async Task<IActionResult> Update(UpdateOfferCommand2 command)
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


    }
}
