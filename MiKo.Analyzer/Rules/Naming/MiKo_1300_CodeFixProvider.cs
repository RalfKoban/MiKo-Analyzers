using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1300_CodeFixProvider)), Shared]
    public sealed class MiKo_1300_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer.Id;

        protected override string Title => Resources.MiKo_1300_CodeFixTitle;

        protected override string GetNewName(ISymbol symbol)
        {
            // find argument candidates to see how long the default identifier shall become (note that the own parent is included)
            var count = CountArgumentSyntaxes(((IParameterSymbol)symbol).GetSyntax());
            switch (count)
            {
                case 0:
                case 1:
                    return Constants.LambdaIdentifiers.Default;

                case 2:
                    return Constants.LambdaIdentifiers.Fallback;

                case 3:
                    return Constants.LambdaIdentifiers.Fallback2;

                default:
                    return string.Concat(Enumerable.Repeat(Constants.LambdaIdentifiers.Default, count));
            }
        }

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<SimpleLambdaExpressionSyntax>().First().Parameter;

        private static int CountArgumentSyntaxes(ParameterSyntax parameter)
        {
            var count = 0;

            foreach (var ancestor in parameter.Ancestors())
            {
                switch (ancestor)
                {
                    case ArgumentSyntax a:
                    {
                        if (a.ChildNodes().OfType<SimpleLambdaExpressionSyntax>().Any())
                        {
                            count++;
                        }

                        break;
                    }

                    case ExpressionStatementSyntax _:
                    case MethodDeclarationSyntax _:
                    {
                        // we do not need to look up further, so we can speed up search when done project- or solution-wide
                        break;
                    }
                }
            }

            return count;
        }
    }
}