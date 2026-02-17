using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3064_CodeFixProvider)), Shared]
    public sealed class MiKo_3064_CodeFixProvider : StringMaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3064";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax) => GetUpdatedSyntaxWithFixedText(syntax, _ => _.AsCachedBuilder().ReplaceAllWithProbe(Constants.Comments.NotContractionReplacementMap.AsSpan()).ToStringAndRelease());
    }
}