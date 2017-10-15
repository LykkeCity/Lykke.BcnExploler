using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Channel;
using Lykke.Service.BcnExploler.Web.Models.Transaction;

namespace Lykke.Service.BcnExploler.Web.Models.Offchain
{
    public class OffchainMixedTransactionViewModel
    {
        public OffChainTransactionViewModel Offchain { get; set; }

        public bool ShowOffchain => Offchain != null;

        public TransactionViewModel Onchain { get; set; }

        public bool ShowOnchain => Onchain != null;

        public static OffchainMixedTransactionViewModel Create(IFilledMixedTransaction source,
            IReadOnlyDictionary<string, IAssetDefinition> assetDictionary)
        {
            return new OffchainMixedTransactionViewModel
            {
                Onchain = TransactionViewModel.Create(source.FilledOnchainTransactionData, assetDictionary),
                Offchain = OffChainTransactionViewModel.Create(source.OffchainTransactionData, assetDictionary)
            };
        }
    }
}
