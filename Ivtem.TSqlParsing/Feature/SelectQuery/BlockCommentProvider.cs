using System.Diagnostics.CodeAnalysis;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Ivtem.TSqlParsing.Feature.SelectQuery;

public class BlockCommentProvider : IBlockCommentProvider
{
    public bool TryGetBlockComments(TSqlFragment sqlFragment, [NotNullWhen(true)] out string[]? commentBlocks)
    {
        var result = new List<string>();

        foreach (var token in sqlFragment.ScriptTokenStream)
        {
            if (token.TokenType == TSqlTokenType.MultilineComment)
            {
                result.Add(token.Text);
            }
        }

        commentBlocks = result.Count == 0 ? null : result.ToArray();

        return commentBlocks != null;
    }
}