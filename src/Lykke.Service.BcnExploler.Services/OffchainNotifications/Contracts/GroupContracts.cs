using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BcnExploler.Services.OffchainNotifications.Contracts
{
    public class GroupContract
    {
        public string GroupId { get; set; }

        public string AssetId { get; set; }

        public bool IsColored { get; set; }

        public string HubAddress { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public IEnumerable<MixedTransactionContract> Transactions { get; set; }
    }
}
