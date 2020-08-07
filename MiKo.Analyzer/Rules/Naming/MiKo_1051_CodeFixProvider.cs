using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1051_CodeFixProvider)), Shared]
    public sealed class MiKo_1051_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1051_DelegateParameterNameSuffixAnalyzer.Id;

        protected override string Title => "Name it '" + MiKo_1051_DelegateParameterNameSuffixAnalyzer.ExpectedName + "'";

        protected override string GetNewName(ISymbol symbol) => MiKo_1051_DelegateParameterNameSuffixAnalyzer.ExpectedName;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterSyntax>().FirstOrDefault();
    }
}