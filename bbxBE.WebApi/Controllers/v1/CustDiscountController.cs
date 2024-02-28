﻿using bbxBE.Application.Queries.qCustDiscount;
using bxBE.Application.Commands.cmdCustDiscount;
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

    public class CustDiscountController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        public CustDiscountController(IWebHostEnvironment env, IConfiguration conf)
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
        public async Task<IActionResult> Get([FromQuery] GetCustDiscount filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("discountforcustomer")]
        public async Task<IActionResult> Query([FromQuery] GetCustDiscountForCustomerList filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateCustDiscountCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

    }
}
