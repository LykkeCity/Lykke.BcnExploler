﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.Core.MainChain;

namespace Lykke.Service.BcnExploler.Web.Jobs
{
    public class MainChainFunctions
    {
        private readonly ICachedMainChainService _cachedMainChainService;

        public MainChainFunctions(ICachedMainChainService cachedMainChainService)
        {
            _cachedMainChainService = cachedMainChainService;
        }


        [TimerTrigger("00:30:00")]
        public Task UpdatePersistentCache()
        {
            try
            {

                return _cachedMainChainService.UpdatePersistentCacheAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        [TimerTrigger("00:02:00")]
        public Task UpdateTemporaryCache()
        {
            try
            {
                return _cachedMainChainService.UpdateTemporaryCacheAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
