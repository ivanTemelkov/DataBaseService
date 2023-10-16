using System.Diagnostics.CodeAnalysis;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SelectQuery;

public interface IBlockCommentProvider
{
    bool TryGetBlockComments(TSqlFragment sqlFragment, [NotNullWhen(true)] out string[]? selectStatement);
}