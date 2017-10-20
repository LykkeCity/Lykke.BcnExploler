using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BcnExploler.Web.Models.Offchain
{
    public class OffchainMixedTransactionsPagedList
    {
        public IEnumerable<OffchainTransactionPageMetadata> Pages { get; set; }
        public long TransactionCount { get; set; }
        public Func<IUrlHelper, int, string> BuildUrl;


        public static OffchainMixedTransactionsPagedList Create(long totalCount, int pageSize, Func<IUrlHelper, int, string> buildUrlFunc)
        {
            return new OffchainMixedTransactionsPagedList
            {
                Pages = OffchainTransactionPageMetadata.CreatePageList(totalCount, pageSize),
                TransactionCount = totalCount,
                BuildUrl = buildUrlFunc
            };
        }
    }

    public class OffchainTransactionPageMetadata
    {
        public int PageNumber { get; set; }

        public bool IsLastPage { get; set; }
        public int NextPage => PageNumber + 1;

        private static OffchainTransactionPageMetadata Create(int pageNumber, bool isLastPage)
        {
            return new OffchainTransactionPageMetadata
            {
                PageNumber = pageNumber,
                IsLastPage = isLastPage
            };
        }

        public static IEnumerable<OffchainTransactionPageMetadata> CreatePageList(long totalCount, int pageSize)
        {
            if (pageSize == 0)
            {
                throw new InvalidOperationException(nameof(pageSize));
            }

            var lastPage = (totalCount + pageSize - 1) / pageSize;
            var pageNumber = 0;

            Func<int, bool> LastPage = (pageNum) => pageNum == lastPage;
            while (!LastPage(pageNumber))
            {
                pageNumber++;
                yield return Create(pageNumber, LastPage(pageNumber));
            }
        }
    }
}
