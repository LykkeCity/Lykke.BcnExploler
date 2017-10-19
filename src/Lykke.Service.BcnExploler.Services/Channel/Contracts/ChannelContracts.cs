using System;
using System.Collections.Generic;

namespace Lykke.Service.BcnExploler.Services.Channel.Contracts
{
    public class ChannelContract
    {
        public string AssetId { get; set; }
        public bool IsColored { get; set; }
        public string GroupId { get; set; }
        public string OpenTransactionId { get; set; }
        public ChannelTransactionType OpenTransactionType { get; set; }
        public string CloseTransactionId { get; set; }
        public ChannelTransactionType CloseTransactionType { get; set; }
        public IEnumerable<OffchainChannelTransactionContract> OffchainTransactions { get; set; }
        public string PrevChannelId { get; set; }
        public string NextChanneld { get; set; }
    }

    public class OffchainChannelTransactionContract
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
    }
}
