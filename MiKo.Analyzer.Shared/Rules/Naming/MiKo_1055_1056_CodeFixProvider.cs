using System.Collections.Immutable;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1055_1056_CodeFixProvider)), Shared]
    public sealed class MiKo_1055_1056_CodeFixProvider : FieldNamingCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("MiKo_1055", "MiKo_1056");

        public override string FixableDiagnosticId => "MiKo_1055_1056";

        protected override string Title => Resources.MiKo_1055_CodeFixTitle;
    }
}