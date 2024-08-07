using System;
using System.Composition;
using System.Text;

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
            return GetUpdatedSyntaxWithFixedText(syntax, _ => new StringBuilder(_).ReplaceAllWithCheck(Constants.Comments.NotContradictionReplacementMap.AsSpan()).ToString());
        }
    }
}