using System;
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
        private static readonly string[] GetOnly = Constants.Comments.CommandPropertyGetterOnlySummaryStartingPhraseTemplate.FormatWith('|').Split('|');
        private static readonly string[] SetOnly = Constants.Comments.CommandPropertySetterOnlySummaryStartingPhraseTemplate.FormatWith('|').Split('|');
        private static readonly string[] GetSet = Constants.Comments.CommandPropertyGetterSetterSummaryStartingPhraseTemplate.FormatWith('|').Split('|');

        public override string FixableDiagnosticId => MiKo_2037_CommandPropertySummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2037_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var element = (XmlElementSyntax)syntax;

            var property = syntax.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
            var commentParts = GetCommentParts(property);

            return CommentStartingWith(element, commentParts[0], SeeCref("ICommand"), commentParts[1]);
        }

        private static string[] GetCommentParts(PropertyDeclarationSyntax property)
        {
            var isArrowGetterOnly = property.ChildNodes<ArrowExpressionClauseSyntax>().Any();

            if (isArrowGetterOnly)
            {
                return GetOnly;
            }

            // try to find a getter
            var getter = property.AccessorList?.FirstChild<AccessorDeclarationSyntax>(SyntaxKind.GetAccessorDeclaration);

            if (getter is null || getter.Modifiers.Any(_ => _.IsKind(SyntaxKind.PrivateKeyword)))
            {
                return SetOnly;
            }

            var setter = property.AccessorList?.FirstChild<AccessorDeclarationSyntax>(SyntaxKind.SetAccessorDeclaration);

            if (setter is null || setter.Modifiers.Any(_ => _.IsKind(SyntaxKind.PrivateKeyword)))
            {
                return GetOnly;
            }

            return GetSet;
        }
    }
}