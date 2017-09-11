using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Transaction;
using Flurl;
using Flurl.Http;
using Lykke.Service.BcnExploler.Core.Settings;
using Lykke.Service.BcnExploler.Services.Ninja.Contracts;
using Lykke.Service.BcnExploler.Services.Settings;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.OpenAsset;

namespace Lykke.Service.BcnExploler.Services.Ninja.Transaction
{
    public class Transaction:ITransaction
    {

        public string TransactionId { get; set; }
        public bool IsCoinBase { get; set; }
        public bool IsColor { get; set; }
        public string Hex { get; set; }
        public double Fees { get; set; }
        public int InputsCount { get; set; }
        public int OutputsCount { get; set; }
        public IBlockMinInfo Block { get; set; }
        public IEnumerable<IInOutsByAsset> TransactionsByAssets { get; set; }

        public static Transaction Create(TransactionContract contract, Network network)
        {
            var inputs = contract.Inputs.Select(p => InOut.Create(p, network));
            var outputs = contract.Outputs.Select(p => InOut.Create(p, network));

            var transactionInfo = NBitcoin.Transaction.Parse(contract.Hex);

            return new Transaction
            {
                TransactionId = contract.TransactionId,
                Hex = transactionInfo.ToHex(),
                IsCoinBase = transactionInfo.IsCoinBase,
                IsColor = transactionInfo.HasValidColoredMarker(),
                Block = BlockMinInfo.Create(contract.Block),
                Fees = contract.Fees,
                TransactionsByAssets = InOutsByAsset.Create(inputs, outputs),
                InputsCount = contract.Inputs.Count(),
                OutputsCount = contract.Outputs.Count()
            };
        }
        

        #region Classes
        public class BlockMinInfo: IBlockMinInfo
        {
            public double Confirmations { get; set; }

            public double Height { get; set; }

            public string BlockId { get; set; }

            public DateTime Time { get; set; }

            public static BlockMinInfo Create(TransactionContract.BlockContract contract)
            {
                if (contract != null)
                {
                    return new BlockMinInfo
                    {
                        BlockId = contract.BlockId,
                        Time = contract.BlockTime,
                        Confirmations = contract.Confirmations,
                        Height = contract.Height
                    };
                }

                return null;
            }
        }

        class InOut: IInOut
        {
            public string Address { get; set; }

            public string TransactionId { get; set; }

            public int Index { get; set; }

            public double Value { get; set; }

            public string ScriptPubKey { get; set; }

            public string AssetId { get; set; }

            public double Quantity { get; set; }

            public static InOut Create(InOutContract contract, Network network)
            {
                string address = null;
                try
                {
                    address = GetScriptFromBytes(contract.ScriptPubKey).GetDestinationAddress(network).ToString();
                }
                catch 
                {
                }
                return new InOut
                {
                    Address = address,
                    AssetId = contract.AssetId ?? "",
                    Index = contract.Index,
                    Quantity = contract.Quantity ?? 0,
                    ScriptPubKey = contract.ScriptPubKey,
                    TransactionId = contract.TransactionId,
                    Value = contract.Value
                };
            }
            //todo move to helper class
            private static Script GetScriptFromBytes(string data)
            {
                var bytes = Encoders.Hex.DecodeData(data);
                var script = Script.FromBytesUnsafe(bytes);
                bool hasOps = false;
                var reader = script.CreateReader();
                foreach (var op in reader.ToEnumerable())
                {
                    hasOps = true;
                    if (op.IsInvalid || (op.Name == "OP_UNKNOWN" && op.PushData == null))
                        return null;
                }
                return !hasOps ? null : script;
            }
        }


        class InOutsByAsset : IInOutsByAsset
        {
            public bool IsColored { get; set; }
            public string AssetId { get; set; }
            public IEnumerable<IInOut> TransactionIn { get; set; }
            public IEnumerable<IInOut> TransactionsOut { get; set; }

            //TODO рефакторить (копипаст со старого проекта)
            public static IEnumerable<IInOutsByAsset> Create(
                IEnumerable<InOut> inputs,
                IEnumerable<InOut> outputs)
            {

                //item1 = input, item2 = output
                var dict = new Dictionary<string, Tuple<IList<InOut>, IList<InOut>>>();

                foreach (var input in inputs)
                {
                    if (!dict.ContainsKey(input.AssetId))
                    {
                        var newTuple = new Tuple<IList<InOut>, IList<InOut>>(new List<InOut>(), new List<InOut>());
                        dict.Add(input.AssetId, newTuple);
                    }

                    dict[input.AssetId].Item1.Add(input);
                }

                foreach (var output in outputs)
                {
                    if (!dict.ContainsKey(output.AssetId))
                    {
                        var newTuple = new Tuple<IList<InOut>, IList<InOut>>(new List<InOut>(), new List<InOut>());
                        dict.Add(output.AssetId, newTuple);
                    }

                    dict[output.AssetId].Item2.Add(output);
                }

                return dict.Select(p => new InOutsByAsset
                {
                    AssetId = p.Key,
                    IsColored = !string.IsNullOrEmpty(p.Key),
                    TransactionIn = p.Value.Item1,
                    TransactionsOut = p.Value.Item2
                });
            }
        }
        #endregion
    }

    public class TransactionService:ITransactionService
    {
        private readonly AppSettings _settings;

        public TransactionService(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task<ITransaction> GetAsync(string id)
        {
            try
            {
                var resp = await _settings.BcnExplolerService.NinjaUrl
                    .AppendPathSegment($"transactions/{id}")
                    .SetQueryParam("colored", true)
                    .GetJsonAsync<TransactionContract>();

                return Transaction.Create(resp, _settings.BcnExplolerService.UsedNetwork());
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }
    }
}
