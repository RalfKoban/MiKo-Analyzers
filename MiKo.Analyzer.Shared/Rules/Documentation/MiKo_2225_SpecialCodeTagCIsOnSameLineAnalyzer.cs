using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2225_SpecialCodeTagCIsOnSameLineAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2225";

        public MiKo_2225_SpecialCodeTagCIsOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var node in comment.DescendantNodes<XmlElementSyntax>())
            {
                if (node.GetXmlTagName() == Constants.XmlTag.C)
                {
                    var start = node.StartTag.GetStartingLine();
                    var end = node.EndTag.GetStartingLine();

                    if (start != end)
                    {
                        yield return Issue(node);
                    }
                }
            }
        }
    }
}