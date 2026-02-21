using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2037_CodeFixProvider)), Shared]
    public sealed class MiKo_2037_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly string[] GetOnly = Constants.Comments.CommandPropertyGetterOnlySummaryStartingPhraseTemplate.FormatWith("|").Split('|');
        private static readonly string[] SetOnly = Constants.Comments.CommandPropertySetterOnlySummaryStartingPhraseTemplate.FormatWith("|").Split('|');
        private static readonly string[] GetSet = Constants.Comments.CommandPropertyGetterSetterSummaryStartingPhraseTemplate.FormatWith("|").Split('|');

        public override string FixableDiagnosticId => "MiKo_2037";

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax((XmlElementSyntax)syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static XmlElementSyntax GetUpdatedSyntax(XmlElementSyntax element)
        {
            var property = element.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
            var commentParts = GetCommentParts(property);

            return CommentStartingWith(element, commentParts[0], SeeCref("ICommand"), commentParts[1]);
        }

        private static string[] GetCommentParts(PropertyDeclarationSyntax property)
        {
            if (property.ExpressionBody != null)
            {
                return GetOnly;
            }

            // try to find a getter
            var getter = property.GetGetter();

            if (getter is null || getter.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                return SetOnly;
            }

            var setter = property.GetSetter();

            if (setter is null || setter.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                return GetOnly;
            }

            return GetSet;
        }
    }
}