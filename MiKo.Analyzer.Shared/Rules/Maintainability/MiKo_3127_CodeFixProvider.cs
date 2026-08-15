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

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ArgumentListSyntax>().FirstOrDefault();

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

                if (arguments.Count > 1)
                {
                    var illPlacedActual = FindIllPlacedActualArgument(arguments[1]);

                    if (illPlacedActual != null)
                    {
                        var illPlacedExpected = arguments[0]; // this is the actual "expected" parameter

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

        private static ArgumentSyntax FindIllPlacedActualArgument(ArgumentSyntax constraint)
        {
            var invocation = constraint.FirstDescendant<InvocationExpressionSyntax>(_ => _.Expression is MemberAccessExpressionSyntax maes && maes.GetName() is "EqualTo");

            if (invocation?.ArgumentList is ArgumentListSyntax list && list.Arguments is SeparatedSyntaxList<ArgumentSyntax> arguments && arguments.Count > 0)
            {
                return arguments[0]; // this should be the ill-placed "actual" parameter
            }

            return null;
        }
    }
}