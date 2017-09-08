using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BcnExploler.Services.AssetBalanceChanges
{
    public class AssetStatsAddressSummaryContract
    {
        public string Address { get; set; }
        public double Balance { get; set; }
    }

    public class AssetStatsTransactionContract
    {
        public string Hash { get; set; }
    }

    public class AssetStatsBlockContract
    {
        public int Height { get; set; }
    }

    public class AssetsStatsAddressChangeContract
    {
        public string Address { get; set; }
        public double Quantity { get; set; }
    }

    public class CommandResultWithModel<T>
    {
        public string[] ErrorMessages { get; set; }
        public bool Success { get; set; }

        public T Data { get; set; }
    }
}
