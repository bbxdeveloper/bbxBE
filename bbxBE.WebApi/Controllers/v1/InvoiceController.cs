using bbxBE.Application.Commands.cmdImport;
using bbxBE.Application.Commands.cmdInvoice;
using bbxBE.Application.Commands.cmdNAV;
using bbxBE.Application.Queries.qCustomer;
using bbxBE.Application.Queries.qEnum;
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Common;
using bbxBE.Common.NAV;
using bxBE.Application.Commands.cmdInvoice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    //[AllowAnonymous]
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
        public async Task<IActionResult> Get([FromQuery] GetInvoice request)
        {
            return Ok(await Mediator.Send(request));
        }

        [HttpGet("byinvoicenumber")]
        public async Task<IActionResult> GetInvoiceByInvoiceNumber([FromQuery] GetInvoiceByInvoiceNumber request)
        {
            return Ok(await Mediator.Send(request));
        }


        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("aggregateinvoice")]
        public async Task<IActionResult> GetAggregateInvoice([FromQuery] GetAggregateInvoice request)
        {
            return Ok(await Mediator.Send(request));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("aggregateinvoicedeliverynote")]
        public async Task<IActionResult> GetAggregateInvoiceDeliveryNote([FromQuery] GetAggregateInvoiceDeliveryNote request)
        {
            return Ok(await Mediator.Send(request));
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

        [HttpPatch("pricepreview")]
        public async Task<IActionResult> UpdatePriceReview(UpdatePricePreviewCommand command)
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
            var req = new GetEnum() { type = typeof(PaymentMethodType), FilteredItems = new List<string>() { PaymentMethodType.VOUCHER.ToString(), PaymentMethodType.OTHER.ToString() } };

            return Ok(await Mediator.Send(req));
        }

        [HttpGet("pendigdeliverynotessummary")]
        public async Task<IActionResult> Get([FromQuery] GetPendigDeliveryNotesSummary pars)
        {
            return Ok(await Mediator.Send(pars));

        }

        [HttpGet("pendigdeliverynotesitems")]
        public async Task<IActionResult> Get([FromQuery] GetPendigDeliveryNotesItems pars)
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
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("queryunpaid")]
        public async Task<IActionResult> QueryUnpaid([FromQuery] QueryUnpaidInvoice filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("querycustomerinvoicesummary")]
        public async Task<IActionResult> QueryCustomerInvoiceSummary([FromQuery] QueryCustomerInvoiceSummary req)
        {
            return Ok(await Mediator.Send(req));
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
            command.JWT = Utils.getJWT(_context.HttpContext);
            command.baseURL = $"{_context.HttpContext.Request.Scheme.ToString()}://{_context.HttpContext.Request.Host.ToString()}";
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(result.FileStream, "application/octet-stream", result.FileDownloadName); // returns a FileStreamResult
        }


        [HttpGet("csv")]
        public async Task<IActionResult> CSV([FromQuery] CSVInvoice command)
        {
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(result.FileStream, "application/octet-stream", result.FileDownloadName); // returns a FileStreamResult
        }

        [HttpGet("customerunpaidamount")]
        public async Task<IActionResult> GetCustomerUnpaidAmount([FromQuery] GetCustomerUnpaidAmount req)
        {
            return Ok(await Mediator.Send(req));
        }


        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("printcustomerinvoicesummary")]
        public async Task<IActionResult> PrintCustomerInvoiceSummary(PrintCustomerInvoiceSummaryCommand command)
        {

            command.JWT = Utils.getJWT(_context.HttpContext);
            command.baseURL = $"{_context.HttpContext.Request.Scheme.ToString()}://{_context.HttpContext.Request.Host.ToString()}";
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(result.FileStream, "application/octet-stream", result.FileDownloadName); // returns a FileStreamResult
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import([FromQuery] ImportInvoiceCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

    }
}
