using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1115_CodeFixProvider)), Shared]
    public sealed class MiKo_1115_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1115_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer.Id;

        protected override string Title => Resources.MiKo_1115_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1115_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer.FindBetterName(symbol);

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault(_ => _.IsAnyKind(MethodKinds));
    }
}
