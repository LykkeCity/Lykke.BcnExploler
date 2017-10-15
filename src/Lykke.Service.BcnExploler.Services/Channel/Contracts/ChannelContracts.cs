using System;
using System.Collections.Generic;

namespace Lykke.Service.BcnExploler.Services.Channel.Contracts
{
    public class ChannelContract
    {
        public string AssetId { get; set; }
        public bool IsColored { get; set; }
        public string OpenTransactionId { get; set; }
        public string CloseTransactionId { get; set; }
        public IEnumerable<OffchainTransactionContract> OffchainTransactions { get; set; }
    }

    public class OffchainTransactionContract
    {
        public string TransactionId { get; set; }
        public DateTime DateTime { get; set; }
        public string HubAddress { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string AssetId { get; set; }
        public bool IsColored { get; set; }
        public decimal Address1Quantity { get; set; }
        public decimal Address2Quantity { get; set; }
        public decimal Address1QuantityDiff { get; set; }
        public decimal Address2QuantityDiff { get; set; }
        public bool IsRevoked { get; set; }
    }
}
