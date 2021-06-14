using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Aries.Contracts;
using Hyperledger.Indy.PoolApi;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xamarin.Essentials;

namespace Hyperledger.Aries.Max.Services
{
    public class PoolConfigurator : IPoolConfigurator, IHostedService
    {
        private readonly IPoolService poolService;
        private readonly ILogger<PoolConfigurator> logger;

        private Dictionary<string, string> poolConfigs = new Dictionary<string, string>
        {
            { "sovrin-staging", "pool_transactions_sandbox_genesis" },
            { "sovrin-live", "pool_transactions_live_genesis" },
            { "sovrin-builder", "pool_transactions_builder_genesis" },
            { "bcovrin-test", "pool_transactions_bcovrin_test_genesis" }
        };

        public PoolConfigurator(
            IPoolService poolService,
            ILogger<PoolConfigurator> logger)
        {
            this.poolService = poolService;
            this.logger = logger;
        }

        public async Task ConfigurePoolsAsync()
        {
            foreach (var config in poolConfigs)
            {
                try
                {
                    // Path for bundled genesis txn
                    var filename = Path.Combine(FileSystem.CacheDirectory, "genesis.txn");

                    // Dump file contents to cached filename
                    using (var stream = await FileSystem.OpenAppPackageFileAsync(config.Value))
                    using (var reader = new StreamReader(stream))
                    {
                        File.WriteAllText(filename, await reader.ReadToEndAsync()
                            .ConfigureAwait(false));
                    }

                    // Create pool configuration
                    await poolService.CreatePoolAsync(config.Key, filename)
                        .ConfigureAwait(false);
                }
                catch (PoolLedgerConfigExistsException)
                {
                    // OK
                }
                catch (Exception e)
                {
                    logger.LogCritical(e, "Couldn't create pool config");
                    //throw;
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return ConfigurePoolsAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public interface IPoolConfigurator
    {
        Task ConfigurePoolsAsync();
    }
}
