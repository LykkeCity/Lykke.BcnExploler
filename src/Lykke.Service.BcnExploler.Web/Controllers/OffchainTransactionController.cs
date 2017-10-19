using System;
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
        private readonly IOffchainNotificationsService _offchainNotificationsService;

        public OffchainTransactionController(ICachedTransactionService transactionService,
            IAssetService assetService,
            IOffchainNotificationsService offchainNotificationsService)
        {
            _assetService = assetService;
            _offchainNotificationsService = offchainNotificationsService;
        }

        [Route("transaction/offchain/{id}")]
        public async Task<ActionResult> Index(string id)
        {
            var getOffchainTx = _offchainNotificationsService.GetOffchainMixedTransaction(id);
            var getAssetDictionary = _assetService.GetAssetDefinitionDictionaryAsync();

            await Task.WhenAll(getOffchainTx, getAssetDictionary);


            if (getOffchainTx.Result == null)
            {
                return View("NotFound");
            }
            

            var offchainTransactionCount =  await _offchainNotificationsService.GetTransactionCountByGroupAsync(getOffchainTx.Result.GroupId);

            return View(OffchainTransactionDetailsViewModel.Create(getOffchainTx.Result, 
                getAssetDictionary.Result,
                offchainTransactionCount));
        }
    }
}