using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1300_CodeFixProvider)), Shared]
    public sealed class MiKo_1300_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1300";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var syntaxNode in syntaxNodes)
            {
                switch (syntaxNode)
                {
                    case SimpleLambdaExpressionSyntax simple:
                        return simple.Parameter;

                    case ParenthesizedLambdaExpressionSyntax parenthesized:
                        return parenthesized.ParameterList.Parameters[0];
                }
            }

            return null;
        }
    }
}