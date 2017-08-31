using Lykke.Service.BcnExploler.Core.Block;

namespace Lykke.Service.BcnExploler.Web.Models.Block
{
    public class BlockHeaderViewModel
    {
        public string BlockId { get; set; }
        public int Height { get; set; }

        public static BlockHeaderViewModel Create(IBlockHeader header)
        {
            if (header != null)
            {
                return new BlockHeaderViewModel
                {
                    BlockId = header.Hash,
                    Height = header.Height
                };
            }

            return null;
        }
    }
}
