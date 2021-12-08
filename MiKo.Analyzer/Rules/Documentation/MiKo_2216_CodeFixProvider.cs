using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2216_CodeFixProvider)), Shared]
    public sealed class MiKo_2216_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2216_ParamInsteadOfParamRefAnalyzer.Id;

        protected override string Title => Resources.MiKo_2216_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var elements = MiKo_2216_ParamInsteadOfParamRefAnalyzer.GetProblematicElements(syntax);

            var issue = elements.First(_ => _.GetLocation().Equals(diagnostic.Location));

            // TODO RKN: use this for bulk replace: return syntax.ReplaceNodes(elements, (original, rewritten) => GetUpdatedSyntax(original));
            return syntax.ReplaceNode(issue, GetUpdatedSyntax(issue));
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            switch (syntax)
            {
                case XmlElementSyntax xes:
                    return SyntaxFactory.XmlParamRefElement(GetParameterName(xes));

                case XmlEmptyElementSyntax xees:
                    return SyntaxFactory.XmlParamRefElement(GetParameterName(xees));

                default:
                    return syntax;
            }
        }

        private static string GetParameterName(XmlElementSyntax syntax) => syntax.GetAttributes<XmlNameAttributeSyntax>().First().Identifier.GetName();

        private static string GetParameterName(XmlEmptyElementSyntax syntax) => syntax.Attributes.OfType<XmlNameAttributeSyntax>().First().Identifier.GetName();
    }
}