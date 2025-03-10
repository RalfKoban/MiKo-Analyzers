using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> results = null;

            foreach (var node in comment.DescendantNodes(_ => CodeTags.Contains(_.GetXmlTagName()) is false, true))
            {
                if (node is XmlElementStartTagSyntax || node is XmlEmptyElementSyntax)
                {
                    var issue = FindIssue(node);

                    if (issue is null)
                    {
                        continue;
                    }

                    if (results is null)
                    {
                        results = new List<Diagnostic>(1);
                    }

                    results.Add(issue);
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }

        private Diagnostic FindIssue(SyntaxNode node)
        {
            var tag = node.GetXmlTagName().ToLowerCase();

            switch (tag)
            {
                case "br":
                    return Issue(node, "<br/>");

                case "p":
                    return Issue(node, "<p>...</p>");

                default:
                    return null;
            }
        }
    }
}