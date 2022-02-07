using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2213_DocumentationContainsNtContradictionAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2213";

        public MiKo_2213_DocumentationContainsNtContradictionAnalyzer() : base(Id)
        {
        }

        internal static bool CommentHasIssue(string comment) => comment.ContainsAny(Constants.Comments.NotContradictionPhrase, StringComparison.OrdinalIgnoreCase);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            foreach (var token in symbol.GetDocumentationCommentTriviaSyntax().DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
            {
                foreach (var location in GetLocations(token, Constants.Comments.NotContradictionPhrase, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}