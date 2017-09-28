using Lykke.Service.BcnExploler.Web.Models.Asset;

namespace Lykke.Service.BcnExploler.Web.Models.Offchain
{
    public class OffchainInOutViewModel
    {
        public string Address { get; set; }

        public decimal Quantity { get; set; }

        public decimal QuantityDiff { get; set; }

        public AssetViewModel Asset { get; set; }

        public static OffchainInOutViewModel Create(string address, decimal quantity, decimal quantityDiff, AssetViewModel asset)
        {
            return new OffchainInOutViewModel
            {
                Address = address,
                Quantity = quantity,
                QuantityDiff = quantityDiff,
                Asset = asset
            };
        }
    }
}
