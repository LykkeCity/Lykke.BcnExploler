﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Service.BcnExploler.Services.Ninja.Contracts
{

    #region WhatIsItAdrressContract

    public class WhatIsItAdrressContract
    {
        [JsonProperty("coloredAddress")]
        public string ColoredAddress { get; set; }
        [JsonProperty("uncoloredAddress")]
        public string UncoloredAddress { get; set; }
    }
    
    #endregion

    #region AddressTransactionListContract

    public class AddressTransactionListContract
    {
        [JsonProperty("continuation")]
        public string ContinuationToken { get; set; }

        [JsonProperty("operations")]
        public AddressTransactionListItemContract[] Transactions { get; set; }
    }

    public class AddressTransactionListItemContract
    {
        [JsonProperty("transactionId")]
        public string TxId { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }

        [JsonProperty("blockId")]
        public string BlockId { get; set; }
        
        [JsonProperty("receivedCoins")]
        public InOutContract[] Received { get; set; }

        [JsonProperty("spentCoins")]
        public InOutContract[] Spent { get; set; }
    }



    #endregion

    #region AddressSummaryContract

    public class AddressSummaryContract
    {
        [JsonProperty("confirmed")]
        public AddressSummaryInnerContract Confirmed { get; set; }

        [JsonProperty("unConfirmed")]
        public AddressSummaryInnerContract Unconfirmed { get; set; }

        public class AddressSummaryInnerContract
        {
            [JsonProperty("transactionCount")]
            public int TotalTransactions { get; set; }

            [JsonProperty("spendedTransactionCount")]
            public int? SpendedTransactions { get; set; }

            [JsonProperty("receivedTransactionCount")]
            public int? ReceivedTransactions { get; set; }

            [JsonProperty("amount")]
            public long Balance { get; set; }
            [JsonProperty("assets")]
            public List<AddressAssetContract> Assets { get; set; }

            public class AddressAssetContract
            {
                [JsonProperty("asset")]
                public string AssetId { get; set; }
                [JsonProperty("quantity")]
                public long Quantity { get; set; }
                [JsonProperty("received")]
                public long Received { get; set; }
            }
        }
    }





    #endregion
}
