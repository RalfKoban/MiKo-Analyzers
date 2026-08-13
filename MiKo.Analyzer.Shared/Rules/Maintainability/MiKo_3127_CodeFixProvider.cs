using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3127_CodeFixProvider)), Shared]
    public sealed class MiKo_3127_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3127";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            return syntaxNodes.OfType<ArgumentListSyntax>().First();
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is ArgumentListSyntax list)
            {
                var arguments = list.Arguments;

                if (arguments.Count > 1 && arguments[1].Expression is InvocationExpressionSyntax comparison)
                {
                    var comparisonArguments = comparison.ArgumentList.Arguments;
                    if (comparisonArguments.Any())
                    {
                        var illPlacedExpected = arguments[0]; // this is the actual "expected" parameter
                        var illPlacedActual = comparisonArguments.First(); // this is the actual "actual" parameter

                        var fixedActual = illPlacedActual.WithTriviaFrom(illPlacedExpected);
                        var fixedExpected = illPlacedExpected.WithoutTrivia();

                        var illPlacedNodes = new[] { illPlacedExpected, illPlacedActual };

                        return list.ReplaceNodes(
                                             illPlacedNodes,
                                             (o, r) =>
                                                      {
                                                          if (r.IsEquivalentTo(illPlacedExpected))
                                                          {
                                                              return fixedActual;
                                                          }

                                                          if (r.IsEquivalentTo(illPlacedActual))
                                                          {
                                                              return fixedExpected;
                                                          }

                                                          return r;
                                                      });
                    }
                }
            }

            return syntax;
        }
    }
}