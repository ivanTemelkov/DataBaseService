using System.Collections.Immutable;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SelectQuery;

public interface ISelectColumnNamesProvider
{
    ImmutableArray<string> GetColumnNames(TSqlFragment sqlFragment);
}