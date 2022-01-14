using bbxBE.Commands.USR_USER;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using static bbxBE.Commands.USR_USER.createUSR_USERCommand;

namespace bbxBE.WebApi.Controllers.v1
{
    public class USRController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly createUSR_USERCommandHandler _USRCommandHandler;
        public USRController(
           IWebHostEnvironment env,
           IConfiguration conf,
           createUSR_USERCommandHandler USRCommandHandler)
        {
            _env = env;
            _conf = conf;
            _USRCommandHandler = USRCommandHandler;
    }
   
        // GET: USRController/Details/5
        public ActionResult Details(int id)
        {
            return Ok();
        }

        // GET: USRController/Create
        public ActionResult Create()
        {

            return Ok();
        }

        // POST: USRController/Create
        [HttpPost]
        public async Task<ActionResult> Create(createUSR_USERCommand req )
        {
            
            return Ok(await Mediator.Send(_USRCommandHandler.Handle(req, default)));

        }

        // GET: USRController/Edit/5
        public ActionResult Edit(int id)
        {
            return Ok();
        }

        // POST: USRController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            return Ok();
        }

        // GET: USRController/Delete/5
        public ActionResult Delete(int id)
        {
            return Ok();
        }

        // POST: USRController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            return Ok();
        }
    }
}
