using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1004_CodeFixProvider)), Shared]
    public sealed class MiKo_1004_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1004_EventNameSuffixAnalyzer.Id;

        protected override string Title => Resources.MiKo_1004_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1004_EventNameSuffixAnalyzer.FindBetterName(symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            foreach (var syntaxNode in syntaxNodes)
            {
                switch (syntaxNode)
                {
                    case EventDeclarationSyntax eds: return eds;
                    case VariableDeclaratorSyntax vds: return vds;
                }
            }

            return null;
        }
    }
}