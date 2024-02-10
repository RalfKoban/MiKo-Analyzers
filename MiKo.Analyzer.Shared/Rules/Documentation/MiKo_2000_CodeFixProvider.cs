using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2000_CodeFixProvider)), Shared]
    public sealed class MiKo_2000_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> XmlEntities = new Dictionary<string, string>
                                                                             {
                                                                                 { "&", "&amp;" },
                                                                             };

        public override string FixableDiagnosticId => MiKo_2000_MalformedDocumentationAnalyzer.Id;

        protected override string Title => Resources.MiKo_2000_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var token = syntax.FindToken(diagnostic);

            return XmlEntities.TryGetValue(token.Text, out var text)
                   ? syntax.ReplaceToken(token, token.WithText(text))
                   : syntax;
        }
    }
}