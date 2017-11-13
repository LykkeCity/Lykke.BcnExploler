using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCNExplorer.Web.Models;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.AssetBalanceChanges;
using Lykke.Service.BcnExploler.Core.Block;
using Lykke.Service.BcnExploler.Core.MainChain;
using Lykke.Service.BcnExploler.Services.Helpers;
using Lykke.Service.BcnExploler.Web.Models.Asset;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Controllers
{
    public class AssetController : Controller
    {
        private readonly IAssetService _assetService;
        private readonly IBlockService _blockService;
        private readonly IAssetBalanceChangesRepository _balanceChangesRepository;

        private readonly ICachedMainChainService _mainChainService;

        public AssetController(IAssetService assetService,
            IBlockService blockService,
            ICachedMainChainService mainChainService, 
            IAssetBalanceChangesRepository assetBalanceChangesRepository)
        {
            _assetService = assetService;
            _blockService = blockService;
            _mainChainService = mainChainService;
            _balanceChangesRepository = assetBalanceChangesRepository;
        }

        [Route("asset/{id}")]
        public async Task<ActionResult> Index(string id)
        {
            var result = await _assetService.GetAssetAsync(id);
            if (result != null)
            {
                return View(AssetViewModel.Create(result));
            }

            return View("NotFound");
        }

        [Route("assets")]
        public ActionResult AssetDirectiory()
        {
            return View();
        }

        #region owners

        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [Route("asset/owners/{id}")]
        public Task<ActionResult> Owners(string id)
        {
            return _OwnersInner(id);
        }

        [Route("asset/ownershistory/{id}")]
        public Task<ActionResult> OwnersHistory(string id, [FromQuery]int? at)
        {
            return _OwnersInner(id, at);
        }

        [Route("asset/ownershistorybydata/{id}")]
        public async Task<ActionResult> OwnersHistoryByDate([FromQuery]DateTime at, string id)
        {
            var mainChain = await _mainChainService.GetMainChainAsync();

            var block = mainChain.GetClosestToTimeBlock(at);

            return RedirectToAction("OwnersHistory", new { id = id, at = block?.Height ?? 0 });
        }

        private async Task<ActionResult> _OwnersInner(string id, int? at = null)
        {
            var result = await GetOwnersAsync(id, at);

            if (result != null)
            {
                return View("Owners", result);
            }

            return View("NotFound");
        }

        private async Task<AssetCoinholdersViewModel> GetOwnersAsync(string id, int? at)
        {
            var asset = await _assetService.GetAssetAsync(id);

            if (asset != null)
            {
                var summaryTask = _balanceChangesRepository.GetSummaryAsync(at, asset.AssetIds.ToArray());
                var addressChangesTask = _balanceChangesRepository.GetBlocksWithChanges(asset.AssetIds);
                var lastBlockTask = _blockService.GetLastBlockHeaderAsync();
                Task<IDictionary<string, double>> addressChangesAtBlockTask;
                Task<IBlockHeader> atBlockInfoTask;
                if (at != null)
                {
                    atBlockInfoTask = _blockService.GetBlockHeaderAsync(at.ToString());
                    addressChangesAtBlockTask = _balanceChangesRepository.GetAddressQuantityChangesAtBlock(at.Value, asset.AssetIds.ToArray());
                }
                else
                {
                    atBlockInfoTask = Task.FromResult((IBlockHeader)null);
                    addressChangesAtBlockTask = Task.FromResult((IDictionary<string, double>)new Dictionary<string, double>());
                }

                await Task.WhenAll(addressChangesAtBlockTask, summaryTask, addressChangesTask, lastBlockTask, atBlockInfoTask);

                var result = AssetCoinholdersViewModel.Create(
                    AssetViewModel.Create(asset),
                    summaryTask.Result,
                    at,
                    addressChangesAtBlockTask.Result,
                    addressChangesTask.Result,
                    lastBlockTask.Result,
                    atBlockInfoTask.Result
                    );

                return result;
            }

            return null;
        }

        [Route("asset/transactions/{id}")]
        public async Task<ActionResult> Transactions(string id)
        {
            var asset = await _assetService.GetAssetAsync(id);

            if (asset != null)
            {
                var txs = await _balanceChangesRepository.GetTransactionsAsync(asset.AssetIds);

                var txList = new TransactionIdList(txs.Select(p => p.Hash));

                return View("~/Views/Transaction/TransactionIdList.cshtml", txList);
            }


            return View("NotFound");
        }

        [Route("asset/ownerstocsv/{id}")]
        public async Task<ActionResult> OwnersToCsv(string id, [FromQuery]int at)
        {
            var result = await GetOwnersAsync(id, at);

            if (result != null)
            {
                return File(await result.ToCsvAsync(), "text/csv", $"Coinholders-{result.Asset.NameShort}-{at}.csv");
            }

            return View("NotFound");
        }

        #endregion
    }
}
