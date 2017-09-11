using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Service.BcnExploler.Core.Address;
using Lykke.Service.BcnExploler.Core.Settings;
using Lykke.Service.BcnExploler.Services.Helpers;
using Lykke.Service.BcnExploler.Services.Ninja.Contracts;
using Lykke.Service.BcnExploler.Services.Settings;
using NBitcoin;

namespace Lykke.Service.BcnExploler.Services.Ninja.Address
{

    #region  MainInfo

    public class AddressMainInfo : IAddressMainInfo
    {
        public string AddressId { get; set; }
        public string UncoloredAddress { get; set; }
        public string ColoredAddress { get; set; }
        public bool IsColored { get; set; }
    }

    #endregion

    #region   Balance
    public class AddressBalance : IAddressBalance
    {
        private const int TransactionsCountNotCalculated = -1;
        public string AddressId { get; set; }
        public int TotalTransactions { get; set; }
        public bool TotalTransactionsCountCalculated => TotalTransactions != TransactionsCountNotCalculated;
        public int TotalSpendedTransactions { get; set; }
        public bool TotalSpendedTransactionsCountCalculated => TotalSpendedTransactions != TransactionsCountNotCalculated;
        public int TotalReceivedTransactions { get; set; }
        public bool TotalReceivedTransactionsCountCalculated => TotalReceivedTransactions != TransactionsCountNotCalculated;
        public double BtcBalance { get; set; }
        public double UnconfirmedBalanceDelta { get; set; }
        public IEnumerable<IColoredBalance> ColoredBalances { get; set; }

        public AddressBalance()
        {
            ColoredBalances = Enumerable.Empty<IColoredBalance>();
        }

        public static AddressBalance Create(AddressSummaryContract coloredSummary, string address)
        {
            var result = new AddressBalance
            {
                AddressId = address,
                BtcBalance = coloredSummary.Confirmed.Balance,
                TotalTransactions = coloredSummary.Confirmed.TotalTransactions,
                TotalReceivedTransactions = coloredSummary.Confirmed.ReceivedTransactions ?? TransactionsCountNotCalculated,
                TotalSpendedTransactions = coloredSummary.Confirmed.SpendedTransactions ?? TransactionsCountNotCalculated,
                UnconfirmedBalanceDelta = coloredSummary.Unconfirmed?.Balance ?? 0
            };
            var unconfirmedAssets = coloredSummary.Unconfirmed?.Assets ?? Enumerable.Empty<AddressSummaryContract.AddressSummaryInnerContract.AddressAssetContract>().ToList();

            foreach (var assetSummary in unconfirmedAssets.Where(p => !coloredSummary.Confirmed.Assets.Select(x => x.AssetId).Contains(p.AssetId))) //assets with 0
            {
                coloredSummary.Confirmed.Assets.Add(new AddressSummaryContract.AddressSummaryInnerContract.AddressAssetContract
                {
                    AssetId = assetSummary.AssetId
                });
            }

            result.ColoredBalances = coloredSummary.Confirmed.Assets.Select(p =>
            {
                var coloredBalance = new ColoredBalance
                {
                    AssetId = p.AssetId,
                    Quantity = p.Quantity
                };
                var unconfirmedAsset = unconfirmedAssets.FirstOrDefault(ua => ua.AssetId == coloredBalance.AssetId);
                if (unconfirmedAsset != null)
                {
                    coloredBalance.UnconfirmedQuantityDelta = unconfirmedAsset.Quantity;
                }

                return coloredBalance;
            });

            return result;
        }
    }

    public class ColoredBalance : IColoredBalance
    {
        public string AssetId { get; set; }
        public double Quantity { get; set; }
        public double UnconfirmedQuantityDelta { get; set; }
    }

    #endregion


    #region Transactions

    public class AddressTransactions : IAddressTransactions
    {
        public IEnumerable<IAddressTransaction> All { get; set; }
        public IEnumerable<IAddressTransaction> Send { get; set; }
        public IEnumerable<IAddressTransaction> Received { get; set; }
        public bool FullLoaded { get; set; }

        public AddressTransactions()
        {
            All = Enumerable.Empty<IAddressTransaction>();
            Send = Enumerable.Empty<IAddressTransaction>();
            Received = Enumerable.Empty<IAddressTransaction>();
        }

        public static IAddressTransactions Create(string address, AddressTransactionListContract source)
        {
            var allTxs = source.Transactions.Select(p => AddressTransaction.Create(address, p)).ToList();

            return new AddressTransactions
            {
                All = allTxs,
                Received = allTxs.Where(p => p.IsReceived).ToList(),
                Send = allTxs.Where(p => !p.IsReceived).ToList(),
                FullLoaded = string.IsNullOrEmpty(source.ContinuationToken)
            };
        }
    }


    public class AddressTransaction : IAddressTransaction
    {
        public string TransactionId { get; set; }

        public bool IsReceived { get; set; }

        public static AddressTransaction Create(string address, AddressTransactionListItemContract source)
        {
            return new AddressTransaction
            {
                TransactionId = source.TxId,
                IsReceived = IsReceivedTx(source),
            };
        }

        private static bool IsReceivedTx(AddressTransactionListItemContract source)
        {
            if (source.Amount > 0 || (source.Amount == 0 && source.Received.Any()))
            {
                return true;
            }

            return false;
        }
    }

    #endregion




    public class AddressService:IAddressService
    {
        private readonly AppSettings _appSettings;

        public AddressService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<IAddressMainInfo> GetMainInfoAsync(string id)
        {
            if (BitcoinAddressHelper.IsBitcoinColoredAddress(id, _appSettings.BcnExplolerService.UsedNetwork()))
            {
                var result = new BitcoinColoredAddress(id, _appSettings.BcnExplolerService.UsedNetwork());

                return new AddressMainInfo
                {
                    AddressId = id,
                    ColoredAddress = result.ToWif(),
                    UncoloredAddress = result.Address.ToString(),
                    IsColored = true
                };
            }

            if (BitcoinAddressHelper.IsBitcoinPubKeyAddress(id, _appSettings.BcnExplolerService.UsedNetwork()))
            {
                var result = new BitcoinPubKeyAddress(id, _appSettings.BcnExplolerService.UsedNetwork());

                return new AddressMainInfo
                {
                    AddressId = id,
                    ColoredAddress = result.ToColoredAddress().ToWif(),
                    UncoloredAddress = result.ToString(),
                    IsColored = false
                };
            }

            if (BitcoinAddressHelper.IsBitcoinScriptAddress(id, _appSettings.BcnExplolerService.UsedNetwork()))
            {
                var result = new BitcoinScriptAddress(id, _appSettings.BcnExplolerService.UsedNetwork());

                return new AddressMainInfo
                {
                    AddressId = id,
                    ColoredAddress = result.ToColoredAddress().ToWif(),
                    UncoloredAddress = result.ToString(),
                    IsColored = false
                };
            }

            return null;
        }

        public async Task<IAddressTransactions> GetTransactions(string id)
        {
            var resp = await _appSettings.BcnExplolerService.NinjaUrl.AppendPathSegment($"/balances/{id}").GetJsonAsync<AddressTransactionListContract>();

            return AddressTransactions.Create(id, resp);
        }

        public async Task<IAddressBalance> GetBalanceAsync(string address, int? at = null)
        {
            var url = _appSettings.BcnExplolerService.NinjaUrl.AppendPathSegment($"/balances/{address}/summary")
                .SetQueryParam("colored", true);

            if (at != null)
            {
                url = url.SetQueryParam("at", at.Value);
            }

            var resp = await url.GetJsonAsync<AddressSummaryContract>();

            return AddressBalance.Create(resp, address);
        }
    }
}
