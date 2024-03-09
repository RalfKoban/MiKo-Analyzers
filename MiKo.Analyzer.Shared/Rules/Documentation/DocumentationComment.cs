using System;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    internal static class DocumentationComment
    {
        internal static bool ContainsNotContradiction(ReadOnlySpan<char> comment) => comment.ContainsAny(Constants.Comments.NotContradictionPhrase, StringComparison.OrdinalIgnoreCase);
    }
}