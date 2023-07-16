using System.Diagnostics.CodeAnalysis;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SelectQuery;

public interface ISelectQueryProvider
{
    bool TryGetStatement(TSqlFragment sqlFragment, [NotNullWhen(true)] out SelectStatement? selectStatement);
}