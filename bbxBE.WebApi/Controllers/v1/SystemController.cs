using bbxBE.Application.Commands.cmdCustomer;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.qEnum;
using bbxBE.Application.Queries.qZip;
using bbxBE.Common.Enums;
using bbxBE.Common.ExpiringData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class SystemController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IProductRepositoryAsync _productRepositoryAsync;
        private readonly IExpiringData<ExpiringDataObject> _expiringData;
        public SystemController(IWebHostEnvironment env, IConfiguration conf,
            IProductRepositoryAsync productRepositoryAsync,
             IExpiringData<ExpiringDataObject> expiringData)
        {
            _env = env;
            _conf = conf;
            _productRepositoryAsync = productRepositoryAsync;
            _expiringData = expiringData;
        }


        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("refreshcaches")]
        public async Task<IActionResult> RefreshCaches()
        {
            // https://docs.microsoft.com/en-us/answers/questions/481854/load-controller-at-startup-web-api-core.html

            await _productRepositoryAsync.RefreshProductCache();
            return Ok(true);
        }


        //   [Authorize]
        [HttpGet("currencycodes")]
        public async Task<IActionResult> GetCurrencyCodes()
        {


            var req = new GetEnum() { type = typeof(enCurrencyCodes) };
            return Ok(await Mediator.Send(req));
        }

        [HttpGet("invoicetypes")]
        public async Task<IActionResult> GetInvoiceTypes()
        {

            var req = new GetEnum() { type = typeof(enInvoiceType) };
            return Ok(await Mediator.Send(req));
        }

        [HttpGet("userlevels")]
        public async Task<IActionResult> GetUserLevels()
        {

            var req = new GetEnum() { type = typeof(enUserLevel) };
            return Ok(await Mediator.Send(req));
        }

        //   [Authorize]
        [HttpGet("exchangerate")]
        public async Task<IActionResult> GetExchangeRate([FromQuery] DateTime ExchengeRateDate, string Currency)
        {
            if (Currency == enCurrencyCodes.HUF.ToString())
            {
                return Ok(1);
            }

            decimal ExchangeRate = -1;

            ServiceReference1.MNBArfolyamServiceSoapClient sm = new ServiceReference1.MNBArfolyamServiceSoapClient();
            var body = new ServiceReference1.GetExchangeRatesRequestBody();
            body.currencyNames = Currency.ToUpper();

            bool bCompleted = false;
            DateTime CurrExchengeRateDate = ExchengeRateDate;
            while (!bCompleted)
            {
                body.startDate = CurrExchengeRateDate.ToString("yyyy-MM-dd");
                body.endDate = CurrExchengeRateDate.ToString("yyyy-MM-dd");
                try
                {
                    var res = await sm.GetExchangeRatesAsync(body);
                    string xml = res.GetExchangeRatesResponse1.GetExchangeRatesResult;
                    if (!string.IsNullOrWhiteSpace(xml))
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml);

                        if ((xmlDoc.ChildNodes.Count > 0) && (xmlDoc.ChildNodes[0].ChildNodes.Count > 0))
                        {
                            foreach (XmlNode oNode in xmlDoc.ChildNodes[0].ChildNodes[0].ChildNodes)
                            {
                                if (oNode.Attributes["curr"].Value.ToUpper() == Currency.ToUpper())
                                {
                                    try
                                    {
                                        ExchangeRate = Convert.ToDecimal(oNode.InnerText.Replace(",", "."));
                                    }
                                    catch
                                    {
                                        ExchangeRate = -1;
                                    }
                                    bCompleted = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            CurrExchengeRateDate = CurrExchengeRateDate.AddDays(-1);
                        }
                    }
                }
                catch
                {
                    ExchangeRate = -1;
                    bCompleted = true;
                }
            }
            return Ok(ExchangeRate);
        }

        [HttpGet("citybyzip")]
        public async Task<IActionResult> GetCityByZipCode([FromQuery] GetCityByZip req)
        {
            return Ok(await Mediator.Send(req));
        }

        [HttpGet("zipbycity")]
        public async Task<IActionResult> GetZipcodeByCity([FromQuery] GetZipByCity req)
        {
            return Ok(await Mediator.Send(req));
        }

        [HttpDelete("purgeexpiringdata")]
        public async Task<IActionResult> PurgeExpiringData()
        {
            _expiringData.Purge();
            return Ok(true);
        }

        [HttpPost("addorupdateexpiringdata")]
        public async Task<IActionResult> AddExpiringData([FromQuery] string Key, [FromQuery] string Data, [FromQuery] int LifetimeSec)
        {
            var SessionID = HttpContext.Session.Id;
            var Lifetime = TimeSpan.FromSeconds(LifetimeSec);
            var ret = await _expiringData.AddOrUpdateItemAsync(Key, Data, SessionID, Lifetime);
            return Ok(ret);
        }

        [HttpGet("getexpiringdata")]
        public async Task<IActionResult> GetExpiringData([FromQuery] string key)
        {
            var item = _expiringData.GetItem(key, false);
            return Ok(item);
        }


        [HttpGet("listexpiringdata")]
        public async Task<IActionResult> ListExpiringData()
        {
            var item = _expiringData.List();
            return Ok(item);
        }


        [HttpPost("unlockallcustomers")]
        public async Task<IActionResult> UnlockAllCustomers(UnlockAllCustomersCommand req)
        {
            return Ok(await Mediator.Send(req));
        }
    }
}
