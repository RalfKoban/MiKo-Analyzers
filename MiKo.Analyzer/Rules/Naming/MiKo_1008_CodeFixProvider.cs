using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1008_CodeFixProvider)), Shared]
    public sealed class MiKo_1008_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzer.Id;

        protected override string Title => "Rename DependencyProperty event handler argument";

        protected override string GetNewName(ISymbol symbol) => MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzer.FindBetterName((IParameterSymbol)symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterSyntax>().FirstOrDefault();
    }
}