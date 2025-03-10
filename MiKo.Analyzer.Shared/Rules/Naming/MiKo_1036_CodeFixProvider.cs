using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1036_CodeFixProvider)), Shared]
    public sealed class MiKo_1036_CodeFixProvider : FieldNamingCodeFixProvider // events are similar to fields (variable declarators)
    {
        public override string FixableDiagnosticId => "MiKo_1036";
    }
}