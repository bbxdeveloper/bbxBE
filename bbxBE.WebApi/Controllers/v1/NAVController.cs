using bbxBE.Application.Commands.cmdNAV;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Common.ExpiringData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    //   [Authorize]
    public class NAVController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IProductRepositoryAsync _productRepositoryAsync;
        private readonly IExpiringData<ExpiringDataObject> _expiringData;
        public NAVController(IWebHostEnvironment env, IConfiguration conf,
            IProductRepositoryAsync productRepositoryAsync,
             IExpiringData<ExpiringDataObject> expiringData)
        {
            _env = env;
            _conf = conf;
            _productRepositoryAsync = productRepositoryAsync;
            _expiringData = expiringData;
        }


        //   [Authorize]
        [HttpGet("getxml")]
        public async Task<IActionResult> GetXML([FromQuery] GetInvoiceNAVXML request)
        {
            var result = await Mediator.Send(request);

            if (result == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(result.FileStream, "application/octet-stream", result.FileDownloadName); // returns a FileStreamResult
        }



        [HttpPost("sendinvoicetonav")]
        public async Task<IActionResult> SendInvoiceToNAV([FromQuery] sendInvoiceToNAVCommand request)
        {
            return Ok(await Mediator.Send(request));
        }


        [HttpGet("sendannulmenttonav")]
        public async Task<IActionResult> SendAnnulmentNAV([FromQuery] sendAnnulmentNAVCommand request)
        {
            return Ok(await Mediator.Send(request));
        }


        [HttpGet("querytransactionstatus")]
        public async Task<IActionResult> QueryTransactionStatusNAV([FromQuery] queryTransactionStatusNAVCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        /**************/


        [HttpGet("callmanageinvoice")]
        public async Task<IActionResult> callManageInvoiceNAV([FromQuery] callManageInvoiceNAVCommand request)
        {
            return Ok(await Mediator.Send(request));
        }


        [HttpGet("callmanageannulment")]
        public async Task<IActionResult> CallManageAnnulmentNAV([FromQuery] callManageAnnulmentNAVCommand request)
        {
            return Ok(await Mediator.Send(request));
        }


    }
}
