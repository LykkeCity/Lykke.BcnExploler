using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Web.Models.Offchain
{
    public class OffchainPopoverViewModel
    {
        public IEnumerable<OffchainChannelViewModel> Channels { get; set; }

        public string Title { get; set; }

        public static OffchainPopoverViewModel Create(IEnumerable<OffchainChannelViewModel> channels, string title)
        {
            return new OffchainPopoverViewModel
            {
                Channels = channels,
                Title = title
            };
        }
    }
}
