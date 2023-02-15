using bbxBE.Application.Commands.cmdInvoice;
using bbxBE.Application.Commands.cmdUser;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Queries.qEnum;
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Application.Wrappers;
using bbxBE.Common;
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
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using static bbxBE.Common.NAV.NAV_enums;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
 //   [Authorize]
    public class InvoiceController : BaseApiController
    {

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IHttpContextAccessor _context;
        public InvoiceController(
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
        public async Task<IActionResult> Get([FromQuery] GetInvoice filter)
        {
             return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("getaggregateinvoice")]
        public async Task<IActionResult> GetAggregateInvoice([FromQuery] GetAggregateInvoice filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateInvoiceCommand command)
        {
            if (command.UserID == null || command.UserID == 0)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                command.UserID = Utils.GetUserIDFromClaimsIdentity(identity);
            }
            return Ok(await Mediator.Send(command));
        }



        [HttpGet("paymentmethod")]
        public async Task<IActionResult> GetPaymentMethod()
        {
            var req = new GetEnum() { type = typeof(PaymentMethodType), FilteredItems = new List<string>() { PaymentMethodType.VOUCHER.ToString(), PaymentMethodType.OTHER.ToString()}};

            return Ok(await Mediator.Send(req));
        }

        [HttpGet("pendigdeliverynotessummary")]
        public async Task<IActionResult> Get([FromQuery] GetPendigDeliveryNotesSummary pars)
        {
            return Ok(await Mediator.Send(pars));

        }

        [HttpGet("pendigdeliverynotes")]
        public async Task<IActionResult> Get([FromQuery] GetPendigDeliveryNotes pars)
        {
            return Ok(await Mediator.Send(pars));

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


        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("importfromnav")]
        public async Task<IActionResult> ImportFromNAV(importFromNAVCommand command)
        {
                return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("print")]
        public async Task<IActionResult> Print(PrintInvoiceCommand command)
        {

            command.baseURL = $"{_context.HttpContext.Request.Scheme.ToString()}://{_context.HttpContext.Request.Host.ToString()}";
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(result.FileStream, "application/octet-stream", result.FileDownloadName); // returns a FileStreamResult
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

 

                [HttpGet("currencycode")]
                public async Task<IActionResult> GetCurrencyCode()
                {
                    var req = new GetEnum() {  type = typeof(enCurrencyCodes) };
                    return Ok(await Mediator.Send(req));
                }

                */
    }
}
