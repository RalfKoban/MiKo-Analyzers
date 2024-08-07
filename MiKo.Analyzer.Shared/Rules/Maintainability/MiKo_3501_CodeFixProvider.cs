using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3501_CodeFixProvider)), Shared]
    public sealed class MiKo_3501_CodeFixProvider : DoNotUseSuppressNullableWarningAnalyzerCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3501";
    }
}