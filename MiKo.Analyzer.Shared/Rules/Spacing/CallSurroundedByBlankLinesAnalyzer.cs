using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class CallSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        protected CallSurroundedByBlankLinesAnalyzer(string id) : base(id)
        {
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        protected abstract bool IsCall(ITypeSymbol type);

        protected virtual bool IsCall(MemberAccessExpressionSyntax syntax, SemanticModel semanticModel)
        {
            var type = syntax.GetTypeSymbol(semanticModel);

            return type != null && IsCall(type);
        }

        protected virtual bool IsAlsoCall(MemberAccessExpressionSyntax syntax, SemanticModel semanticModel) => IsCall(syntax, semanticModel);

        private bool IsAlsoCall(StatementSyntax statement, SemanticModel semanticModel) => statement is ExpressionStatementSyntax e && IsAlsoCall(e, semanticModel);

        private bool IsAlsoCall(ExpressionStatementSyntax statement, SemanticModel semanticModel) => statement.Expression is InvocationExpressionSyntax i && IsAlsoCall(i, semanticModel);

        private bool IsAlsoCall(InvocationExpressionSyntax invocation, SemanticModel semanticModel) => invocation.Expression is MemberAccessExpressionSyntax call && IsAlsoCall(call, semanticModel);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;
            var issue = AnalyzeSimpleMemberAccessExpression(node, context.SemanticModel);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeSimpleMemberAccessExpression(MemberAccessExpressionSyntax syntax, SemanticModel semanticModel)
        {
            if (IsCall(syntax, semanticModel))
            {
                foreach (var ancestor in syntax.Ancestors())
                {
                    switch (ancestor.Kind())
                    {
                        case SyntaxKind.Block:
                            return AnalyzeSimpleMemberAccessExpression(((BlockSyntax)ancestor).Statements, syntax, semanticModel);

                        case SyntaxKind.SwitchSection:
                            return AnalyzeSimpleMemberAccessExpression(((SwitchSectionSyntax)ancestor).Statements, syntax, semanticModel);

                        case SyntaxKind.IfStatement:
                        {
                            var statement = (IfStatementSyntax)ancestor;

                            if (statement == syntax.Parent)
                            {
                                continue; // let's look into the surrounding block because the call is the condition of the 'if' statement
                            }

                            return null;
                        }

                        case SyntaxKind.ElseClause:
                        case SyntaxKind.CaseSwitchLabel:
                        case SyntaxKind.ArrowExpressionClause:
                        case SyntaxKind.LocalFunctionStatement:
                            return null; // stop lookup as there is no valid ancestor anymore

                        // base methods
                        case SyntaxKind.ConversionOperatorDeclaration:
                        case SyntaxKind.ConstructorDeclaration:
                        case SyntaxKind.DestructorDeclaration:
                        case SyntaxKind.MethodDeclaration:
                        case SyntaxKind.OperatorDeclaration:
                            return null; // stop lookup as there is no valid ancestor anymore

                        // base types
                        case SyntaxKind.RecordDeclaration:
                        case SyntaxKind.ClassDeclaration:
                        case SyntaxKind.InterfaceDeclaration:
                        case SyntaxKind.StructDeclaration:
                            return null; // stop lookup as there is no valid ancestor anymore

                        // initializers
                        case SyntaxKind.ObjectInitializerExpression:
                        case SyntaxKind.CollectionInitializerExpression:
                        case SyntaxKind.ArrayInitializerExpression:
                        case SyntaxKind.ComplexElementInitializerExpression:
                        case SyntaxKind.WithInitializerExpression:
                            return null; // stop lookup as there is no valid ancestor anymore

                        // lambdas
                        case SyntaxKind.SimpleLambdaExpression:
                        case SyntaxKind.ParenthesizedLambdaExpression:
                            return null; // stop lookup as there is no valid ancestor anymore
                    }
                }
            }

            return null;
        }

        private Diagnostic AnalyzeSimpleMemberAccessExpression(SyntaxList<StatementSyntax> statements, MemberAccessExpressionSyntax call, SemanticModel semanticModel)
        {
            var callLineSpan = call.GetLocation().GetLineSpan();

            var noBlankLinesBefore = statements.Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                               .Any(_ => IsAlsoCall(_, semanticModel) is false);

            var noBlankLinesAfter = statements.Where(_ => HasNoBlankLinesAfter(callLineSpan, _))
                                              .Any(_ => IsAlsoCall(_, semanticModel) is false);

            if (noBlankLinesBefore || noBlankLinesAfter)
            {
                return Issue(call, noBlankLinesBefore, noBlankLinesAfter);
            }

            return null;
        }
    }
}