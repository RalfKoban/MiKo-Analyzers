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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var node in comment.DescendantNodes(_ => CodeTags.Contains(_.GetXmlTagName()) is false, true))
            {
                if (node is XmlElementStartTagSyntax || node is XmlEmptyElementSyntax)
                {
                    var tag = node.GetXmlTagName().ToLowerCase();

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
            }
        }
    }
}