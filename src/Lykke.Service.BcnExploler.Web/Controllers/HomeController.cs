using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Domain.Block;
using Lykke.Service.BcnExploler.Web.Models.Block;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IBlockService _blockService;

        public HomeController(IBlockService blockService)
        {
            _blockService = blockService;
        }

        public async Task<IActionResult> Index()
        {
            var lastBlock = await _blockService.GetLastBlockHeaderAsync();
            return View(BlockHeaderViewModel.Create(lastBlock));
        }
    }
}
