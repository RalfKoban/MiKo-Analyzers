using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2039_CodeFixProvider)), Shared]
    public sealed class MiKo_2039_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly string[] Parts = string.Format(Constants.Comments.ExtensionMethodClassStartingPhraseTemplate, '|').Split('|');

        private static readonly Dictionary<string, string> ReplacementMap = new Dictionary<string, string>
                                                                                {
                                                                                    { "Contains extensions for", string.Empty },
                                                                                    { "Contains extension methods for", string.Empty },
                                                                                    { "Provides extensions for", string.Empty },
                                                                                    { "Provides extension methods for", string.Empty },
                                                                                };

        public override string FixableDiagnosticId => MiKo_2039_ExtensionMethodsClassSummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2039_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var comment = PrepareComment((XmlElementSyntax)syntax);

            return CommentStartingWith(comment, Parts[0], SeeLangword("static"), Parts[1]);
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMap.Keys, ReplacementMap);
    }
}