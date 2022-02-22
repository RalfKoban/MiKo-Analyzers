using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3062_CodeFixProvider)), Shared]
    public sealed class MiKo_3062_CodeFixProvider : StringMaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3062_ExceptionLogMessageEndsWithColonAnalyzer.Id;

        protected override string Title => Resources.MiKo_3062_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntaxWithTextEnding(syntax, ":");
    }
}