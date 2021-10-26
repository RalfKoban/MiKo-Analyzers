﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2212_CodeFixProvider)), Shared]
    public sealed class MiKo_2212_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzer.Id;

        protected override string Title => Resources.MiKo_2212_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var affectedTokens = syntax.DescendantNodes().OfType<XmlTextSyntax>().SelectMany(_ => _.TextTokens).Where(_ => _.ValueText.Contains(Constants.Comments.WasNotSuccessfulPhrase)).ToList();

            return syntax.ReplaceTokens(affectedTokens, (original, rewritten) => original.WithText(GetFixedText(original.Text)));
        }

        private static string GetFixedText(string text) => text.Replace(Constants.Comments.WasNotSuccessfulPhrase, "failed");
    }
}