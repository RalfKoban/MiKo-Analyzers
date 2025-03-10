using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1504_CodeFixProvider)), Shared]
    public sealed class MiKo_1504_CodeFixProvider : PropertyNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1504";
    }
}