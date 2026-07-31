#if VS2022 || VS2026

using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6066_CodeFixProvider)), Shared]
    public sealed class MiKo_6066_CodeFixProvider : IndendedSpacingCodeFixProvider<ExpressionElementSyntax>
    {
        public override string FixableDiagnosticId => "MiKo_6066";
    }
}

#endif
