using Newtonsoft.Json;

namespace Lykke.Service.BcnExploler.Services.Ninja.Contracts
{
    public class WhatIsItContract
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        public const string UncoloredAddressType = "PUBKEY_ADDRESS";
        public const string ColoredAddressType = "COLORED_ADDRESS";
        public const string ScriptAddressType = "SCRIPT_ADDRESS";
    }
}
