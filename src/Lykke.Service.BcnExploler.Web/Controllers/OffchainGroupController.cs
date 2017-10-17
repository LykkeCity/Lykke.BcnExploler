using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Channel;
using Lykke.Service.BcnExploler.Web.Models.Offchain;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Controllers
{
    public class OffchainGroupController:Controller
    {
        private readonly IAssetService _assetService;
        private readonly IChannelService _channelService;

        public OffchainGroupController(IAssetService assetService, IChannelService channelService)
        {
            _assetService = assetService;
            _channelService = channelService;
        }

        [Route("offchain/group/offchaintransactionspage")]
        public async Task<ActionResult> OffchainMixedTransactionsPage(string group, int page, int pageSize)
        {
            var getTransactions =
                _channelService.GetMixedTransactionsByGroupAsync(group,
                    PageOptions.Create(page, pageSize));
            var getAssetDictionary = _assetService.GetAssetDefinitionDictionaryAsync();

            await Task.WhenAll(getTransactions, getAssetDictionary);

            return View("Offchain/OffchainMixedTransactionsPage", getTransactions.Result.Select(p => OffchainMixedTransactionViewModel.Create(p, getAssetDictionary.Result)));
        }
    }
}