using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Web.Models.Asset;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Controllers.api
{
    public class AssetsController : Controller
    {
        private readonly IAssetService _assetService;

        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [HttpGet("api/assets")]
        public async Task<IEnumerable<AssetDirectoryViewModel.Asset>> Get()
        {
            var assetDefinitions = _assetService.GetAssetDefinitionsAsync();
            var assetCoinholdersIndexes = _assetService.GetAssetCoinholdersIndexAsync();
            var assetScores = _assetService.GetAssetScoreDictionaryAsync();

            await Task.WhenAll(assetCoinholdersIndexes, assetDefinitions, assetScores);
            var result = AssetDirectoryViewModel.Create(assetDefinitions.Result, assetCoinholdersIndexes.Result, assetScores.Result);
            return result.Assets;
        }
    }
}
