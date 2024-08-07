using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2216_CodeFixProvider)), Shared]
    public sealed class MiKo_2216_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2216";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var issue = syntax.FindNode(diagnostic.Location.SourceSpan, true, true);

            // TODO RKN: use this for bulk replace: return syntax.ReplaceNodes(elements, (original, rewritten) => GetUpdatedSyntax(rewritten));
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
    }
}