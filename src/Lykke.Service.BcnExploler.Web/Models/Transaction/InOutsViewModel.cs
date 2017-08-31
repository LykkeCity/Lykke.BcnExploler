using System.Collections.Generic;

namespace Lykke.Service.BcnExploler.Web.Models.Transaction
{
    public class InOutsViewModel
    {
        public IInOutViewModel InOut { get; set; }
        public string AggregatedTransContainerId { get; set; }

        public static InOutsViewModel Create(IInOutViewModel inOutViewModel, string aggregatedTransContainerId = null)
        {
            return new InOutsViewModel
            {
                InOut = inOutViewModel,
                AggregatedTransContainerId = aggregatedTransContainerId
            };
        }
    }

    public class BtcInOutsViewModel
    {
        public IEnumerable<AggregatedInOut<AssetInOutBase>> AggregatedIns { get; set; }
        public IEnumerable<AggregatedInOut<AssetInOutBase>> AggregatedOuts { get; set; }
        public bool ShowConsumedReleased { get; set; }
        public TransactionViewModel.BitcoinAsset Bitcoin { get; set; }

        public static BtcInOutsViewModel CreateWithChange(TransactionViewModel.BitcoinAsset bitcoin,
            bool showConsumedReleased)
        {
            return new BtcInOutsViewModel
            {
                AggregatedIns = bitcoin.AggregatedIns,
                AggregatedOuts = bitcoin.AggregatedOuts,
                ShowConsumedReleased = showConsumedReleased,
                Bitcoin = bitcoin
            };
        }

        public static BtcInOutsViewModel CreateWithoutChange(TransactionViewModel.BitcoinAsset bitcoin,
            bool showConsumedReleased)
        {
            return new BtcInOutsViewModel
            {
                AggregatedIns = bitcoin.AggregatedInsWithoutChange,
                AggregatedOuts = bitcoin.AggregatedOutsWithoutChange,
                ShowConsumedReleased = showConsumedReleased,
                Bitcoin = bitcoin
            };
        }
    }

    public class ColoredAssetInOutsViewModel
    {
        public TransactionViewModel.ColoredAsset ColoredAsset { get; set; }

        public IEnumerable<AggregatedInOut<TransactionViewModel.ColoredAsset.In>> AggregatedIns { get; set; }
        public IEnumerable<AggregatedInOut<TransactionViewModel.ColoredAsset.Out>> AggregatedOuts { get; set; }

        public static ColoredAssetInOutsViewModel CreateWithChange(TransactionViewModel.ColoredAsset coloredAsset)
        {
            return new ColoredAssetInOutsViewModel
            {
                AggregatedIns = coloredAsset.AggregatedIns,
                AggregatedOuts = coloredAsset.AggregatedOuts,
                ColoredAsset = coloredAsset
            };
        }

        public static ColoredAssetInOutsViewModel CreateWithoutChange(TransactionViewModel.ColoredAsset coloredAsset)
        {
            return new ColoredAssetInOutsViewModel
            {
                AggregatedIns = coloredAsset.AggregatedInsWithoutChange,
                AggregatedOuts = coloredAsset.AggregatedOutsWithoutChange,
                ColoredAsset = coloredAsset
            };
        }
    }
}
