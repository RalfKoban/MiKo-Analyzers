using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1081_CodeFixProvider)), Shared]
    public sealed class MiKo_1081_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1081";
    }
}