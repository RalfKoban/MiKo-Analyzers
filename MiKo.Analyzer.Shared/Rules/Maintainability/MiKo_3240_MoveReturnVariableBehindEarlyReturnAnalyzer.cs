using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3240_MoveReturnVariableBehindEarlyReturnAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3240";

        public MiKo_3240_MoveReturnVariableBehindEarlyReturnAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.GetAccessorDeclaration);

        private static bool HasIssue(LocalDeclarationStatementSyntax declaration, IfStatementSyntax ifStatement)
        {
            VariableDeclarationSyntax declarationSyntax = declaration.Declaration;

            var variables = declarationSyntax.Variables;

            if (variables.Count is 1)
            {
                var variable = variables[0];

                if (variable.Initializer is EqualsValueClauseSyntax initializer)
                {
                    var expression = initializer.Value;

                    switch (expression)
                    {
                        case PrefixUnaryExpressionSyntax u when u.Operand is LiteralExpressionSyntax:
                        case LiteralExpressionSyntax _:
                        case ObjectCreationExpressionSyntax _:
                        case ImplicitObjectCreationExpressionSyntax _:
                        case ArrayCreationExpressionSyntax _:
                        case ImplicitArrayCreationExpressionSyntax _:
                        case AnonymousObjectCreationExpressionSyntax _:
#if VS2022 || VS2026
                        case CollectionExpressionSyntax _:
#endif
                        case InvocationExpressionSyntax invocation when invocation.ArgumentList?.Arguments.Count is 0
                                                                     && invocation.GetName() is nameof(Array.Empty)
                                                                     && (invocation.GetIdentifierName() is nameof(Array) || invocation.GetIdentifierName() is nameof(Enumerable)):
                        {
                            switch (ifStatement.Statement)
                            {
                                case ReturnStatementSyntax directReturn when directReturn.Expression is IdentifierNameSyntax i && i.GetName() == variable.GetName():
                                {
                                    return true;
                                }

                                case BlockSyntax block when block.Statements is SyntaxList<StatementSyntax> statements && statements.Count is 1:
                                {
                                    return statements[0] is ReturnStatementSyntax blockReturn && blockReturn.Expression is IdentifierNameSyntax i && i.GetName() == variable.GetName();
                                }
                            }

                            break;
                        }
                    }
                }
            }

            return false;
        }

        private void AnalyzeDeclaration(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case MethodDeclarationSyntax method when method.Body is BlockSyntax body && method.ReturnType.IsVoid() is false:
                {
                    AnalyzeBlock(context, body.Statements);

                    break;
                }

                case AccessorDeclarationSyntax accessor when accessor.Body is BlockSyntax body:
                {
                    AnalyzeBlock(context, body.Statements);

                    break;
                }
            }
        }

        private void AnalyzeBlock(in SyntaxNodeAnalysisContext context, in SyntaxList<StatementSyntax> statements)
        {
            if (statements.Count >= 2 && statements[0] is LocalDeclarationStatementSyntax localDeclaration && statements[1] is IfStatementSyntax ifStatement)
            {
                if (HasIssue(localDeclaration, ifStatement))
                {
                    ReportDiagnostics(context, Issue(localDeclaration));
                }
            }
        }
    }
}