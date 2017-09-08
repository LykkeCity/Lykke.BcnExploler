using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.AssetBalanceChanges;
using Lykke.Service.BcnExploler.Web.Models.Asset;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Controllers.api
{
    public class AssetsTransactionsController : Controller
    {
        private readonly IAssetService _assetService;
        private readonly IAssetBalanceChangesRepository _balanceChangesRepository;

        public AssetsTransactionsController(IAssetBalanceChangesRepository balanceChangesRepository,
            IAssetService assetService)
        {
            _balanceChangesRepository = balanceChangesRepository;
            _assetService = assetService;
        }

        [HttpGet("api/assetstransactions/{id}")]
        public async Task<IEnumerable<AssetTransactionViewModel>> Get(string id)
        {
            var asset = await _assetService.GetAssetAsync(id);

            if (asset != null)
            {
                var txs = await _balanceChangesRepository.GetTransactionsAsync(asset.AssetIds);

                return txs.Select(AssetTransactionViewModel.Create);
            }

            return Enumerable.Empty<AssetTransactionViewModel>();
        }
    }
}