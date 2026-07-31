using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6058_CodeFixProvider)), Shared]
    public sealed class MiKo_6058_CodeFixProvider : IndendedSpacingCodeFixProvider<TypeParameterConstraintClauseSyntax>
    {
        public override string FixableDiagnosticId => "MiKo_6058";
    }
}