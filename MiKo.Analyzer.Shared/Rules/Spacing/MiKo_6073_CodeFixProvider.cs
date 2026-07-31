using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6073_CodeFixProvider)), Shared]
    public sealed class MiKo_6073_CodeFixProvider : IndendedSpacingCodeFixProvider<SyntaxNode>
    {
        public override string FixableDiagnosticId => "MiKo_6073";
    }
}