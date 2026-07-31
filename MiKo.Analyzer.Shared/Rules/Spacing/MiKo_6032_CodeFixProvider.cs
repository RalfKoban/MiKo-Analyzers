using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6032_CodeFixProvider)), Shared]
    public sealed class MiKo_6032_CodeFixProvider : IndendedSpacingCodeFixProvider<ParameterSyntax>
    {
        public override string FixableDiagnosticId => "MiKo_6032";
    }
}