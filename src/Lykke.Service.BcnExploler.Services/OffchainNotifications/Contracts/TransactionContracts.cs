using System;

namespace Lykke.Service.BcnExploler.Services.OffchainNotifications.Contracts
{
    public class MixedTransactionContract
    {
        public string ChannelId { get; set; }

        public string GroupId { get; set; }

        public OffchainTransactionMetadataContract Metadata { get; set; }

        public ChannelTransactionType Type { get; set; }
        public bool IsOffchain { get; set; }
        public OnchainTransactionDataContract OnchainTransactionData { get; set; }

        public OffchainTransactionDataContract OffchainTransactionData { get; set; }
    }

    public enum ChannelTransactionType
    {
        RevokedOffchain = 0,
        ConfirmedOffchain,
        ReOpenedOffchain,
        OpenOnChain,
        CloseOnchain,
        ReOpenOnChain,
        None
    }

    public class OffchainTransactionMetadataContract
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
        
        public DateTime Date { get; set; }
    }
}
