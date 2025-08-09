using LocalSearcher.Api.Common.Configs;
using LocalSearcher.Api.Common.Factories;
using LocalSearcher.Api.Common.Interfaces;
using LocalSearcher.Api.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace LocalSearcher.Api.Utils;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SearchConfig>(configuration.GetSection("SearchConfig"));
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddControllers();
        services.AddCors();
        
        return services
            .ConfigureConfig(configuration)
            .ConfigureRedis(configuration)
            .ConfigureSerilog(configuration)
            .RegisterServices();
    }

    private static IServiceCollection ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddStackExchangeRedisCache(options => {
            var redisConnection = configuration["ConnectionStrings:Redis"] 
                                  ?? "localhost:6379";
            
            options.Configuration = redisConnection;
            options.InstanceName = "LocalSearcher:";
        });
    }

    private static IServiceCollection ConfigureSerilog(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateBootstrapLogger();
        
        return services;
    }

    private static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        return services
            .AddHostedService<IndexBackgroundService>()
            .AddSingleton<ITfIdfIndexStorageService, TfIdfRedisCacheService>()
            .AddSingleton<ITokenizerFactory, TokenizersFactory>()
            .AddScoped<IIndexService, TfIdfIndexService>()
            .AddScoped<ISearchService, TfIdfSearchService>();
    }

    private static IServiceCollection ConfigureConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var config = new ApplicationGLobalConfig
        {
            RedisConnectionString = configuration["ConnectionStrings:Redis"]!,
            SeqConnectionUrl = configuration["ConnectionStrings:SeqUrl"]!,
            LogsDirectoryPath = configuration["LogsFolder"]!,
            SearchBaseDirectoryPath = configuration["SearchConfig:Directory"]!
        };
        
        var env = services.BuildServiceProvider().GetRequiredService<IHostEnvironment>();

        if (string.IsNullOrWhiteSpace(config.RedisConnectionString))
            throw new InvalidOperationException("RedisConnectionString is required.");

        if (env.IsProduction() && string.IsNullOrWhiteSpace(config.SeqConnectionUrl))
            throw new InvalidOperationException("SeqConnectionUrl is required in production.");

        if (string.IsNullOrWhiteSpace(config.LogsDirectoryPath))
            throw new InvalidOperationException("LogsDirectoryPath is required.");

        if (string.IsNullOrWhiteSpace(config.SearchBaseDirectoryPath))
            throw new InvalidOperationException("SearchBaseDirectoryPath is required.");

        services.AddSingleton(Options.Create(config));

        return services;
    }
}