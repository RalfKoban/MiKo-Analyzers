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

        protected override string Title => "Apply default comment to command property";

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var element = (XmlElementSyntax)syntax;

            var property = syntax.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().First();
            var commentParts = GetCommentParts(property);
            var command = SyntaxFactory.ParseTypeName("ICommand");

            return StartCommentWith(element, commentParts[0], SeeCref(command), commentParts[1]);
        }

        private static string[] GetCommentParts(PropertyDeclarationSyntax property)
        {
            var isArrowGetterOnly = property.ChildNodes().OfType<ArrowExpressionClauseSyntax>().Any();
            if (isArrowGetterOnly)
            {
                return string.Format(Constants.Comments.CommandPropertyGetterOnlySummaryStartingPhraseTemplate, '|').Split('|');
            }

            // try to find a getter
            var getter = (AccessorDeclarationSyntax)property.AccessorList?.ChildNodes().FirstOrDefault(_ => _.IsKind(SyntaxKind.GetAccessorDeclaration));
            var setter = (AccessorDeclarationSyntax)property.AccessorList?.ChildNodes().FirstOrDefault(_ => _.IsKind(SyntaxKind.SetAccessorDeclaration));

            if (getter is null || getter.Modifiers.Any(_ => _.IsKind(SyntaxKind.PrivateKeyword)))
            {
                return string.Format(Constants.Comments.CommandPropertySetterOnlySummaryStartingPhraseTemplate, '|').Split('|');
            }

            if (setter is null || setter.Modifiers.Any(_ => _.IsKind(SyntaxKind.PrivateKeyword)))
            {
                return string.Format(Constants.Comments.CommandPropertyGetterOnlySummaryStartingPhraseTemplate, '|').Split('|');
            }

            return string.Format(Constants.Comments.CommandPropertyGetterSetterSummaryStartingPhraseTemplate, '|').Split('|');
        }
    }
}