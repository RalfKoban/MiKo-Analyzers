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

        private static readonly SyntaxKind[] Lambdas = { SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression };

        public MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, Lambdas);
        }

        private static int CountArgumentSyntaxes(ParameterSyntax parameter)
        {
            var count = 0;

            foreach (var ancestor in parameter.Ancestors())
            {
                switch (ancestor.Kind())
                {
                    case SyntaxKind.Argument:
                    {
                        foreach (var lambda in ((ArgumentSyntax)ancestor).ChildNodes<LambdaExpressionSyntax>())
                        {
                            if (lambda is SimpleLambdaExpressionSyntax)
                            {
                                count++;

                                break;
                            }

                            if (lambda is ParenthesizedLambdaExpressionSyntax parenthesized && parenthesized.ParameterList.Parameters.Count == 1)
                            {
                                count++;

                                break;
                            }
                        }

                        break;
                    }

                    case SyntaxKind.ExpressionStatement:

                    // base methods
                    case SyntaxKind.ConversionOperatorDeclaration:
                    case SyntaxKind.ConstructorDeclaration:
                    case SyntaxKind.DestructorDeclaration:
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.OperatorDeclaration:

                    // base types
                    case SyntaxKind.RecordDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.StructDeclaration:
                    {
                        // we do not need to look up further, so we can speed up search when done project- or solution-wide
                        return count;
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
                    return Constants.LambdaIdentifiers.FallbackUnderscores2;

                case 3:
                    return Constants.LambdaIdentifiers.FallbackUnderscores3;

                case 4:
                    return Constants.LambdaIdentifiers.FallbackUnderscores4;

                case 5:
                    return Constants.LambdaIdentifiers.Fallback4;

                case 6:
                    return Constants.LambdaIdentifiers.Fallback5;

                default:
                    return Constants.LambdaIdentifiers.Default + (count - 1);
            }
        }

        private void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var issue = AnalyzeLambdaExpression(context.Node);

            if (issue != null)
            {
                ReportDiagnostics(context, issue);
            }
        }

        private Diagnostic AnalyzeLambdaExpression(SyntaxNode node)
        {
            switch (node)
            {
                case SimpleLambdaExpressionSyntax simple:
                {
                    return AnalyzeParameter(simple.Parameter);
                }

                case ParenthesizedLambdaExpressionSyntax parenthesized:
                {
                    var parameters = parenthesized.ParameterList.Parameters;

                    if (parameters.Count == 1)
                    {
                        return AnalyzeParameter(parameters[0]);
                    }

                    return null;
                }
            }

            return null;
        }

        private Diagnostic AnalyzeParameter(ParameterSyntax parameter)
        {
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
                case Constants.LambdaIdentifiers.FallbackUnderscores2: // correct identifier (fallback as there is already another identifier in the parent lambda expression)
                case Constants.LambdaIdentifiers.FallbackUnderscores3: // correct identifier (fallback as there is already another identifier in the parent lambda expression)
                case Constants.LambdaIdentifiers.FallbackUnderscores4: // correct identifier (fallback as there is already another identifier in the parent lambda expression)
                case Constants.LambdaIdentifiers.Fallback0: // correct identifier (fallback as there is already another identifier in the parent lambda expression)
                case Constants.LambdaIdentifiers.Fallback1: // correct identifier (fallback as there is already another identifier in the parent lambda expression)
                case Constants.LambdaIdentifiers.Fallback2: // correct identifier (2nd fallback as there is already another identifier in the parent lambda expression)
                case Constants.LambdaIdentifiers.Fallback3: // correct identifier (3rd fallback as there is already another identifier in the parent lambda expression)
                case Constants.LambdaIdentifiers.Fallback4: // correct identifier (4th fallback as there is already another identifier in the parent lambda expression)
                case Constants.LambdaIdentifiers.Fallback5: // correct identifier (5th fallback as there is already another identifier in the parent lambda expression)
                case "failed": // result in ASP .NET core to indicate a failure
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