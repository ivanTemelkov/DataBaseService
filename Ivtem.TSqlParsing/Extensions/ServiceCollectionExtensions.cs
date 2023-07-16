using Ivtem.TSqlParsing.Feature;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SelectQuery;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Ivtem.TSqlParsing.Feature.SqlGenerator;
using Microsoft.Extensions.DependencyInjection;

namespace Ivtem.TSqlParsing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlParsing(this IServiceCollection sc, string connectionString) 
        => sc
            .AddScoped<ISqlGeneratorFactory, SqlGeneratorFactory>()
            .AddScoped<ISqlFragmentProvider, TSqlFragmentProvider>()
            .AddScoped<ISelectColumnNamesProvider, SelectColumnNamesProvider>()
            .AddScoped<ISelectQueryFieldNamesProvider, SelectQueryFieldNamesProvider>()
            .AddScoped<ISqlFragmentAndGeneratorProvider, SqlFragmentAndGeneratorProvider>()
            .AddScoped<ISelectStatementProvider, SelectStatementProvider>()
            .AddScoped<ISqlCompatibilityLevelProvider>(_ => new SqlCompatibilityLevelProvider(connectionString));

    public static IServiceCollection AddSqlParsing(this IServiceCollection sc, Func<string> connectionStringConfig)
    {
        var connectionString = connectionStringConfig();
        return sc.AddSqlParsing(connectionString);
    }
}
