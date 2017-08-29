using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {

        public async Task<IActionResult> Index()
        {
            return View();
        }

    }
}
