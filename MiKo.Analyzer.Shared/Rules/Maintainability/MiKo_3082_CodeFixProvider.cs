using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3082_CodeFixProvider)), Shared]
    public sealed class MiKo_3082_CodeFixProvider : UsePatternMatchingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3082";

        protected override string Title => Resources.MiKo_3082_CodeFixTitle;
    }
}