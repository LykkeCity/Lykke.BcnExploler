using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCNExplorer.Web.Models;
using Core.Transaction;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Web.Models.Transaction;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lykke.Service.BcnExploler.Web.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IAssetService _assetService;

        public TransactionController(ITransactionService transactionService, IAssetService assetService)
        {
            _transactionService = transactionService;
            _assetService = assetService;
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
    }
}
