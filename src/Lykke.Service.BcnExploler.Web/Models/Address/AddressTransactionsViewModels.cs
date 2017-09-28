using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BCNExplorer.Web.Models;
using Lykke.Service.BcnExploler.Core.Address;
using Lykke.Service.BcnExploler.Web.Models.Offchain;

namespace Lykke.Service.BcnExploler.Web.Models.Address
{
    public class AddressTransactionsViewModel
    {
        public TransactionIdList AllTransactionIdList { get; set; }
        public TransactionIdList SendTransactionIdList { get; set; }
        public TransactionIdList ReceivedTransactionIdList { get; set; }
        public OffchainChannelPagedList OffchainChannelPagedList { get; set; }
        public bool FullLoaded { get; set; }
        private const int PageSize = 20;

        public static AddressTransactionsViewModel Create(string address, IAddressTransactions source, long offchainChannelsCount, int offchainTransactionsPageSize)
        {
            return new AddressTransactionsViewModel
            {
                AllTransactionIdList = new TransactionIdList(source.All?.Select(p => p.TransactionId), PageSize, false),
                SendTransactionIdList = new TransactionIdList(source.Send?.Select(p => p.TransactionId), PageSize, source.FullLoaded),
                ReceivedTransactionIdList = new TransactionIdList(source.Received?.Select(p => p.TransactionId), PageSize, source.FullLoaded),
                FullLoaded = source.FullLoaded,
                OffchainChannelPagedList = OffchainChannelPagedList.Create(
                    offchainChannelsCount,
                    offchainTransactionsPageSize,
                    (url, page) => url.Action("OffchainChannelPage", "Address", new { address = address, page = page })
                )
            };
        }
    }
}
