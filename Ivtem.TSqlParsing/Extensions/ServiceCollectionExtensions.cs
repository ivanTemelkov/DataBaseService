using Ivtem.TSqlParsing.Feature;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SelectQuery;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Ivtem.TSqlParsing.Feature.SqlGenerator;
using Ivtem.TSqlParsing.Feature.SqlParser;
using Microsoft.Extensions.DependencyInjection;

namespace Ivtem.TSqlParsing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlParsing(this IServiceCollection sc, string connectionString) 
        => sc.AddScoped<ITSqlParserFactory, TSqlParserFactory>()
            .AddScoped<ISqlGeneratorFactory, SqlGeneratorFactory>()
            .AddScoped<ISqlFragmentProvider, TSqlFragmentProvider>()
            .AddScoped<ISelectColumnNamesProvider, SelectColumnNamesProvider>()
            .AddScoped<ISelectQueryFieldNamesProvider, SelectQueryFieldNamesProvider>()
            .AddScoped<ISqlFragmentAndGeneratorProvider, SqlFragmentAndGeneratorProvider>()
            .AddScoped<ISqlCompatibilityLevelProvider>(_ => new SqlCompatibilityLevelProvider(connectionString));

    public static IServiceCollection AddSqlParsing(this IServiceCollection sc, Func<string> connectionStringConfig)
    {
        var connectionString = connectionStringConfig();
        return sc.AddSqlParsing(connectionString);
    }
}
