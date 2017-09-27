using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;

namespace Lykke.Service.BcnExploler.Web.Models.Asset
{
    public class AssetDictionary
    {
        public IDictionary<string, AssetViewModel> Dic { get; set; }

        public AssetViewModel Get(string asset)
        {
            if (!string.IsNullOrEmpty(asset) && Dic.ContainsKey(asset))
            {
                return Dic[asset];
            }

            return null;
        }

        public T GetAssetProp<T>(string asset, Expression<Func<AssetViewModel, T>> selectPropExpression, T defaultValue)
        {
            var ent = Get(asset);
            if (ent != null)
            {
                return selectPropExpression.Compile()(ent);
            }
            return defaultValue;
        }

        public static AssetDictionary Create(IDictionary<string, IAssetDefinition> source)
        {
            return new AssetDictionary
            {
                Dic = source.ToDictionary(p => p.Key, p => AssetViewModel.Create(p.Value))
            };
        }
    }
}