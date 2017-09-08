using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Testing;
using Lykke.Service.BcnExploler.Core.AssetBalanceChanges;
using Lykke.Service.BcnExploler.Core.Settings;

namespace Lykke.Service.BcnExploler.Services.AssetBalanceChanges
{

    #region Result Models

    public class BalanceSummary : IBalanceSummary
    {
        public IEnumerable<string> AssetIds { get; set; }
        public IEnumerable<IBalanceAddressSummary> AddressSummaries { get; set; }

        public static BalanceSummary Create(IEnumerable<string> assetIds, IEnumerable<IBalanceAddressSummary> addressSummaries)
        {
            return new BalanceSummary
            {
                AddressSummaries = addressSummaries,
                AssetIds = assetIds
            };
        }
    }

    public class BalanceAddressSummary : IBalanceAddressSummary
    {
        public string Address { get; set; }
        public double Balance { get; set; }

        public static BalanceAddressSummary Create(AssetStatsAddressSummaryContract source)
        {
            return new BalanceAddressSummary
            {
                Address = source.Address,
                Balance = source.Balance
            };
        }
    }

    public class BalanceTransaction : IBalanceTransaction
    {
        public string Hash { get; set; }

        public static BalanceTransaction Create(AssetStatsTransactionContract source)
        {
            if (source != null)
            {
                return new BalanceTransaction
                {
                    Hash = source.Hash
                };
            }

            return null;
        }
    }

    public class BalanceBlock : IBalanceBlock
    {
        public int Height { get; set; }

        public static BalanceBlock Create(AssetStatsBlockContract source)
        {
            return new BalanceBlock
            {
                Height = source.Height
            };
        }
    }


    #endregion


    public class AssetBalanceChangesRepository: IAssetBalanceChangesRepository
    {
        private readonly AppSettings _appSettings;

        public AssetBalanceChangesRepository(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public Task<IBalanceSummary> GetSummaryAsync(params string[] assetIds)
        {
            return GetSummaryAsync(null, assetIds);
        }

        public async Task<IBalanceSummary> GetSummaryAsync(int? at, params string[] assetIds)
        {
            var url = _appSettings.BcnExplolerService.AssetStatsServiceUrl.AppendPathSegment("assetstats/addresses");

            foreach (var assetId in assetIds)
            {
                url = url.SetQueryParam("assetIds", assetId);
            }

            if (at != null)
            {
                url.SetQueryParam("at", at);
            }

            var resp = await url.GetJsonAsync<CommandResultWithModel<List<AssetStatsAddressSummaryContract>>>();

            if (!resp.Success)
            {
                throw new HttpRequestException(string.Join(", ", resp.ErrorMessages));
                
            }

            return BalanceSummary.Create(assetIds, resp.Data.Select(BalanceAddressSummary.Create));
        }

        public async Task<IEnumerable<IBalanceTransaction>> GetTransactionsAsync(IEnumerable<string> assetIds, 
            int? fromBlock = null)
        {
            var url = _appSettings.BcnExplolerService.AssetStatsServiceUrl.AppendPathSegment("assetstats/transactions");

            foreach (var assetId in assetIds)
            {
                url = url.SetQueryParam("assetIds", assetId);
            }

            if (fromBlock != null)
            {
                url.SetQueryParam("from", fromBlock);
            }

            var resp = await url.GetJsonAsync<CommandResultWithModel<List<AssetStatsTransactionContract>>>();

            if (!resp.Success)
            {
                throw new HttpRequestException(string.Join(", ", resp.ErrorMessages));
            }

            return resp.Data.Select(BalanceTransaction.Create);
        }

        public async Task<IBalanceTransaction> GetLatestTxAsync(IEnumerable<string> assetIds)
        {
            var url = _appSettings.BcnExplolerService.AssetStatsServiceUrl.AppendPathSegment("assetstats/transactions/last");

            foreach (var assetId in assetIds)
            {
                url = url.SetQueryParam("assetIds", assetId);
            }

            var resp = await url.GetJsonAsync<CommandResultWithModel<AssetStatsTransactionContract>>();

            if (!resp.Success)
            {
                throw new HttpRequestException(string.Join(", ", resp.ErrorMessages));
            }

            return BalanceTransaction.Create(resp.Data);
        }

        public async Task<IDictionary<string, double>> GetAddressQuantityChangesAtBlock(int blockHeight, IEnumerable<string> assetIds)
        {
            
            var url = _appSettings.BcnExplolerService.AssetStatsServiceUrl.AppendPathSegment("assetstats/addresschanges")
                .SetQueryParam("at", blockHeight);

            foreach (var assetId in assetIds)
            {
                url = url.SetQueryParam("assetIds", assetId);
            }


            var resp = await url.GetJsonAsync<CommandResultWithModel<List<AssetsStatsAddressChangeContract>>>();
            if (!resp.Success)
            {
                throw new HttpRequestException(string.Join(", ", resp.ErrorMessages));
            }

            var result = new Dictionary<string, double>();

            foreach (var addressChange in resp.Data.Where(p => !string.IsNullOrEmpty(p.Address)))
            {
                result[addressChange.Address] = addressChange.Quantity;
            }

            return result;
        }

        public async Task<IEnumerable<IBalanceBlock>> GetBlocksWithChanges(IEnumerable<string> assetIds)
        {
            var url = _appSettings.BcnExplolerService.AssetStatsServiceUrl.AppendPathSegment("assetstats/blockChanges");

            foreach (var assetId in assetIds)
            {
                url = url.SetQueryParam("assetIds", assetId);
            }

            var resp = await url.GetJsonAsync<CommandResultWithModel<List<AssetStatsBlockContract>>>();

            if (!resp.Success)
            {
                throw new HttpRequestException(string.Join(", ", resp.ErrorMessages));
            }


            return resp.Data.Select(BalanceBlock.Create);
        }
    }
}
