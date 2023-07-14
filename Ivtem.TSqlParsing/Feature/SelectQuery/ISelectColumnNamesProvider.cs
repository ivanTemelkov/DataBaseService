using System.Collections.Immutable;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.ColumnNames;

public interface ISelectColumnNamesProvider
{
    ImmutableArray<string> GetColumnNames(TSqlFragment sqlFragment);
}