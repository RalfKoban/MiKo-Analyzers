using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2070_CodeFixProvider)), Shared]
    public sealed class MiKo_2070_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2070_ReturnsSummaryAnalyzer.Id;

        protected override string Title => "Replace 'Return' in comment";

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var summary = (XmlElementSyntax)syntax;

            var text = summary.Content.OfType<XmlTextSyntax>().First();

            foreach (var token in text.TextTokens)
            {
                var valueText = token.WithoutTrivia().ValueText.Without(Constants.Comments.AsynchrounouslyStartingPhrase).Trim();

                if (valueText.StartsWithAny(MiKo_2070_ReturnsSummaryAnalyzer.Phrases))
                {
                    var startText = GetCorrectStartText(summary);
                    var newText = " " + startText + valueText.WithoutFirstWord();
                    var newToken = SyntaxFactory.Token(token.LeadingTrivia, token.Kind(), newText, newText, token.TrailingTrivia);

                    return summary.ReplaceToken(token, newToken);
                }
            }

            return summary;
        }

        private static string GetCorrectStartText(XmlElementSyntax element)
        {
            foreach (var ancestor in element.AncestorsAndSelf())
            {
                switch (ancestor)
                {
                    case PropertyDeclarationSyntax p:
                        return GetCorrectStartText(p);

                    case MethodDeclarationSyntax m:
                        return GetCorrectStartText(m);
                }
            }

            return string.Empty;
        }

        private static string GetCorrectStartText(BasePropertyDeclarationSyntax property)
        {
            var isBool = property.Type.IsBoolean();
            var isAsync = property.Modifiers.Any(_ => _.IsKind(SyntaxKind.AsyncKeyword));

            if (property.Type is GenericNameSyntax g && g.Identifier.ValueText == nameof(Task))
            {
                isAsync = true;
                isBool = g.TypeArgumentList.Arguments.Count > 0 && g.TypeArgumentList.Arguments[0].IsBoolean();
            }

            var startText = isBool ? "Gets a value indicating whether" : "Gets";
            if (isAsync)
            {
                return Constants.Comments.AsynchrounouslyStartingPhrase + startText.ToLowerCaseAt(0);
            }

            return startText;
        }

        private static string GetCorrectStartText(MethodDeclarationSyntax method)
        {
            var isBool = method.ReturnType.IsBoolean();
            var isAsync = method.Modifiers.Any(_ => _.IsKind(SyntaxKind.AsyncKeyword));

            if (method.ReturnType is GenericNameSyntax g && g.Identifier.ValueText == nameof(Task))
            {
                isAsync = true;
                isBool = g.TypeArgumentList.Arguments.Count > 0 && g.TypeArgumentList.Arguments[0].IsBoolean();
            }

            var startText = isBool ? "Determines whether" : "Gets";
            if (isAsync)
            {
                return Constants.Comments.AsynchrounouslyStartingPhrase + startText.ToLowerCaseAt(0);
            }

            return startText;
        }
    }
}