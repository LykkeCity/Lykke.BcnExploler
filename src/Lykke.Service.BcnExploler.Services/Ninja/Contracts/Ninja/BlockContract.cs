using System;
using Newtonsoft.Json;

namespace Lykke.Service.BcnExploler.Services.Ninja.Contracts.Ninja
{
    public class BlockContract
    {
        [JsonProperty("additionalInformation")]
        public AdditionalInformationContract AdditionalInformation { get; set; }

        [JsonProperty("block")]
        public string Hex { get; set; }
    }

    public class AdditionalInformationContract
    {
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("blockTime")]
        public DateTime Time { get; set; }
        [JsonProperty("confirmations")]
        public long Confirmations { get; set; }
        [JsonProperty("blockId")]
        public string BlockId { get; set; }
    }

    public class BlockHeaderContract
    {
        [JsonProperty("additionalInformation")]
        public AdditionalInformationContract AdditionalInformation { get; set; }
    }

}
