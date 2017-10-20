using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Web.Models.Asset;

namespace Lykke.Service.BcnExploler.Web.Models.Offchain
{
    public class OffchainPopoverViewModel
    {
        public IEnumerable<OffchainGroupViewModel> Groups { get; set; }

        public string Title { get; set; }
        
        public static OffchainPopoverViewModel Create(IEnumerable<OffchainGroupViewModel> channels, string title)
        {
            return new OffchainPopoverViewModel
            {
                Groups = channels.Reverse(),
                Title = title
            };
        }
    }

    public class IndexedPopoverChannel
    {
        public AssetViewModel Asset { get; set; }

        public string HubAddress { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string AssetId { get; set; }


        public decimal Address1Quantity { get; set; }

        public decimal Address1QuanrtityPercents => Math.Round((Address1Quantity / TotalQuantity) * 100);
        public decimal Address2Quantity { get; set; }
        public decimal Address2QuanrtityPercents => Math.Round((Address2Quantity / TotalQuantity) * 100);

        public decimal TotalQuantity => Address1Quantity + Address2Quantity;
    } 
}
