using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Controllers
{
    public class ErrorController:Controller
    {
        [Route("error/{statusCode}")]
        public ActionResult Handle(string statusCode)
        {
            if (statusCode == "404")
            {
                return View("NotFound");
            }

            return View("Server");
        }
    }
}
