using System.Diagnostics.CodeAnalysis;
using Ivtem.TSqlParsing.Feature.CompatibilityLevel;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SqlFragment;

public interface ISqlFragmentProvider
{
    bool TryGetSqlFragment(string query, [NotNullWhen(true)] out TSqlFragment? sqlFragment,
        [NotNullWhen(false)] out ParseError[]? parseErrors);

    bool TryGetSqlFragment(string query, TSqlCompatibilityLevel compatibilityLevel,
        [NotNullWhen(true)] out TSqlFragment? sqlFragment, [NotNullWhen(false)] out ParseError[]? parseErrors);
}