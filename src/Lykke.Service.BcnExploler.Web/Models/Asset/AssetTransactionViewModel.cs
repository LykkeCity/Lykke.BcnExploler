using Lykke.Service.BcnExploler.Core.AssetBalanceChanges;

namespace Lykke.Service.BcnExploler.Web.Models.Asset
{
    public class AssetTransactionViewModel
    {
        public string TransactionHash { get; set; }

        public static AssetTransactionViewModel Create(IBalanceTransaction source)
        {
            return new AssetTransactionViewModel
            {
                TransactionHash = source.Hash
            };
        }
    }
}