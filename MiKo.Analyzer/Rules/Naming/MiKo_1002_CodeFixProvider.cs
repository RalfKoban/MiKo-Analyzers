using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1002_CodeFixProvider)), Shared]
    public sealed class MiKo_1002_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1002_EventHandlingMethodParametersAnalyzer.Id;

        protected override string Title => "Rename event argument";

        protected override string GetNewName(ISymbol symbol) => MiKo_1002_EventHandlingMethodParametersAnalyzer.FindBetterName((IParameterSymbol)symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterSyntax>().FirstOrDefault();
    }
}