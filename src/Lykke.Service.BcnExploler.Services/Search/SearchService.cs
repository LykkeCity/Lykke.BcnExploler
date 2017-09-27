using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Search;
using Lykke.Service.BcnExploler.Core.Settings;
using Lykke.Service.BcnExploler.Services.Helpers;
using Lykke.Service.BcnExploler.Services.Ninja.Contracts;
using Lykke.Service.BcnExploler.Services.Settings;

namespace Lykke.Service.BcnExploler.Services.Search
{
    public class SearchService : ISearchService
    {
        private readonly IAssetService _assetProvider;
        private readonly AppSettings _baseSettings;

        public SearchService(IAssetService assetProvider,
            AppSettings baseSettings)
        {
            _assetProvider = assetProvider;
            _baseSettings = baseSettings;
        }

        public async Task<SearchResultType?> GetTypeAsync(string id)
        {
            if (BitcoinAddressHelper.IsAddress(id, _baseSettings.BcnExploler.UsedNetwork()))
            {
                return SearchResultType.Address;
            }

            var result = await GetFromNinja(id);

            if (result == null)
            {
                var asset = await _assetProvider.GetAssetAsync(id);
                result = asset != null ? (SearchResultType?)SearchResultType.Asset : null;
            }

            return result;
        }

        private async Task<SearchResultType?> GetFromNinja(string id)
        {
            try
            {
                var responce = await _baseSettings.BcnExploler.NinjaUrl.AppendPathSegment($"whatisit/{id}").GetStringAsync();

                if (IsBlock(responce))
                {
                    return SearchResultType.Block;
                }
                if (IsTransaction(responce))
                {
                    return SearchResultType.Transaction;
                }

                return null;
            }
            catch (Exception) //unknown type
            {
                return null;
            }

        }

        private bool IsBlock(string responce)
        {
            var deserialized = TryDeserialize<BlockHeaderContract>(responce);
            return deserialized?.AdditionalInformation?.BlockId != null;
        }

        private bool IsTransaction(string responce)
        {
            var deserialized = TryDeserialize<TransactionContract>(responce);
            return deserialized?.TransactionId != null;
        }

        private bool IsAddress(string responce)
        {
            var deserialized = TryDeserialize<WhatIsItContract>(responce);
            return deserialized?.Type == WhatIsItContract.ColoredAddressType ||
                   deserialized?.Type == WhatIsItContract.UncoloredAddressType ||
                   deserialized?.Type == WhatIsItContract.ScriptAddressType;
        }

        private T TryDeserialize<T>(string source)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(source);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
