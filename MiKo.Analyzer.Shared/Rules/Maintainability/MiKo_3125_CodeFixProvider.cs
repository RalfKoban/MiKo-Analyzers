using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3125_CodeFixProvider)), Shared]
    public sealed class MiKo_3125_CodeFixProvider : AttributeSyntaxCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3125";
    }
}