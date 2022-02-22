using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2037_CodeFixProvider)), Shared]
    public sealed class MiKo_2037_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2037_CommandPropertySummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2037_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var element = (XmlElementSyntax)syntax;

            var property = syntax.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
            var commentPart = GetCommentStartPart(property);

            return CommentStartingWith(element, commentPart, SeeCref("ICommand"), MiKo_2037_CommandPropertySummaryAnalyzer.ContinuePhrase);
        }

        private static string GetCommentStartPart(PropertyDeclarationSyntax property)
        {
            var isArrowGetterOnly = property.ChildNodes<ArrowExpressionClauseSyntax>().Any();
            if (isArrowGetterOnly)
            {
                return MiKo_2037_CommandPropertySummaryAnalyzer.GetterOnlySummaryStartingPhrase;
            }

            // try to find a getter
            var getter = property.AccessorList?.FirstChild<AccessorDeclarationSyntax>(SyntaxKind.GetAccessorDeclaration);
            if (getter is null || getter.Modifiers.Any(_ => _.IsKind(SyntaxKind.PrivateKeyword)))
            {
                return MiKo_2037_CommandPropertySummaryAnalyzer.SetterOnlySummaryStartingPhrase;
            }

            var setter = property.AccessorList?.FirstChild<AccessorDeclarationSyntax>(SyntaxKind.SetAccessorDeclaration);
            if (setter is null || setter.Modifiers.Any(_ => _.IsKind(SyntaxKind.PrivateKeyword)))
            {
                return MiKo_2037_CommandPropertySummaryAnalyzer.GetterOnlySummaryStartingPhrase;
            }

            return MiKo_2037_CommandPropertySummaryAnalyzer.GetterSetterSummaryStartingPhrase;
        }
    }
}