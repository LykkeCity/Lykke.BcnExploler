using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Block;
using Lykke.Service.BcnExploler.Core.Search;
using Lykke.Service.BcnExploler.Web.Models.Block;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;
        private readonly IBlockService _blockService;
        private readonly IAssetService _assetService;

        public SearchController(ISearchService searchService, 
            IBlockService blockService, 
            IAssetService assetService)
        {
            _searchService = searchService;
            _blockService = blockService;
            _assetService = assetService;
        }

        [Route("search")]
        public async Task<ActionResult> Search([FromQuery]string id)
        {
            id = (id ?? "").Trim();
            var type = await _searchService.GetTypeAsync(id);
            switch (type)
            {
                case SearchResultType.Block:
                {
                    return RedirectToAction("Index", "Block", new {id = id});
                }
                case SearchResultType.Transaction:
                {
                    return RedirectToAction("Index", "Transaction", new { id = id });
                }
                case SearchResultType.Address:
                {
                    return RedirectToAction("Index", "Address", new { id = id });
                }
                case SearchResultType.Asset:
                {
                    var asset = await _assetService.GetAssetAsync(id);
                    return RedirectToAction("Index", "Asset", new { id = asset.AssetIds.First() });
                }
                case SearchResultType.OffchainTransaction:
                {
                    return RedirectToAction("Index", "OffchainTransaction", new { id = id });
                }
                default:
                {
                    var lastBlock = await _blockService.GetLastBlockHeaderAsync();
                    return View("NotFound", BlockHeaderViewModel.Create(lastBlock));
                }
            }
        }
    }
}