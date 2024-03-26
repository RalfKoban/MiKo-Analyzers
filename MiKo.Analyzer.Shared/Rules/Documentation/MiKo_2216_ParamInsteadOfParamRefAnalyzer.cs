using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2216_ParamInsteadOfParamRefAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2216";

        public MiKo_2216_ParamInsteadOfParamRefAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var element in FindProblematicElements(comment))
            {
                yield return Issue(element);
            }
        }

        private static IEnumerable<SyntaxNode> FindProblematicElements(SyntaxNode comment)
        {
            foreach (var element in comment.DescendantNodes())
            {
                if (element.Parent is XmlElementSyntax && element.IsXmlTag(Constants.XmlTag.Param))
                {
                    yield return element;
                }
            }
        }
    }
}