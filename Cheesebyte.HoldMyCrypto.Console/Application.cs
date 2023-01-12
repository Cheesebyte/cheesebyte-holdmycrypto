using Cheesebyte.HoldMyCrypto.Caching;
using Cheesebyte.HoldMyCrypto.Caching.Interfaces;
using Cheesebyte.HoldMyCrypto.Console.Commands;
using Cheesebyte.HoldMyCrypto.Console.Commands.Interfaces;
using Cheesebyte.HoldMyCrypto.Console.Extensions;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Binance;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.CryptoCompare;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Services;
using Cheesebyte.HoldMyCrypto.Services.Interfaces;
using Cheesebyte.HoldMyCrypto.Vaults;
using Cheesebyte.HoldMyCrypto.Vaults.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cheesebyte.HoldMyCrypto.Console
{
    /// <summary>
    /// Encapsulates application initialisation. Basically sets up
    /// the dependency injection and starts the main functionality.
    /// </summary>
    public class Application
    {
        // Force update boolean could be added to an options object
        // together with a validator in later iterations. Keep it
        // simple for now.
        private readonly bool _forceUpdate;

        private readonly IServiceProvider _serviceProvider;
        private readonly IConfigurationRoot _configurationRoot;

        public Application(
            IServiceCollection serviceCollection,
            IConfigurationRoot configurationRoot,
            bool forceUpdate)
        {
            ConfigureServices(serviceCollection);

            _forceUpdate = forceUpdate;
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _configurationRoot = configurationRoot;
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Requires IConfigurationRoot, see Program.cs
            serviceCollection.AddSingleton<IConfigurationRoot>(_ => _configurationRoot);
            serviceCollection.AddSingleton<ISecretVault, SecretConfigVault>();

            // Usage and caching of imported data
            serviceCollection.AddSingleton<IItemCache<ExchangeTransaction>, JsonCache<ExchangeTransaction>>();
            serviceCollection.AddSingleton<IItemCache<ExchangePrice>, JsonCache<ExchangePrice>>();

            serviceCollection.AddSingleton<IProcessor, FillTransactionIdProcessor>();
            serviceCollection.AddSingleton<ILedgerService, MainLedgerService>();

            // Importer sources
            serviceCollection.AddTransient<IAssetRangeImporter, BinanceRangeImporter>();
            serviceCollection.AddTransient<IAssetRangeImporter, Bl3PRangeImporter>();
            serviceCollection.AddTransient<IAssetRangeImporter, BybitRangeImporter>();
            serviceCollection.AddTransient<IAssetRangeImporter, CryptoCompareRangeImporter>();

            // Commands supported by this application
            serviceCollection.AddScoped<ICommand, LoadFromSourceCommand>(_forceUpdate);
            serviceCollection.AddScoped<ICommand, DisplayTransactionCommand>();
            serviceCollection.AddScoped<ICommand, WaitForInputCommand>();
        }

        public async Task Run()
        {
            try
            {
                // This is a program with no interactivity, so just
                // execute everything in the pre-defined order with
                // a bit of unwanted service locator anti-pattern.
                // Don't tell anyone you saw this.
                foreach (var command in _serviceProvider.GetServices<ICommand>())
                {
                    await command.Run();
                }
            }
            catch (ValidationException ex)
            {
                ex.WriteToConsole();
            }
        }
    }
}
