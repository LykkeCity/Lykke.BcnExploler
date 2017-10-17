using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Channel;
using Lykke.Service.BcnExploler.Core.Transaction;
using Lykke.Service.BcnExploler.Web.Models.Offchain;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Controllers
{
    public class OffchainTransactionController: Controller
    {
        private readonly IAssetService _assetService;
        private readonly IChannelService _channelService;

        public OffchainTransactionController(ICachedTransactionService transactionService,
            IAssetService assetService,
            IChannelService channelService)
        {
            _assetService = assetService;
            _channelService = channelService;
        }

        [Route("transaction/offchain/{id}")]
        public async Task<ActionResult> Index(string id)
        {
            var channel = _channelService.GetChannelsByOffchainTransactionIdAsync(id);
            var assetDictionary = _assetService.GetAssetDefinitionDictionaryAsync();

            await Task.WhenAll(channel, assetDictionary);


            if (channel.Result == null)
            {
                return View("NotFound");
            }

            var offchainTransactionCount =  await _channelService.GetTrabsactionCountByGroupAsync(channel.Result.GroupId);

            var channelViewModel = OffchainFilledChannelViewModel.Create(channel.Result, assetDictionary.Result);

            return View(OffchainTransactionDetailsViewModel.Create(channelViewModel, id, channel.Result.GroupId, offchainTransactionCount));
        }
    }
}