using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BCNExplorer.Web.Models;
using Common;
using Core.Transaction;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Transaction;
using Lykke.Service.BcnExploler.Web.Models.Transaction;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lykke.Service.BcnExploler.Web.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IAssetService _assetService;
        private readonly ICachedTransactionService _cachedTransactionService;

        public TransactionController(ITransactionService transactionService, IAssetService assetService, ICachedTransactionService cachedTransactionService)
        {
            _transactionService = transactionService;
            _assetService = assetService;
            _cachedTransactionService = cachedTransactionService;
        }

        [Route("transaction/{id}")]
        public async Task<ActionResult> Index(string id)
        {
            var ninjaTransaction = await _transactionService.GetAsync(id);

            if (ninjaTransaction != null)
            {
                var result = TransactionViewModel.Create(ninjaTransaction, await _assetService.GetAssetDefinitionDictionaryAsync());
 
                return View(result);
            }

            return View("NotFound");
        }

        [Route("transation/list")]
        public async Task<ActionResult> List(IList<string> ids)
        {
            var assetDictionary = _assetService.GetAssetDefinitionDictionaryAsync();

            var loadTransactionTask = _cachedTransactionService.GetAsync(ids); 

            await Task.WhenAll(loadTransactionTask, assetDictionary);

            var mappedTxs = await loadTransactionTask.Result.SelectAsync(p =>
                Task.Run(() => TransactionViewModel.Create(p, assetDictionary.Result)));

            return View(mappedTxs.ToList().OrderBy(p => ids.IndexOf(p.TransactionId)));
        }
    }
}
