﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;

namespace Lykke.Service.BcnExploler.Services.Ninja
{
    public class IndexerConfiguration
    {

        public Task EnsureSetupAsync()
        {
            var tasks = EnumerateTables()
                .Select(t => t.CreateIfNotExistsAsync())
                .OfType<Task>()
                .ToList();
            tasks.Add(GetBlocksContainer().CreateIfNotExistsAsync());
            return Task.WhenAll(tasks.ToArray());
        }
        public void EnsureSetup()
        {
            try
            {
                EnsureSetupAsync().Wait();
            }
            catch (AggregateException aex)
            {
                ExceptionDispatchInfo.Capture(aex).Throw();
                throw;
            }
        }
        
        public IndexerConfiguration()
        {
            Network = Network.Main;
        }
        public Network Network
        {
            get;
            set;
        }

        public bool AzureStorageEmulatorUsed
        {
            get;
            set;
        }

        public string Node
        {
            get;
            set;
        }

        public string CheckpointSetName
        {
            get;
            set;
        }

        string _Container = "indexer";
        string _TransactionTable = "transactions";
        string _BalanceTable = "balances";
        string _ChainTable = "chain";
        string _WalletTable = "wallets";

        public StorageCredentials StorageCredentials
        {
            get;
            set;
        }
        public CloudBlobClient CreateBlobClient()
        {
            return new CloudBlobClient(MakeUri("blob", AzureStorageEmulatorUsed), StorageCredentials);
        }

        public CloudTable GetTransactionTable()
        {
            return CreateTableClient().GetTableReference(GetFullName(_TransactionTable));
        }
        public CloudTable GetWalletRulesTable()
        {
            return CreateTableClient().GetTableReference(GetFullName(_WalletTable));
        }

        public CloudTable GetTable(string tableName)
        {
            return CreateTableClient().GetTableReference(GetFullName(tableName));
        }
        private string GetFullName(string storageObjectName)
        {
            return (StorageNamespace + storageObjectName).ToLowerInvariant();
        }
        public CloudTable GetBalanceTable()
        {
            return CreateTableClient().GetTableReference(GetFullName(_BalanceTable));
        }
        public CloudTable GetChainTable()
        {
            return CreateTableClient().GetTableReference(GetFullName(_ChainTable));
        }

        public CloudBlobContainer GetBlocksContainer()
        {
            return CreateBlobClient().GetContainerReference(GetFullName(_Container));
        }

        private Uri MakeUri(string clientType, bool azureStorageEmulatorUsed = false)
        {
            if (!azureStorageEmulatorUsed)
            {
                return new Uri(String.Format("http://{0}.{1}.core.windows.net/", StorageCredentials.AccountName,
                    clientType), UriKind.Absolute);
            }
            else
            {
                if (clientType.Equals("blob"))
                {
                    return new Uri("http://127.0.0.1:10000/devstoreaccount1");
                }
                else
                {
                    if (clientType.Equals("table"))
                    {
                        return new Uri("http://127.0.0.1:10002/devstoreaccount1");
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }


        public CloudTableClient CreateTableClient()
        {
            return new CloudTableClient(MakeUri("table", AzureStorageEmulatorUsed), StorageCredentials);
        }


        public string StorageNamespace
        {
            get;
            set;
        }

        public IEnumerable<CloudTable> EnumerateTables()
        {
            yield return GetTransactionTable();
            yield return GetBalanceTable();
            yield return GetChainTable();
            yield return GetWalletRulesTable();
        }
    }
}
