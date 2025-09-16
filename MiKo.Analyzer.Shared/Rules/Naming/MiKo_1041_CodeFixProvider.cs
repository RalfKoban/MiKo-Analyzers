using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1041_CodeFixProvider)), Shared]
    public sealed class MiKo_1041_CodeFixProvider : FieldNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1041";
    }
}