using bbxBE.Application.Commands.cmdUSR_USER;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Queries.qEnum;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdInvoice;
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
    public class InvoiceController : BaseApiController
    {

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        public InvoiceController(
           IWebHostEnvironment env,
           IConfiguration conf)
        {
            _env = env;
            _conf = conf;
        }


        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(createOutgoingInvoiceCommand command)
        {
            return Ok(await Mediator.Send(command));
        }



        [HttpGet("paymentmethod")]
        public async Task<IActionResult> GetPaymentMethod()
        {
            var req = new GetEnum() { type = typeof(PaymentMethodType) };
            return Ok(await Mediator.Send(req));
        }

        /*

                /// <summary>
                /// GET: api/controller
                /// </summary>
                /// <param name="filter"></param>
                /// <returns></returns>
                [HttpGet]
                public async Task<IActionResult> Get([FromQuery] GetInvoice filter)
                {
                    return Ok(await Mediator.Send(filter));
                }

                /// <summary>
                /// GET: api/controller
                /// </summary>
                /// <param name="filter"></param>
                /// <returns></returns>
                [HttpGet("query")]
                public async Task<IActionResult> Query([FromQuery] QueryInvoice filter)
                {
                    return Ok(await Mediator.Send(filter));
                }


                [HttpGet("currencycode")]
                public async Task<IActionResult> GetCurrencyCode()
                {
                    var req = new GetEnum() {  type = typeof(enCurrencyCodes) };
                    return Ok(await Mediator.Send(req));
                }

                */
    }
}
