using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Services.Ninja;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;

namespace Lykke.Service.BcnExploler.Services
{
    public class IndexerClient
    {
        private readonly IndexerConfiguration _Configuration;
        public IndexerConfiguration Configuration
        {
            get
            {
                return _Configuration;
            }
        }

        public IndexerClient(IndexerConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            _Configuration = configuration;
            BalancePartitionSize = 50;
        }

        public int BalancePartitionSize
        {
            get;
            set;
        }

        public async Task<Block> GetBlock(uint256 blockId)
        {
            var ms = new MemoryStream();
            var container = Configuration.GetBlocksContainer();
            try
            {

                await container.GetPageBlobReference(blockId.ToString()).DownloadToStreamAsync(ms);
                ms.Position = 0;
                Block b = new Block();
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
        public ConcurrentChain GetMainChain()
        {
            ConcurrentChain chain = new ConcurrentChain();
            SynchronizeChain(chain);
            return chain;
        }

        public void SynchronizeChain(ChainBase chain)
        {
            if (chain.Tip != null && chain.Genesis.HashBlock != Configuration.Network.GetGenesis().GetHash())
                throw new ArgumentException("Incompatible Network between the indexer and the chain", "chain");
            if (chain.Tip == null)
                chain.SetTip(new ChainedBlock(Configuration.Network.GetGenesis().Header, 0));
            GetChainChangesUntilFork(chain.Tip, false)
                .UpdateChain(chain);
        }

        public IEnumerable<ChainBlockHeader> GetChainChangesUntilFork(ChainedBlock currentTip, bool forkIncluded, CancellationToken cancellation = default(CancellationToken))
        {
            var oldTip = currentTip;
            var table = Configuration.GetChainTable();

            foreach (var chainPart in
                ExecuteBalanceQuery(table, new TableQuery(), new[] { 1, 2, 10 })
                    .Concat(table.ExecuteQuerySegmentedAsync(new TableQuery(), null).Result.Skip(2))
                    .Select(e => new ChainPartEntry(e)))
            {
                cancellation.ThrowIfCancellationRequested();

                int height = chainPart.ChainOffset + chainPart.BlockHeaders.Count - 1;
                foreach (var block in chainPart.BlockHeaders.Reverse<BlockHeader>())
                {
                    if (currentTip == null && oldTip != null)
                        throw new InvalidOperationException("No fork found, the chain stored in azure is probably different from the one of the provided input");
                    if (oldTip == null || height > currentTip.Height)
                        yield return CreateChainChange(height, block);
                    else
                    {
                        if (height < currentTip.Height)
                            currentTip = currentTip.FindAncestorOrSelf(height);
                        var chainChange = CreateChainChange(height, block);
                        if (chainChange.BlockId == currentTip.HashBlock)
                        {
                            if (forkIncluded)
                                yield return chainChange;
                            yield break;
                        }
                        yield return chainChange;
                        currentTip = currentTip.Previous;
                    }
                    height--;
                }
            }
        }

        private ChainBlockHeader CreateChainChange(int height, BlockHeader block)
        {
            return new ChainBlockHeader()
            {
                Height = height,
                Header = block,
                BlockId = block.GetHash()
            };
        }


        private IEnumerable<DynamicTableEntity> ExecuteBalanceQuery(CloudTable table, TableQuery tableQuery, IEnumerable<int> pages)
        {
            pages = pages ?? new int[0];
            var pagesEnumerator = pages.GetEnumerator();
            TableContinuationToken continuation = null;
            do
            {
                tableQuery.TakeCount = pagesEnumerator.MoveNext() ? (int?)pagesEnumerator.Current : null;
               
                var segment = table.ExecuteQuerySegmentedAsync(tableQuery, continuation).Result;
                continuation = segment.ContinuationToken;
                foreach (var entity in segment)
                {
                    yield return entity;
                }
            } while (continuation != null);
        }
    }

    public static class ChainChangeEntryExtensions
    {
        public static void UpdateChain(this IEnumerable<ChainBlockHeader> entries, ChainBase chain)
        {
            Stack<ChainBlockHeader> toApply = new Stack<ChainBlockHeader>();
            foreach (var entry in entries)
            {
                var prev = chain.GetBlock(entry.Header.HashPrevBlock);
                if (prev == null)
                    toApply.Push(entry);
                else
                {
                    toApply.Push(entry);
                    break;
                }
            }
            while (toApply.Count > 0)
            {
                var newTip = toApply.Pop();

                var chained = new ChainedBlock(newTip.Header, newTip.BlockId, chain.GetBlock(newTip.Header.HashPrevBlock));
                chain.SetTip(chained);
            }
        }
    }
    public class ChainBlockHeader
    {
        public uint256 BlockId
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }
        public BlockHeader Header
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Height + "-" + BlockId;
        }
    }

    public class ChainPartEntry
    {
        public ChainPartEntry()
        {
            BlockHeaders = new List<BlockHeader>();
        }

        public ChainPartEntry(DynamicTableEntity entity)
        {
            ChainOffset = StringToHeight(entity.RowKey);
            BlockHeaders = new List<BlockHeader>();
            foreach (var prop in entity.Properties)
            {
                var header = new BlockHeader();
                header.FromBytes(prop.Value.BinaryValue);
                BlockHeaders.Add(header);
            }
        }

        public int ChainOffset
        {
            get;
            set;
        }

        public List<BlockHeader> BlockHeaders
        {
            get;
            private set;
        }

        public BlockHeader GetHeader(int height)
        {
            if (height < ChainOffset)
                return null;
            height = height - ChainOffset;
            if (height >= BlockHeaders.Count)
                return null;
            return BlockHeaders[height];
        }

        public DynamicTableEntity ToEntity()
        {
            DynamicTableEntity entity = new DynamicTableEntity();
            entity.PartitionKey = "a";
            entity.RowKey = HeightToString(ChainOffset);
            int i = 0;
            foreach (var header in BlockHeaders)
            {
                entity.Properties.Add("a" + i, new EntityProperty(header.ToBytes()));
                i++;
            }
            return entity;
        }

        internal static string HeightToString(int height)
        {
            var input = height.ToString(format);
            return ToggleChars(input);
        }
        internal static int StringToHeight(string rowkey)
        {
            return int.Parse(ToggleChars(rowkey));
        }


        internal static string ToggleChars(string input)
        {
            char[] result = new char[input.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var index = Array.IndexOf(Digit, input[i]);
                result[i] = Digit[Digit.Length - index - 1];
            }
            return new string(result);
        }
        internal static string format = new string(Enumerable.Range(0, int.MaxValue.ToString().Length).Select(c => '0').ToArray());
        static char[] Digit = Enumerable.Range(0, 10).Select(c => c.ToString()[0]).ToArray();


    }


}
