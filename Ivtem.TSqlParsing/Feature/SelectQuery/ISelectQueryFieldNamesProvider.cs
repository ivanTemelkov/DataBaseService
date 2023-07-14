using System.Collections.Immutable;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;

namespace Ivtem.TSqlParsing.Feature.ColumnNames;

public interface ISelectQueryFieldNamesProvider
{
    ImmutableArray<string> GetFieldNames(string sql, TSqlCompatibilityLevel compatibilityLevel);
    Task<ImmutableArray<string>> GetFieldNamesAsync(string sql);
}