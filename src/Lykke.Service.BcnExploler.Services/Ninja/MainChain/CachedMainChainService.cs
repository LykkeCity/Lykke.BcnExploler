using System;
using System.IO;
using Lykke.Service.BcnExploler.AzureRepositories.Helpers;
using System.Threading.Tasks;
using AzureStorage;
using Common.Cache;
using Lykke.Service.BcnExploler.Core;
using Lykke.Service.BcnExploler.Core.MainChain;
using NBitcoin;

namespace Lykke.Service.BcnExploler.Services.Ninja.MainChain
{
    public class CachedMainChainService:ICachedMainChainService
    {
        private readonly IBlobStorage _blobStorage;
        private readonly ICacheManager _cacheManager;
        private readonly IMainChainService _mainChainService;

        public CachedMainChainService(ICacheManager cacheManager, 
            IBlobStorage blobStorage, 
            IMainChainService mainChainService)
        {
            _cacheManager = cacheManager;
            _blobStorage = blobStorage;
            _mainChainService = mainChainService;
        }

        private const string BlobContainerName = "mainchain";
        private const string BlobKeyName = "data";
        private const string CacheKey = "MainChainSource";

        public async Task UpdateTemporaryCacheAsync()
        {
            var mc = await GetMainChainAsync();

            await SetToTemporaryCache(mc);
        }

        public async Task<ConcurrentChain> GetMainChainAsync()
        {
            var result = GetFromTemporaryCache() 
                ?? await GetFromPersistentCacheAsync() 
                ?? await _mainChainService.GetMainChainAsync();

            return result;
        }

        public async Task SetToPersistentCacheAsync(ConcurrentChain chain)
        {
            var memorySteam = new MemoryStream();
            chain.WriteTo(memorySteam);

            await _blobStorage.SaveBlobAsync(BlobContainerName, BlobKeyName, memorySteam);
        }

        public async Task SetToTemporaryCache(ConcurrentChain chain)
        {
            _cacheManager.Set(CacheKey, chain, int.MaxValue);
        }

        public async Task UpdatePersistentCacheAsync()
        {
            var mc = await GetMainChainAsync();

            await SetToPersistentCacheAsync(mc);
        }

        private ConcurrentChain GetFromTemporaryCache()
        {
            return _cacheManager.Get<ConcurrentChain>(CacheKey);
        }

        private async Task<ConcurrentChain> GetFromPersistentCacheAsync()
        {
            try
            {
               return new ConcurrentChain((await _blobStorage.GetAsync(BlobContainerName, BlobKeyName)).ReadToEnd());

            }
            catch
            {
                return null;
            }
        }
    }
}
