using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2042_ParaAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2042";

        public MiKo_2042_ParaAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> results = null;

            foreach (var node in comment.DescendantNodes(_ => CodeTags.Contains(_.GetXmlTagName()) is false, true))
            {
                if (node is XmlElementStartTagSyntax || node is XmlEmptyElementSyntax)
                {
                    var tag = node.GetXmlTagName().ToLowerCase();

                    // <br/> is not an issue, see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/recommended-tags#br
                    if (tag is "p")
                    {
                        if (results is null)
                        {
                            results = new List<Diagnostic>(1);
                        }

                        results.Add(Issue(node));
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}