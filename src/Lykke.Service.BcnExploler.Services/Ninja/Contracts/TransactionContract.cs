﻿using System;
using Newtonsoft.Json;

namespace Lykke.Service.BcnExploler.Services.Ninja.Contracts
{
    public class TransactionContract
    {
        public string TransactionId { get; set; }

        [JsonProperty("transaction")]
        public string Hex { get; set; }

        [JsonProperty("fees")]
        public double Fees { get; set; }

        [JsonProperty("block")]
        public BlockContract Block { get; set; }

        [JsonProperty("receivedCoins")]
        public InOutContract[] Outputs { get; set; }

        [JsonProperty("spentCoins")]
        public InOutContract[] Inputs { get; set; }

        [JsonProperty("firstSeen")]
        public DateTime FirstSeen { get; set; }

        #region Classes
        public class BlockContract
        {
            [JsonProperty("confirmations")]
            public double Confirmations { get; set; }

            [JsonProperty("height")]
            public double Height { get; set; }

            [JsonProperty("blockId")]
            public string BlockId { get; set; }

            [JsonProperty("blockTime")]
            public DateTime BlockTime { get; set; }
        }



        #endregion
    }


    public class InOutContract
    {
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("scriptPubKey")]
        public string ScriptPubKey { get; set; }

        [JsonProperty("assetId")]
        public string AssetId { get; set; }

        [JsonProperty("quantity")]
        public double? Quantity { get; set; }
    }
}
