using System.Collections.Immutable;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1057_1058_CodeFixProvider)), Shared]
    public sealed class MiKo_1057_1058_CodeFixProvider : FieldNamingCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("MiKo_1057", "MiKo_1058");

        public override string FixableDiagnosticId => "MiKo_1057_1058";

        protected override string Title => Resources.MiKo_1057_CodeFixTitle;
    }
}