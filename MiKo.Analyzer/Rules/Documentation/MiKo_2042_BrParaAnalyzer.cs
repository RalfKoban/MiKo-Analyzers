using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2042_BrParaAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2042";

        public MiKo_2042_BrParaAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            foreach (SyntaxNode node in comment.DescendantNodes(_ => true, true))
            {
                var tag = GetTagName(node);

                switch (tag)
                {
                    case "br":
                        yield return Issue(symbol.Name, node, "<br/>");
                        break;

                    case "p":
                        yield return Issue(symbol.Name, node, "<p>...</p>");
                        break;
                }
            }

            string GetTagName(SyntaxNode node)
            {
                switch (node)
                {
                    case XmlEmptyElementSyntax ee: return ee.GetName();
                    case XmlElementSyntax e: return e.GetName();
                    default: return string.Empty;
                }
            }
        }
    }
}