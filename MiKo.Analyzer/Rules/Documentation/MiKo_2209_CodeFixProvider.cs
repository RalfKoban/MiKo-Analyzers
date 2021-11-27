﻿using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2209_CodeFixProvider)), Shared]
    public sealed class MiKo_2209_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2209_DocumentationDoesNotUseDoublePeriodsAnalyzer.Id;

        protected override string Title => Resources.MiKo_2209_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return syntax.ReplaceTokens(
                                        syntax.DescendantTokens().Where(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken)),
                                        (original, rewritten) => original.WithText(original.Text.Replace("..", ".")));
        }
    }
}