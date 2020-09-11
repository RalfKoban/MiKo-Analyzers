using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1108_CodeFixProvider)), Shared]
    public sealed class MiKo_1108_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1108_MockNamingAnalyzer.Id;

        protected override string Title => Resources.MiKo_1108_CodeFixTitle;

        protected override string GetNewName(ISymbol symbol) => MiKo_1108_MockNamingAnalyzer.FindBetterName(symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            var syntax = base.GetSyntax(syntaxNodes);
            if (syntax == null)
            {
                foreach (var syntaxNode in syntaxNodes)
                {
                    switch (syntaxNode.Kind())
                    {
                        case SyntaxKind.Parameter:
                        case SyntaxKind.PropertyDeclaration:
                        {
                            return syntaxNode;
                        }
                    }
                }
            }

            return syntax;
        }
    }
}