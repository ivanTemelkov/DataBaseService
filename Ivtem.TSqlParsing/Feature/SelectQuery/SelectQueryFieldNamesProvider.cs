using System.Collections.Immutable;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Ivtem.TSqlParsing.Feature.SqlFragment;

namespace Ivtem.TSqlParsing.Feature.ColumnNames;

public class SelectQueryFieldNamesProvider : ISelectQueryFieldNamesProvider
{
    private ISqlFragmentProvider SqlFragmentProvider { get; }
    private ISqlCompatibilityLevelProvider CompatibilityLevelProvider { get; }
    private ISelectColumnNamesProvider SelectColumnNamesProvider { get; }

    private TSqlCompatibilityLevel? CompatibilityLevel { get; set; }

    public SelectQueryFieldNamesProvider(ISqlFragmentProvider sqlFragmentProvider,
        ISqlCompatibilityLevelProvider compatibilityLevelProvider,
        ISelectColumnNamesProvider selectColumnNamesProvider)
    {
        SqlFragmentProvider = sqlFragmentProvider;
        CompatibilityLevelProvider = compatibilityLevelProvider;
        SelectColumnNamesProvider = selectColumnNamesProvider;
    }

    public ImmutableArray<string> GetFieldNames(string sql, TSqlCompatibilityLevel compatibilityLevel)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(sql));

        if (SqlFragmentProvider.TryGetSqlFragment(sql, compatibilityLevel,
                out var sqlFragment,
                out var parseErrors))
            return SelectColumnNamesProvider.GetColumnNames(sqlFragment);

        var errors = string.Join(", ", parseErrors.Select(x => x.ToString()));
        throw new InvalidOperationException($"Failed to parse sql string: {sql}! Error(s): {errors}");
    }

    public async Task<ImmutableArray<string>> GetFieldNamesAsync(string sql)
    {
        if (CompatibilityLevel.HasValue == false) CompatibilityLevel = await CompatibilityLevelProvider.GetCompatibilityLevelWithTimeout();

        return GetFieldNames(sql, CompatibilityLevel.Value);
    }
}