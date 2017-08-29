using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Service.BcnExploler.Core;
using Lykke.Service.BcnExploler.Core.Domain.Block;
using Lykke.Service.BcnExploler.Services.Ninja.Contracts.Ninja;

namespace Lykke.Service.BcnExploler.Services.Domain
{

    public class BlockHeader:IBlockHeader
    {
        public string Hash { get; set; }
        public int Height { get; set; }
        public DateTime Time { get; set; }
        public long Confirmations { get; set; }
        public bool IsFork { get; set; }

        public static BlockHeader Create(BlockHeaderContract source)
        {
            return new BlockHeader
            {
                Confirmations = source.AdditionalInformation.Confirmations,
                Time = source.AdditionalInformation.Time,
                Height = source.AdditionalInformation.Height,
                Hash = source.AdditionalInformation.BlockId,
            };
        }
    }

    public class BlockService:IBlockService
    {
        private readonly AppSettings _settings;

        public BlockService(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task<IBlockHeader> GetBlockHeaderAsync(string id)
        {
            var resp = await _settings.BcnExplolerService.NinjaUrl
                .AppendPathSegment($"blocks/{id}")
                .SetQueryParam("headeronly", true)
                .GetJsonAsync<BlockHeaderContract>();


            return BlockHeader.Create(resp);
        }

        public Task<IBlockHeader> GetLastBlockHeaderAsync()
        {
            return GetBlockHeaderAsync("tip");
        }

        public Task<IBlock> GetBlockAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
