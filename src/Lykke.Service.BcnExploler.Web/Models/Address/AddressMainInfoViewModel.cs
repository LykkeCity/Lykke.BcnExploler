using Lykke.Service.BcnExploler.Core.Address;

namespace Lykke.Service.BcnExploler.Web.Models.Address
{
    public class AddressMainInfoViewModel
    {
        public string AddressId { get; set; }
        public string UncoloredAddress { get; set; }
        public string ColoredAddress { get; set; }

        public bool IsColoredAddress { get; set; }

        public bool IsOffchainHub { get; set; }

        public static AddressMainInfoViewModel Create(IAddressMainInfo addressMainInfo, bool isHub)
        {
            return new AddressMainInfoViewModel
            {
                AddressId = addressMainInfo.AddressId,
                ColoredAddress = addressMainInfo.ColoredAddress,
                UncoloredAddress = addressMainInfo.UncoloredAddress,
                IsColoredAddress = addressMainInfo.IsColored,
                IsOffchainHub = isHub
            };
        }
    }
}