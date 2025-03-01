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

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol)
        {
            List<Diagnostic> results = null;

            foreach (var node in comment.DescendantNodes(_ => CodeTags.Contains(_.GetXmlTagName()) is false, true))
            {
                if (node is XmlElementStartTagSyntax || node is XmlEmptyElementSyntax)
                {
                    var tag = node.GetXmlTagName().ToLowerCase();

                    switch (tag)
                    {
                        case "br":
                            if (results is null)
                            {
                                results = new List<Diagnostic>();
                            }

                            results.Add(Issue(node, "<br/>"));

                            break;

                        case "p":
                            if (results is null)
                            {
                                results = new List<Diagnostic>();
                            }

                            results.Add(Issue(node, "<p>...</p>"));

                            break;
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}