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
            .AddTransient<ISqlGeneratorFactory, SqlGeneratorFactory>()
            .AddTransient<ISqlFragmentProvider, TSqlFragmentProvider>()
            .AddTransient<ISelectColumnNamesProvider, SelectColumnNamesProvider>()
            .AddTransient<ISelectQueryFieldNamesProvider, SelectQueryFieldNamesProvider>()
            .AddTransient<ISqlFragmentAndGeneratorProvider, SqlFragmentAndGeneratorProvider>()
            .AddTransient<ISelectStatementProvider, SelectStatementProvider>()
            .AddTransient<ISqlCompatibilityLevelProvider>(_ => new SqlCompatibilityLevelProvider(connectionString));

    public static IServiceCollection AddSqlParsing(this IServiceCollection sc, Func<string> connectionStringConfig)
    {
        var connectionString = connectionStringConfig();
        return sc.AddSqlParsing(connectionString);
    }

    public static IServiceCollection AddSqlParsing(this IServiceCollection sc, Func<ISqlCompatibilityLevelProvider> getCompatibilityLevelProvider)
        => sc
            .AddTransient<ISqlGeneratorFactory, SqlGeneratorFactory>()
            .AddTransient<ISqlFragmentProvider, TSqlFragmentProvider>()
            .AddTransient<ISelectColumnNamesProvider, SelectColumnNamesProvider>()
            .AddTransient<ISelectQueryFieldNamesProvider, SelectQueryFieldNamesProvider>()
            .AddTransient<ISqlFragmentAndGeneratorProvider, SqlFragmentAndGeneratorProvider>()
            .AddTransient<ISelectStatementProvider, SelectStatementProvider>()
            .AddTransient<ISqlCompatibilityLevelProvider>(_ => getCompatibilityLevelProvider());
}
