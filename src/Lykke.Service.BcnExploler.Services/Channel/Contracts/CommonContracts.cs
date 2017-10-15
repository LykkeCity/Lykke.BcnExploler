using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BcnExploler.Services.Channel.Contracts
{
    public class PageOptionsRequestContract
    {
        public int? Skip { get; set; }

        public int? Take { get; set; }
    }
}
