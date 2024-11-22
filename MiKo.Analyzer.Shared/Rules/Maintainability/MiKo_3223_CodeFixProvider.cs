using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3223_CodeFixProvider)), Shared]
    public sealed class MiKo_3223_CodeFixProvider : LogicalConditionsSimplifierCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3223";

        protected override SyntaxKind PredefinedTypeKind => SyntaxKind.ObjectKeyword;
    }
}