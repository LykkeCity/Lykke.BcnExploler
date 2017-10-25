using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Web.Models.Asset;

namespace Lykke.Service.BcnExploler.Web.Models.Offchain
{
    public class OffchainHubPopoverViewModel
    {
        public IEnumerable<OffchainGroupViewModel> Groups { get; set; }

        public string Title { get; set; }
        
        public static OffchainHubPopoverViewModel Create(IEnumerable<OffchainGroupViewModel> groups, string title)
        {
            return new OffchainHubPopoverViewModel
            {
                Groups = groups.Reverse(),
                Title = title
            };
        }
    }

    public class OffchainClientPopoverViewModel
    {
        public IEnumerable<Group> Groups { get; set; }
        public string Title { get; set; }

        public static OffchainClientPopoverViewModel Create(IEnumerable<OffchainGroupViewModel> groups, 
            string title, 
            string currentUncoloredAddress)
        {

            return new OffchainClientPopoverViewModel
            {
                Title = title,
                Groups = groups.Select(p => Group.Create(p, currentUncoloredAddress))
            };
        }

        public class Group
        {
            public AssetViewModel Asset { get; set; }

            public IEnumerable<Transaction> Transactions { get; set; }

            public static Group Create(OffchainGroupViewModel source, string currentUncoloredAddress)
            {
                Func<OffchainTransactionViewModel, decimal> qtySelector;

                var latestOffchainTx = source.OffChainTransactions.OrderBy(p => p.DateTime).Last();

                if (latestOffchainTx.Address1 == currentUncoloredAddress)
                {
                    qtySelector = p => p.Address1Quantity;
                }
                else if (latestOffchainTx.Address2 == currentUncoloredAddress)
                {
                    qtySelector = p => p.Address2Quantity;
                }
                else
                {
                    throw new Exception("Invalid condition");
                }

                return new Group
                {
                    Asset = source.Asset,
                    Transactions = source.OffChainTransactions
                        .Select(p => Transaction.Create(p, qtySelector))
                };
            }
        }

        public class Transaction
        {
            public string HubAddress { get; set; }
            public decimal AddressQuantity { get; set; }
            public decimal TotalQuantity { get; set; }
            public DateTime DateTime { get; set; }
            public decimal AddressQuanrtityPercents => Math.Round((AddressQuantity / TotalQuantity) * 100);

            public static Transaction Create(OffchainTransactionViewModel source, 
                Func<OffchainTransactionViewModel, decimal> qtySelector)
            {
                return new Transaction
                {
                    AddressQuantity = qtySelector(source),
                    TotalQuantity = source.TotalQuantity,
                    HubAddress = source.HubAddress,
                    DateTime = source.DateTime
                };
            }

        }
    }
}
