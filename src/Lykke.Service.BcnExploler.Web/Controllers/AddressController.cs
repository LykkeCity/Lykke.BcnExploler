using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Address;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Block;
using Lykke.Service.BcnExploler.Core.Channel;
using Lykke.Service.BcnExploler.Core.MainChain;
using Lykke.Service.BcnExploler.Services.Helpers;
using Lykke.Service.BcnExploler.Web.Models.Address;
using Lykke.Service.BcnExploler.Web.Models.Offchain;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Controllers
{
    public class AddressController:Controller
    {
        private readonly IAddressService _addressProvider;
        private readonly IAssetService _assetService;
        private readonly IBlockService _blockService;
        private readonly ICachedAddressService _cachedAddressService;
        private readonly ICachedMainChainService _mainChainService;
        private readonly IChannelService _channelService;
        
        public AddressController(IAddressService addressProvider, 
            IAssetService assetService, 
            IBlockService blockService,
            ICachedMainChainService mainChainService, 
            ICachedAddressService cachedAddressService, 
            IChannelService channelService)
        {
            _addressProvider = addressProvider;
            _assetService = assetService;
            _blockService = blockService;
            _mainChainService = mainChainService;
            _cachedAddressService = cachedAddressService;
            _channelService = channelService;
        }
        
        [Route("address/{id}")]
        public async Task<ActionResult> Index(string id)
        {
            var mainInfo = _addressProvider.GetMainInfoAsync(id);
            var isOffchainHub = _channelService.IsHubAsync(id);

            await Task.WhenAll(mainInfo, isOffchainHub);

            if (mainInfo.Result != null)
            {
                return View(AddressMainInfoViewModel.Create(mainInfo.Result, isOffchainHub.Result));
            }

            return View("NotFound");
        }

        [Route("address/balance/{id}")]
        public Task<ActionResult> Balance(string id)
        {
            return BalanceAtBlockInner(id, null);
        }

        [Route("address/balance/{id}/block")]
        public Task<ActionResult> BalanceAtBlock(string id, [FromQuery] int? at)
        {
            return BalanceAtBlockInner(id, at);
        }

        private async Task<ActionResult> BalanceAtBlockInner(string id, int? at)
        {
            var balance = _addressProvider.GetBalanceAsync(id, at);
            var assetDefinitionDictionary = _assetService.GetAssetDefinitionDictionaryAsync();
            var lastBlock = _blockService.GetLastBlockHeaderAsync();
            var offchainChannels = _channelService.GetChannelsByAddressAsync(id, ChannelStatusQueryType.OpenOnly);
            Task<IBlockHeader> atBlockTask;

            if (at != null)
            {
                atBlockTask = _blockService.GetBlockHeaderAsync(at.ToString());
            }
            else
            {
                atBlockTask = Task.FromResult<IBlockHeader>(null);
            }

            await Task.WhenAll(balance, assetDefinitionDictionary, lastBlock, atBlockTask, offchainChannels);

            if (balance.Result != null)
            {
                return View("Balance" ,AddressBalanceViewModel.Create(balance.Result,
                    assetDefinitionDictionary.Result,
                    lastBlock.Result,
                    atBlockTask.Result,
                    offchainChannels.Result));
            }

            if (at != null && atBlockTask.Result == null)
            {
                return RedirectToAction("BalanceAtBlock", new {id = id, at = lastBlock.Result});
            }

            return NotFound();
        }

        [Route("address/balance/{id}/time/")]
        public async Task<ActionResult> BalanceAtTime([FromQuery]DateTime at, string id)
        {
            var mainChain = await _mainChainService.GetMainChainAsync();

            var block = mainChain.GetClosestToTimeBlock(at);

            return RedirectToAction("BalanceAtBlock", new { id = id, at = block?.Height ?? 0 });
        }

        
        [Route("address/transactions/{id}")]
        public async Task<ActionResult> Transactions(string id)
        {
            var onchainTransactions = _cachedAddressService.GetTransactions(id);
            var offchainTransactionCount = _channelService.GetTrabsactionCountByAddressAsync(id);

            await Task.WhenAll(onchainTransactions, offchainTransactionCount);

            return View(AddressTransactionsViewModel.Create(id, onchainTransactions.Result, offchainTransactionCount.Result));
        }

        [Route("address/offchaintransactionspage")]
        public async Task<ActionResult> OffchainMixedTransactionsPage(string address, int page, int pageSize)
        {
            var getTransactions =
                _channelService.GetMixedTransactionsByAddressAsync(address,
                    PageOptions.Create(page, pageSize));
            var getAssetDictionary = _assetService.GetAssetDefinitionDictionaryAsync();

            await Task.WhenAll(getTransactions, getAssetDictionary);

            return View("Offchain/OffchainMixedTransactionsPage", getTransactions.Result.Select(p => OffchainMixedTransactionViewModel.Create(p, getAssetDictionary.Result)));
        }
    }
}