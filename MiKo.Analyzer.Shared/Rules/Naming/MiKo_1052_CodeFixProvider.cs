using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1052_CodeFixProvider)), Shared]
    public sealed class MiKo_1052_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1052";

        protected override string Title => Resources.MiKo_1052_CodeFixTitle;
    }
}