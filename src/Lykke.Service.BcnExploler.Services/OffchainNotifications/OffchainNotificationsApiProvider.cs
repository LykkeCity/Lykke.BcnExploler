using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Service.BcnExploler.Core.OffchainNotifcations;
using Lykke.Service.BcnExploler.Core.Settings;
using Lykke.Service.BcnExploler.Services.OffchainNotifications.Contracts;

namespace Lykke.Service.BcnExploler.Services.OffchainNotifications
{

    public static class TransactionTypeMapperHelper
    {
        private static readonly IDictionary<ChannelTransactionType, MixedTransactionType> TypeMapping = new Dictionary<ChannelTransactionType, MixedTransactionType>()
        {
            {ChannelTransactionType.RevokedOffchain,  MixedTransactionType.RevokedOffchain},
            {ChannelTransactionType.ConfirmedOffchain,  MixedTransactionType.ConfirmedOffchain},
            {ChannelTransactionType.CloseOnchain,  MixedTransactionType.CloseOnchain},
            {ChannelTransactionType.OpenOnChain,  MixedTransactionType.OpenOnChain},
            {ChannelTransactionType.ReOpenOnChain,  MixedTransactionType.ReOpenOnChain},
            {ChannelTransactionType.ReOpenedOffchain,  MixedTransactionType.ReOpenedOffchain},
            {ChannelTransactionType.None,  MixedTransactionType.None }
        };

        public static MixedTransactionType Map(ChannelTransactionType source)
        {
            if (TypeMapping.ContainsKey(source))
            {
                return TypeMapping[source];
            }

            return MixedTransactionType.None;
        }
    }

    public class OffchainTransaction: IOffchainTransaction
    {
        public string TransactionId { get; set; }
        public DateTime DateTime { get; set; }
        public string HubAddress { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string AssetId { get; set; }
        public bool IsColored { get; set; }
        public decimal Address1Quantity { get; set; }
        public decimal Address2Quantity { get; set; }
        public decimal Address1QuantityDiff { get; set; }
        public decimal Address2QuantityDiff { get; set; }
        public MixedTransactionType Type { get; set; }

        public static OffchainTransaction Create(OffchainTransactionDataContract source,
            OffchainTransactionMetadataContract metadata, MixedTransactionType type)
        {
            if (source != null)
            {
                return new OffchainTransaction
                {
                    Address1 = metadata.ClientAddress1,
                    AssetId = metadata.AssetId,
                    Address1Quantity = source.Address1Quantity,
                    Address1QuantityDiff = source.Address1QuantityDiff,
                    Address2 = metadata.ClientAddress2,
                    Address2Quantity = source.Address2Quantity,
                    Address2QuantityDiff = source.Address2QuantityDiff,
                    DateTime = source.Date,
                    HubAddress = metadata.HubAddress,
                    IsColored = metadata.IsColored,
                    TransactionId = source.TransactionId,
                    Type = type
                };
            }

            return null;
        }
    }

    public class Group:IGroup
    {
        public string GroupId { get; set; }
        public string AssetId { get; set; }
        public bool IsColored { get; set; }
        public string HubAddress { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public IEnumerable<IMixedTransaction> Transactions { get; set; }

        public static Group Create(GroupContract source)
        {
            return new Group
            {
                Address1 = source.Address1,
                Address2 = source.Address2,
                AssetId = source.AssetId,
                GroupId = source.GroupId,
                IsColored = source.IsColored,
                HubAddress = source.HubAddress,
                Transactions = source.Transactions.Select(MixedTransaction.Create)
            };
        }
    }
    

    public class MixedTransaction : IMixedTransaction
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

        public static MixedTransaction Create(MixedTransactionContract source)
        {
            if (source == null)
            {
                return null;
            }

            var type = TransactionTypeMapperHelper.Map(source.Type);

            return new MixedTransaction
            {
                AssetId = source.Metadata.AssetId,
                ClientAddress1 = source.Metadata.ClientAddress1,
                ClientAddress2 = source.Metadata.ClientAddress2,
                HubAddress = source.Metadata.HubAddress,
                IsColored = source.Metadata.IsColored,
                IsOffchain = source.IsOffchain,
                OffchainTransactionData = OffchainTransaction.Create(source.OffchainTransactionData, source.Metadata, type),
                OnchainTransactionData = OnchainTransaction.Create(source.OnchainTransactionData?.TransactionId, type),
                GroupId = source.GroupId
            };
        }
    }

    public class OnchainTransaction : IOnchainTransaction
    {
        public string TransactionId { get; set; }
        public MixedTransactionType Type { get; set; }


        public static OnchainTransaction Create(string transactionId, MixedTransactionType transactionType)
        {
            if (!string.IsNullOrEmpty(transactionId))
            {
                return new OnchainTransaction
                {
                    TransactionId = transactionId,
                    Type = transactionType
                };
            }

            return null;
        }
    }

    public class OffchainNotificationsApiProvider : IOffchainNotificationsApiProvider
    {
        private readonly string _baseUrl;

        public OffchainNotificationsApiProvider(AppSettings baseSettings)
        {
            _baseUrl = baseSettings.BcnExploler.OffchainNotificationsHandlerUrl;
        }
        
        public async Task<bool> IsHubAsync(string address)
        {
            var resp = await _baseUrl
                .AppendPathSegment($"api/channels/hub/{address}/exists")
                .GetJsonAsync<bool>();

            return resp;
        }

        public async Task<IMixedTransaction> GetOffchainTransaction(string txId)
        {
            var resp = await _baseUrl
                .AppendPathSegment($"api/transactions/offchain/{txId}")
                .GetJsonAsync<MixedTransactionContract>();

            return MixedTransaction.Create(resp);
        }

        public async Task<bool> OffchainTransactionExistsAsync(string transactionId)
        {
            var resp = await _baseUrl
                .AppendPathSegment($"api/transactions/offchain/{transactionId}/exist")
                .GetJsonAsync<bool>();

            return resp;
        }

        public async Task<IEnumerable<IMixedTransaction>> GetMixedTransactionsByAddress(string address, IPageOptions pageOptions)
        {
            var query = _baseUrl
                .AppendPathSegment($"api/transactions/address/{address}");

            query = AppendPageOptions(query, pageOptions);

            return (await query.GetJsonAsync<MixedTransactionContract[]>())
                .Select(MixedTransaction.Create);
        }

        public Task<long> GetMixedTransactionCountByAddress(string address)
        {
            var query = _baseUrl
                .AppendPathSegment($"api/transactions/address/{address}/count");

            return query.GetJsonAsync<long>();
        }

        public async Task<IEnumerable<IMixedTransaction>> GetMixedTransactionsByGroup(string group, IPageOptions pageOptions)
        {
            var query = _baseUrl
                .AppendPathSegment($"api/transactions/group/{group}");

            query = AppendPageOptions(query, pageOptions);

            return (await query.GetJsonAsync<MixedTransactionContract[]>())
                .Select(MixedTransaction.Create);
        }

        public Task<long> GetMixedTransactionCountByGroup(string groupId)
        {
            var query = _baseUrl
                .AppendPathSegment($"api/transactions/group/{groupId}/count");

            return query.GetJsonAsync<long>();
        }

        public async Task<IEnumerable<IGroup>> GetGroups(string address, bool openOnly = true, int take = 10, bool offchainOnly = true)
        {
            var queryParams = new
            {
                openOnly = openOnly,
                take = take,
                offchainOnly = offchainOnly
            };

            var query = _baseUrl
                .AppendPathSegment($"api/groups/address/{address}")
                .SetQueryParams(queryParams);

            var resp = await query.GetJsonAsync<GroupContract[]>();

            return resp.Select(Group.Create);
        }

        private static Url AppendPageOptions(Url source, IPageOptions pageOptions)
        {
            if (pageOptions != null)
            {
                var pageOptionsRequest = new PageOptionsRequestContract
                {
                    Skip = pageOptions.ItemsToSkip,
                    Take = pageOptions.ItemsToTake
                };

                return source.SetQueryParams(pageOptionsRequest);
            }

            return source;
        }
    }
}
