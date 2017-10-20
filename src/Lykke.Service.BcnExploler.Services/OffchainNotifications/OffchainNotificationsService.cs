using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Transaction;
using Lykke.Service.BcnExploler.Core.OffchainNotifcations;
using Lykke.Service.BcnExploler.Core.Transaction;
using Lykke.Service.BcnExploler.Services.Helpers;
using NBitcoin;

namespace Lykke.Service.BcnExploler.Services.OffchainNotifications
{
    public class FilledMixedTransaction : IFilledMixedTransaction
    {
        public string AssetId { get; set; }
        public string GroupId { get; set; }
        public bool IsColored { get; set; }
        public string HubAddress { get; set; }
        public string ClientAddress1 { get; set; }
        public string ClientAddress2 { get; set; }
        public bool IsOffchain { get; set; }
        public IOnchainTransaction OnchainTransactionData { get; set; }
        public IOffchainTransaction OffchainTransactionData { get; set; }
        public ITransaction FilledOnchainTransactionData { get; set; }

        public static FilledMixedTransaction Create(IMixedTransaction mixedTransaction,
            ITransaction onchainTransaction)
        {
            return new FilledMixedTransaction
            {
                AssetId = mixedTransaction.AssetId,
                ClientAddress1 = mixedTransaction.ClientAddress1,
                ClientAddress2 = mixedTransaction.ClientAddress2,
                OnchainTransactionData = mixedTransaction.OnchainTransactionData,
                FilledOnchainTransactionData = onchainTransaction,
                HubAddress = mixedTransaction.HubAddress,
                IsColored = mixedTransaction.IsColored,
                IsOffchain = mixedTransaction.IsOffchain,
                OffchainTransactionData = mixedTransaction.OffchainTransactionData,
                GroupId = mixedTransaction.GroupId
            };
        }
    }


    public class OffchainNotificationsService:IOffchainNotificationsService
    {
        private readonly IOffchainNotificationsApiProvider _offchainNotificationsApiProvider;
        private readonly ICachedTransactionService _cachedTransactionService;
        private readonly Network _network;

        public OffchainNotificationsService(IOffchainNotificationsApiProvider offchainNotificationsApiProvider,
            ICachedTransactionService cachedTransactionService, Network network)
        {
            _offchainNotificationsApiProvider = offchainNotificationsApiProvider;
            _cachedTransactionService = cachedTransactionService;
            _network = network;
        }


        public async Task<IMixedTransaction> GetOffchainMixedTransaction(string txId)
        {
            return await _offchainNotificationsApiProvider.GetOffchainTransaction(txId);
        }

        public Task<bool> OffchainTransactionExistsAsync(string transactionId)
        {
            return _offchainNotificationsApiProvider.OffchainTransactionExistsAsync(transactionId);
        }
        
        
        public Task<long> GetTransactionCountByAddressAsync(string address)
        {
            var uncoloredAddress = GetUncoloredAddress(address);

            return _offchainNotificationsApiProvider.GetMixedTransactionCountByAddress(uncoloredAddress);
        }

        public Task<long> GetTransactionCountByGroupAsync(string group)
        {
            return _offchainNotificationsApiProvider.GetMixedTransactionCountByGroup(group);
        }

        public Task<bool> IsHubAsync(string address)
        {
            var uncoloredAddress = GetUncoloredAddress(address);

            return _offchainNotificationsApiProvider.IsHubAsync(uncoloredAddress);
        }

        public async Task<IEnumerable<IFilledMixedTransaction>> GetMixedTransactionsByAddressAsync(string address, IPageOptions pageOptions)
        {
            var txs = await _offchainNotificationsApiProvider.GetMixedTransactionsByAddress(address, pageOptions);

            return await FillTransactions(txs.ToArray());
        }

        public async Task<IEnumerable<IFilledMixedTransaction>> GetMixedTransactionsByGroupAsync(string groupId, IPageOptions pageOptions)
        {
            var txs = await _offchainNotificationsApiProvider.GetMixedTransactionsByGroup(groupId, pageOptions);

            return await FillTransactions(txs.ToArray());
        }

        public Task<IEnumerable<IGroup>> GetGroups(string address, bool openOnly = true, int take = 10, bool offchainOnly = true)
        {
            return _offchainNotificationsApiProvider.GetGroups(address, openOnly, take, offchainOnly);
        }

        private string GetUncoloredAddress(string address)
        {
            if (BitcoinAddressHelper.IsBitcoinColoredAddress(address, _network))
            {
                var coloredAddress= new BitcoinColoredAddress(address, _network);

                return coloredAddress.Address.ToString();
            }

            return address;
        }
        
        private async Task<IEnumerable<IFilledMixedTransaction>> FillTransactions(
            params IMixedTransaction[] mixedTransactions)
        {
            var txIds = mixedTransactions.Select(p => p.OnchainTransactionData?.TransactionId)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToList();

            var filledTxs = (await _cachedTransactionService.GetAsync(txIds))
                .ToDictionary(p => p.TransactionId);

            return mixedTransactions.Select(p => FilledMixedTransaction.Create(p,
                p.OnchainTransactionData?.TransactionId != null? filledTxs.GetValueOrDefault(p.OnchainTransactionData?.TransactionId, null): null))
                .ToList();
        }
    }
}
