using System.CommandLine;
using Cheesebyte.HoldMyCrypto.Caching.Models;
using Cheesebyte.HoldMyCrypto.Console.Validators;
using Cheesebyte.HoldMyCrypto.Console.Extensions;
using Cheesebyte.HoldMyCrypto.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cheesebyte.HoldMyCrypto.Console
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var forceUpdateOption = new Option<bool>(
                name: "--forceUpdate",
                description: "Force retrieval of updated info from the importers.",
                getDefaultValue: () => false);

            var rootCommand = new RootCommand("Sample app for System.CommandLine");
            rootCommand.AddOption(forceUpdateOption);
            rootCommand.SetHandler(HandleStart, forceUpdateOption);

            return await rootCommand.InvokeAsync(args);
        }

        private static async Task HandleStart(bool forceUpdate)
        {
            var serviceCollection = new ServiceCollection();
            var configurationRoot = BuildConfiguration(serviceCollection);

            var application = new Application(serviceCollection, configurationRoot, forceUpdate);
            await application.Run();
        }

        private static IConfigurationRoot BuildConfiguration(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddLogging(opt => opt.AddConsole())
                .AddOptions();

            // Define the different ways to get input for this program's options
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appvault.json", true, true)
                .AddEnvironmentVariables()
                .AddUserSecretsFromAssembly();

            // Register and validate specific option objects. Currently all validated
            // on start, but should be per controller in production code to keep the
            // code that handles each option object in one place (e.g. local).
            var config = builder.Build();
            serviceCollection
                .AddValidatedOptions<JsonCacheOptions, JsonCacheValidator>(config, "JsonCache")
                .AddValidatedOptions<TransactionOptions, TransactionValidator>(config, "AssetExtract");

            return config;
        }
    }
}
