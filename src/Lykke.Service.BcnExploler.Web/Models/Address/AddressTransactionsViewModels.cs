using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCNExplorer.Web.Models;
using Lykke.Service.BcnExploler.Core.Address;

namespace Lykke.Service.BcnExploler.Web.Models.Address
{
    public class AddressTransactionsViewModel
    {
        public TransactionIdList AllTransactionIdList { get; set; }
        public TransactionIdList SendTransactionIdList { get; set; }
        public TransactionIdList ReceivedTransactionIdList { get; set; }
        public bool FullLoaded { get; set; }
        private const int PageSize = 20;

        public static AddressTransactionsViewModel Create(IAddressTransactions source)
        {
            return new AddressTransactionsViewModel
            {
                AllTransactionIdList = new TransactionIdList(source.All?.Select(p => p.TransactionId), PageSize, source.FullLoaded),
                SendTransactionIdList = new TransactionIdList(source.Send?.Select(p => p.TransactionId), PageSize, source.FullLoaded),
                ReceivedTransactionIdList = new TransactionIdList(source.Received?.Select(p => p.TransactionId), PageSize, source.FullLoaded),
                FullLoaded = source.FullLoaded
            };
        }
    }
}
