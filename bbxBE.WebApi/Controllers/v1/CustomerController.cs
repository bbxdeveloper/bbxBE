using bbxBE.Application.Commands.cmdCustomer;
using bbxBE.Application.Commands.cmdUSR_USER;
using bbxBE.Application.Enums;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Queries.qCustomer;
using bbxBE.Application.Queries.qEnum;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdCustomer;
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
    public class CustomerController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IRequestHandler<CreateCustomerCommand, Response<Customer>> _customerCommandHandler;
        public CustomerController(
           IWebHostEnvironment env,
           IConfiguration conf,
           IRequestHandler<CreateCustomerCommand, Response<Customer>> customerCommandHandler)
        {
            _env = env;
            _conf = conf;
            _customerCommandHandler = customerCommandHandler;
    }


        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetCustomer filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QueryCustomer filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        // POST: USRController/Edit/5
        [HttpPut]
 //       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateCustomerCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // GET: USRController/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteCustomerCommand command)
        {
            return Ok(await Mediator.Send(command));
        }



        [HttpGet("countrycode")]
        public async Task<IActionResult> GetCountryCode()
        {
            var req = new GetEnum() { type = typeof(enCountries) };
            return Ok(await Mediator.Send(req));
        }

    }
}
