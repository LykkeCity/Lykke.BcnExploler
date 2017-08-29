using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Service.BcnExploler.Core;
using Lykke.Service.BcnExploler.Core.Domain.Block;
using Lykke.Service.BcnExploler.Services.Domain.Settings;
using Lykke.Service.BcnExploler.Services.Ninja.Contracts.Ninja;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using NBitcoin;
using NBitcoin.Indexer;
using NBitcoin.OpenAsset;

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

    public class Block : IBlock
    {
        public string Hash { get; set; }
        public long Height { get; set; }
        public DateTime Time { get; set; }
        public long Confirmations { get; set; }
        public double Difficulty { get; set; }
        public string MerkleRoot { get; set; }
        public long Nonce { get; set; }
        public int TotalTransactions { get; set; }
        public string PreviousBlock { get; set; }
        public IEnumerable<string> AllTransactionIds { get; set; }
        public IEnumerable<string> ColoredTransactionIds { get; set; }
        public IEnumerable<string> UncoloredTransactionIds { get; set; }

        public Block()
        {
            AllTransactionIds = Enumerable.Empty<string>();
            ColoredTransactionIds = Enumerable.Empty<string>();
            UncoloredTransactionIds = Enumerable.Empty<string>();
        }
    }

    public class BlockService:IBlockService
    {
        private readonly AppSettings _settings;
        private readonly IndexerClient _indexerClient;

        public BlockService(AppSettings settings, IndexerClient indexerClient)
        {
            _settings = settings;
            _indexerClient = indexerClient;
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
            if (uint256.TryParse(id, out var hash))
            {
                return GetBlockAsync(hash);
            }
            if (int.TryParse(id, out var height))
            {
                return GetBlockAsync(height);
            }

            return null;
        }

        public async Task<IBlock> GetBlockAsync(uint256 hash)
        {
            var result = new Lazy<Block>(() => new Block());

            var fillHeaderTask = GetBlockHeaderAsync(hash.ToString())
                .ContinueWith(tsk =>
                {
                    if (tsk.Result != null)
                    {
                        FillHeaderData(tsk.Result, result.Value);
                    }
                });

            var fillDbDataTask = Task.Run(async () =>
            {
                var block = await GetBlock(hash);
                if (block != null)
                {
                    FillBlockDataFromDb(block, result.Value);
                }
            });

            await Task.WhenAll(fillHeaderTask, fillDbDataTask);

            return result.IsValueCreated ? result.Value : null;
        }

        private async Task<IBlock> GetBlockAsync(int height)
        {
            var header = await GetBlockHeaderAsync(height.ToString());
            if (header != null)
            {
                var result = new Block();
                var block = await GetBlock(uint256.Parse(header.Hash));

                FillHeaderData(header, result);
                FillBlockDataFromDb(block, result);

                return result;
            }

            return null;
        }


        #region read block from db

        //copy code from nbitcoin indexer cause nuget pckg doest work properly on this method
        public async Task<NBitcoin.Block> GetBlock(uint256 blockId)
        {
            var ms = new MemoryStream();

            var container = _indexerClient.Configuration.GetBlocksContainer();
            try
            {
                await container.GetPageBlobReference(blockId.ToString()).DownloadToStreamAsync(ms);

                ms.Position = 0;
                NBitcoin.Block b = new NBitcoin.Block();
                b.ReadWrite(ms, false);
                return b;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation != null && ex.RequestInformation.HttpStatusCode == 404)
                {
                    return null;
                }
                throw;
            }
        }

        #endregion


        private void FillHeaderData(IBlockHeader header, Block result)
        {
            result.Confirmations = header.Confirmations;
            result.Height = header.Height;
        }

        private void FillBlockDataFromDb(NBitcoin.Block block, Block result)
        {
            result.Time = block.Header.BlockTime.DateTime;
            result.Hash = block.Header.GetHash().ToString();
            result.TotalTransactions = block.Transactions.Count;
            result.Difficulty = block.Header.Bits.Difficulty;
            result.MerkleRoot = block.Header.HashMerkleRoot.ToString();
            result.PreviousBlock = block.Header.HashPrevBlock.ToString();
            result.Nonce = block.Header.Nonce;
            result.AllTransactionIds = block.Transactions.Select(p => p.GetHash().ToString()).ToList();
            result.ColoredTransactionIds = block.Transactions.Where(p => p.HasValidColoredMarker()).Select(p => p.GetHash().ToString()).ToList();
            result.UncoloredTransactionIds = block.Transactions.Where(p => !p.HasValidColoredMarker()).Select(p => p.GetHash().ToString()).ToList();
        }
    }
}
