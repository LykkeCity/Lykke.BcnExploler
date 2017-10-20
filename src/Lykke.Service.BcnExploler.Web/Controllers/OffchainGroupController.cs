using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.OffchainNotifcations;
using Lykke.Service.BcnExploler.Web.Models.Offchain;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Controllers
{
    public class OffchainGroupController:Controller
    {
        private readonly IAssetService _assetService;
        private readonly IOffchainNotificationsService _offchainNotificationsService;

        public OffchainGroupController(IAssetService assetService, IOffchainNotificationsService offchainNotificationsService)
        {
            _assetService = assetService;
            _offchainNotificationsService = offchainNotificationsService;
        }

        [Route("offchain/group/offchaintransactionspage")]
        public async Task<ActionResult> OffchainMixedTransactionsPage(string group, int page, int pageSize)
        {
            var getTransactions =
                _offchainNotificationsService.GetMixedTransactionsByGroupAsync(group,
                    PageOptions.Create(page, pageSize));
            var getAssetDictionary = _assetService.GetAssetDefinitionDictionaryAsync();

            await Task.WhenAll(getTransactions, getAssetDictionary);

            return View("Offchain/OffchainMixedTransactionsPage", getTransactions.Result.Select(p => OffchainMixedTransactionViewModel.Create(p, getAssetDictionary.Result)));
        }
    }
}