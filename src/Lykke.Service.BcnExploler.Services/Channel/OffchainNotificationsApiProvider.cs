using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Service.BcnExploler.Core.Channel;
using Lykke.Service.BcnExploler.Core.Settings;
using Lykke.Service.BcnExploler.Services.Channel.Contracts;

namespace Lykke.Service.BcnExploler.Services.Channel
{
    public class Channel : IChannel
    {
        public string AssetId { get; set; }
        public bool IsColored { get; set; }
        public IOnchainTransaction OpenTransaction { get; set; }
        public IOnchainTransaction CloseTransaction { get; set; }
        public IEnumerable<IOffchainTransaction> OffchainTransactions { get; set; }

        public static Channel Create(ChannelContract source)
        {
            if (source != null)
            {
                return new Channel
                {
                    AssetId = source.AssetId,
                    CloseTransaction = OnchainTransaction.Create(source.CloseTransactionId, source.CloseTransactionType),
                    IsColored = source.IsColored,
                    OpenTransaction = OnchainTransaction.Create(source.OpenTransactionId, source.OpenTransactionType),
                    OffchainTransactions = source.OffchainTransactions.Select(OffchainTransaction.Create)
                };
            }

            return null;
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
        public bool IsRevoked { get; set; }
        public decimal Address1QuantityDiff { get; set; }
        public decimal Address2QuantityDiff { get; set; }

        public static OffchainTransaction Create(OffchainTransactionContract source)
        {
            return new OffchainTransaction
            {
                TransactionId = source.TransactionId,
                Address1 = source.Address1,
                Address1Quantity = source.Address1Quantity,
                Address2 = source.Address2,
                Address2Quantity = source.Address2Quantity,
                AssetId = source.AssetId,
                DateTime = source.DateTime,
                HubAddress = source.HubAddress,
                IsColored = source.IsColored,
                IsRevoked = source.IsRevoked,
                Address2QuantityDiff = source.Address2QuantityDiff,
                Address1QuantityDiff = source.Address1QuantityDiff
            };
        }

        public static OffchainTransaction Create(OffchainTransactionDataContract source,
            ChannelMetadataContract metadata)
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
                    IsRevoked = source.IsRevoked
                };
            }

            return null;
        }
    }

    public class MixedChannelTransaction : IMixedChannelTransaction
    {
        public string AssetId { get; set; }
        public bool IsColored { get; set; }
        public string HubAddress { get; set; }
        public string ClientAddress1 { get; set; }
        public string ClientAddress2 { get; set; }
        public bool IsOffchain { get; set; }
        public IOnchainTransaction OnchainTransactionData { get; set; }
        public IOffchainTransaction OffchainTransactionData { get; set; }

        public static MixedChannelTransaction Create(ChannelTransactionContract source)
        {
            return new MixedChannelTransaction
            {
                AssetId = source.Metadata.AssetId,
                ClientAddress1 = source.Metadata.ClientAddress1,
                ClientAddress2 = source.Metadata.ClientAddress2,
                HubAddress = source.Metadata.HubAddress,
                IsColored = source.Metadata.IsColored,
                IsOffchain = source.Type == ChannelTransactionType.Offchain,
                OffchainTransactionData = OffchainTransaction.Create(source.OffchainTransactionData, source.Metadata),
                OnchainTransactionData = OnchainTransaction.Create(source.OnchainTransactionData?.TransactionId, source.Type)
            };
        }
    }

    public class OnchainTransaction : IOnchainTransaction
    {
        private static readonly IDictionary<ChannelTransactionType, MixedTransactionType> TypeMapping = new Dictionary<ChannelTransactionType, MixedTransactionType>()
        {
            {ChannelTransactionType.Offchain,  MixedTransactionType.Offchain},
            {ChannelTransactionType.CloseOnchain,  MixedTransactionType.CloseChannel},
            {ChannelTransactionType.OpenOnChain,  MixedTransactionType.ChannelSetup},
            {ChannelTransactionType.ReOpenOnChain,  MixedTransactionType.ReopenChannel},
            {ChannelTransactionType.None,  MixedTransactionType.None }
        };

        public string TransactionId { get; set; }
        public MixedTransactionType Type { get; set; }


        public static OnchainTransaction Create(string transactionId, ChannelTransactionType transactionType)
        {
            if (!string.IsNullOrEmpty(transactionId))
            {
                return new OnchainTransaction
                {
                    TransactionId = transactionId,
                    Type = TypeMapping[transactionType]
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

        public async Task<IChannel> GetByOffchainTransactionIdAsync(string transactionId)
        {
            var resp = await _baseUrl
                .AppendPathSegment($"api/offchaintransactions/{transactionId}")
                .GetJsonAsync<ChannelContract>();

            return Channel.Create(resp);
        }

        public async Task<bool> OffchainTransactionExistsAsync(string transactionId)
        {
            var resp = await _baseUrl
                .AppendPathSegment($"api/offchaintransactions/{transactionId}/exists")
                .GetJsonAsync<bool>();

            return resp;
        }

        public async Task<IEnumerable<IChannel>> GetByAddressAsync(string address, 
            ChannelStatusQueryType channelStatusQueryType = ChannelStatusQueryType.All,
            IPageOptions pageOptions = null)
        {
            var query = _baseUrl
                .AppendPathSegment($"api/channels/address/{address}");

            query = AppendPageOptions(query, pageOptions);
            query = AppenedChannelStatusQueryType(query, channelStatusQueryType);

            var resp = await query
                .GetJsonAsync<ChannelContract[]>();

            return resp.Select(Channel.Create);
        }

        public async Task<bool> IsHubAsync(string address)
        {
            var resp = await _baseUrl
                .AppendPathSegment($"api/channels/hub/{address}/exists")
                .GetJsonAsync<bool>();

            return resp;
        }

        public async Task<long> GetCountByAddressAsync(string address)
        {
            var resp = await _baseUrl
                .AppendPathSegment($"api/channels/address/{address}/count")
                .GetJsonAsync<long>();

            return resp;
        }

        public async Task<IEnumerable<IMixedChannelTransaction>> GetMixedTransactions(string address, IPageOptions pageOptions)
        {
            var query = _baseUrl
                .AppendPathSegment($"api/mixedtransactions/{address}");

            query = AppendPageOptions(query, pageOptions);

            return (await query.GetJsonAsync<ChannelTransactionContract[]>())
                .Select(MixedChannelTransaction.Create);
        }

        public Task<long> TransactionCountByAddress(string address)
        {
            var query = _baseUrl
                .AppendPathSegment($"api/mixedtransactions/{address}/count");

            return query.GetJsonAsync<long>();
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

        private static Url AppenedChannelStatusQueryType(Url source, ChannelStatusQueryType type)
        {
            return source.SetQueryParam("channelStatusQueryType", type.ToString());
        }
    }
}
