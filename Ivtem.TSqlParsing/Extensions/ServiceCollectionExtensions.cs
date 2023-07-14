using Ivtem.TSqlParsing.Feature.ColumnNames;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SqlFragment;
using Ivtem.TSqlParsing.Feature.SQLParser;
using Microsoft.Extensions.DependencyInjection;

namespace Ivtem.TSqlParsing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlParsing(this IServiceCollection sc, string connectionString)
    {
        return sc.AddScoped<ITSqlParserFactory, TSqlParserFactory>()
            .AddScoped<ISqlScriptGeneratorFactory, SqlScriptGeneratorFactory>()
            .AddScoped<ISqlFragmentProvider, TSqlFragmentProvider>()
            .AddScoped<ISelectColumnNamesProvider, SelectColumnNamesProvider>()
            .AddScoped<ISelectQueryFieldNamesProvider, SelectQueryFieldNamesProvider>()
            .AddScoped<ISqlCompatibilityLevelProvider>(_ => new SqlCompatibilityLevelProvider(connectionString));
    }

    public static IServiceCollection AddSqlParsing(this IServiceCollection sc, Func<string> connectionStringConfig)
    {
        var connectionString = connectionStringConfig();
        return sc.AddSqlParsing(connectionString);
    }
}
