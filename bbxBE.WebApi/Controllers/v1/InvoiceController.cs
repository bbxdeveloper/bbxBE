using bbxBE.Application.Commands.cmdUSR_USER;
using bbxBE.Application.Enums;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Queries.qEnum;
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
    public class InvoiceController : BaseApiController
    {
        /*
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IRequestHandler<CreateInvoiceCommand, Response<Invoice>> _InvoiceCommandHandler;
        public InvoiceController(
           IWebHostEnvironment env,
           IConfiguration conf,
           IRequestHandler<CreateInvoiceCommand, Response<Invoice>> InvoiceCommandHandler)
        {
            _env = env;
            _conf = conf;
            _InvoiceCommandHandler = InvoiceCommandHandler;
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

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateInvoiceCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        */
    }
}
