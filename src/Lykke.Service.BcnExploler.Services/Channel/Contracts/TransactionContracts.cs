using System;

namespace Lykke.Service.BcnExploler.Services.Channel.Contracts
{
    public class ChannelTransactionContract
    {
        public string ChannelId { get; set; }

        public ChannelMetadataContract Metadata { get; set; }

        public ChannelTransactionType Type { get; set; }

        public OnchainTransactionDataContract OnchainTransactionData { get; set; }

        public OffchainTransactionDataContract OffchainTransactionData { get; set; }
    }

    public enum ChannelTransactionType
    {
        Offchain = 0,
        OpenOnChain = 1,
        CloseOnchain = 2,
        ReOpenOnChain = 3,
        None = 4
    }

    public class ChannelMetadataContract
    {
        public string AssetId { get; set; }

        public bool IsColored { get; set; }

        public string HubAddress { get; set; }

        public string ClientAddress1 { get; set; }

        public string ClientAddress2 { get; set; }
    }

    public class OnchainTransactionDataContract
    {
        public string TransactionId { get; set; }
    }

    public class OffchainTransactionDataContract
    {
        public string TransactionId { get; set; }

        public decimal Address1Quantity { get; set; }

        public decimal Address2Quantity { get; set; }

        public decimal Address1QuantityDiff { get; set; }

        public decimal Address2QuantityDiff { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime Date { get; set; }
    }
}
