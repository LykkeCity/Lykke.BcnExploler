using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Block;
using Lykke.Service.BcnExploler.Web.Models.Block;
using Lykke.Service.BcnExploler.Web.Models.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Controllers
{
    public class BlockController : Controller
    {
        private readonly IBlockService _blockService;
        private readonly ICachedBlockService _cachedBlockService;


        public BlockController(IBlockService blockService,
            ICachedBlockService cachedBlockService,
            IAssetService assetService)
        {
            _blockService = blockService;
            _cachedBlockService = cachedBlockService;
        }

        [Route("block/{id}")]
        public async Task<ActionResult> Index(string id)
        {
            int numBlockHeight;

            if (int.TryParse(id, out numBlockHeight))
            {
                var header = await _blockService.GetBlockHeaderAsync(numBlockHeight.ToString());
                if (header != null)
                {
                    return RedirectToAction("Index", "Block", new { id = header.Hash });
                }
            }

            var block = _cachedBlockService.GetBlockAsync(id);

            var lastBlock = _blockService.GetLastBlockHeaderAsync();

            await Task.WhenAll(block, lastBlock);

            if (block.Result != null)
            {
                var result = BlockViewModel.Create(block.Result, lastBlock.Result);

                return View(result);
            }

            return View("NotFound");
        }
    }
}
