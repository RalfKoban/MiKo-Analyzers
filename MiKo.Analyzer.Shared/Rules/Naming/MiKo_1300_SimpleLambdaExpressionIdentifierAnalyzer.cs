using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1300";

        public MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSimpleLambdaExpression, SyntaxKind.SimpleLambdaExpression);
        }

        private static int CountArgumentSyntaxes(ParameterSyntax parameter)
        {
            var count = 0;

            foreach (var ancestor in parameter.Ancestors())
            {
                switch (ancestor)
                {
                    case ArgumentSyntax a:
                    {
                        if (a.ChildNodes<SimpleLambdaExpressionSyntax>().Any())
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

        private static string FindBetterName(ParameterSyntax parameter)
        {
            // find argument candidates to see how long the default identifier shall become (note that the own parent is included)
            var count = CountArgumentSyntaxes(parameter);

            switch (count)
            {
                case 0:
                case 1:
                    return Constants.LambdaIdentifiers.Default;

                case 2:
                    return Constants.LambdaIdentifiers.Fallback;

                case 3:
                    return Constants.LambdaIdentifiers.Fallback2;

                case 4:
                    return Constants.LambdaIdentifiers.Fallback3;

                default:
                    return string.Concat(Enumerable.Repeat(Constants.LambdaIdentifiers.Default, count));
            }
        }

        private void AnalyzeSimpleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (SimpleLambdaExpressionSyntax)context.Node;
            var issue = AnalyzeSimpleLambdaExpression(node);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            var parameter = node.Parameter;

            if (parameter is null)
            {
                return null;
            }

            var identifier = parameter.Identifier;
            var parameterName = parameter.GetName();

            switch (parameterName)
            {
                case null: // we do not have one
                case Constants.LambdaIdentifiers.Default: // correct identifier (default one)
                case Constants.LambdaIdentifiers.Fallback: // correct identifier (fallback as there is already another identifier in the parent lambda expression)
                case Constants.LambdaIdentifiers.Fallback2: // correct identifier (2nd fallback as there is already another identifier in the parent lambda expression)
                case Constants.LambdaIdentifiers.Fallback3: // correct identifier (3rd fallback as there is already another identifier in the parent lambda expression)
                    return null;

                default:
                {
                    var proposal = FindBetterName(parameter);

                    return Issue(parameterName, identifier, proposal, CreateBetterNameProposal(proposal));
                }
            }
        }
    }
}