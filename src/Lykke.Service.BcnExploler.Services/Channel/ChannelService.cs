using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Transaction;
using Lykke.Service.BcnExploler.Core.Channel;
using Lykke.Service.BcnExploler.Core.Transaction;
using Lykke.Service.BcnExploler.Services.Helpers;
using NBitcoin;

namespace Lykke.Service.BcnExploler.Services.Channel
{
    public class FilledChannel:IFilledChannel
    {
        public string GroupId { get; set; }
        public string AssetId { get; set; }
        public bool IsColored { get; set; }
        public IOnchainTransaction OpenTransaction { get; set; }
        public IOnchainTransaction CloseTransaction { get; set; }
        public IEnumerable<IOffchainTransaction> OffchainTransactions { get; set; }
        public ITransaction OpenFilledTransaction { get; set; }
        public ITransaction CloseFilledTransaction { get; set; }

        public static FilledChannel Create(IChannel channel, ITransaction openTransaction,
            ITransaction closeTransaction)
        {
            return new FilledChannel
            {
                OpenTransaction = channel.OpenTransaction,
                CloseTransaction = channel.CloseTransaction,

                OffchainTransactions = channel.OffchainTransactions,
                CloseFilledTransaction = closeTransaction,
                OpenFilledTransaction = openTransaction,
                AssetId = channel.AssetId,
                IsColored = channel.IsColored,
                GroupId = channel.GroupId
            };
        }
    }

    public class FilledMixedTransaction : IFilledMixedTransaction
    {
        public string AssetId { get; set; }
        public bool IsColored { get; set; }
        public string HubAddress { get; set; }
        public string ClientAddress1 { get; set; }
        public string ClientAddress2 { get; set; }
        public bool IsOffchain { get; set; }
        public IOnchainTransaction OnchainTransactionData { get; set; }
        public IOffchainTransaction OffchainTransactionData { get; set; }
        public ITransaction FilledOnchainTransactionData { get; set; }

        public static FilledMixedTransaction Create(IMixedChannelTransaction mixedTransaction,
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
                OffchainTransactionData = mixedTransaction.OffchainTransactionData
            };
        }
    }


    public class ChannelService:IChannelService
    {
        private readonly IOffchainNotificationsApiProvider _offchainNotificationsApiProvider;
        private readonly ICachedTransactionService _cachedTransactionService;
        private readonly Network _network;

        public ChannelService(IOffchainNotificationsApiProvider offchainNotificationsApiProvider,
            ICachedTransactionService cachedTransactionService, Network network)
        {
            _offchainNotificationsApiProvider = offchainNotificationsApiProvider;
            _cachedTransactionService = cachedTransactionService;
            _network = network;
        }

        public async Task<IFilledChannel> GetChannelsByOffchainTransactionIdAsync(string transactionId)
        {
            var channel = await _offchainNotificationsApiProvider.GetByOffchainTransactionIdAsync(transactionId);

            return await FillChannel(channel);
        }

        public Task<bool> OffchainTransactionExistsAsync(string transactionId)
        {
            return _offchainNotificationsApiProvider.OffchainTransactionExistsAsync(transactionId);
        }
        
        
        public async Task<IEnumerable<IFilledChannel>> GetChannelsByAddressFilledAsync(string address, 
            ChannelStatusQueryType channelStatusQueryType = ChannelStatusQueryType.All,
            IPageOptions pageOptions = null)
        {
            var uncoloredAddress = GetUncoloredAddress(address);

            var dbChannels = await _offchainNotificationsApiProvider.GetByAddressAsync(uncoloredAddress, channelStatusQueryType, pageOptions);

            return await FillChannels(dbChannels);
        }
        
        public Task<long> GetTrabsactionCountByAddressAsync(string address)
        {
            var uncoloredAddress = GetUncoloredAddress(address);

            return _offchainNotificationsApiProvider.GetMixedTransactionCountByAddress(uncoloredAddress);
        }

        public Task<long> GetTrabsactionCountByGroupAsync(string group)
        {
            return _offchainNotificationsApiProvider.GetMixedTransactionCountByGroup(group);
        }

        public async Task<IEnumerable<IChannel>> GetChannelsByAddressAsync(string address, ChannelStatusQueryType channelStatusQueryType = ChannelStatusQueryType.All)
        {
            var uncoloredAddress = GetUncoloredAddress(address);

            return await _offchainNotificationsApiProvider.GetByAddressAsync(uncoloredAddress, channelStatusQueryType);
        }

        public Task<bool> IsHubAsync(string address)
        {
            var uncoloredAddress = GetUncoloredAddress(address);

            return _offchainNotificationsApiProvider.IsHubAsync(uncoloredAddress);
        }

        public async Task<IEnumerable<IFilledMixedTransaction>> GetMixedTransactionsByAddressAsync(string address, IPageOptions pageOptions)
        {
            var txs = await _offchainNotificationsApiProvider.GetMixedTransactionsByAddress(address, pageOptions);

            return await FillTransactions(txs);
        }

        public async Task<IEnumerable<IFilledMixedTransaction>> GetMixedTransactionsByGroupAsync(string groupId, IPageOptions pageOptions)
        {
            var txs = await _offchainNotificationsApiProvider.GetMixedTransactionsByGroup(groupId, pageOptions);

            return await FillTransactions(txs);
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

        private async Task<IFilledChannel> FillChannel(IChannel channel)
        {
            return (await FillChannels(new [] { channel })).First();
        }

        private async Task<IEnumerable<IFilledChannel>> FillChannels(IEnumerable<IChannel> channels)
        {
            var txs = channels.SelectMany(p => new[] {p.CloseTransaction, p.OpenTransaction})
                .Where(p => !string.IsNullOrEmpty(p?.TransactionId))
                .Distinct()
                .ToList();

            var filledTxs = (await _cachedTransactionService.GetAsync(txs.Select(p=>p.TransactionId))).ToDictionary(p => p.TransactionId);
            
            return channels.Select(p => 
            FilledChannel.Create(p, 
                p.OpenTransaction != null ? filledTxs.GetValueOrDefault(p.OpenTransaction?.TransactionId, null): null,
                p.CloseTransaction != null ? filledTxs.GetValueOrDefault(p.CloseTransaction?.TransactionId, null): null));
        }

        private async Task<IEnumerable<IFilledMixedTransaction>> FillTransactions(
            IEnumerable<IMixedChannelTransaction> mixedTransactions)
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
