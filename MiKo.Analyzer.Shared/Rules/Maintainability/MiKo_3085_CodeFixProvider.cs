using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3085_CodeFixProvider)), Shared]
    public sealed class MiKo_3085_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        private static readonly Dictionary<SpecialType, SyntaxKind> TypeMapping = new Dictionary<SpecialType, SyntaxKind>
                                                                                      {
                                                                                          { SpecialType.System_Object, SyntaxKind.ObjectKeyword },
                                                                                          { SpecialType.System_Boolean, SyntaxKind.BoolKeyword },
                                                                                          { SpecialType.System_Char, SyntaxKind.CharKeyword },
                                                                                          { SpecialType.System_SByte, SyntaxKind.SByteKeyword },
                                                                                          { SpecialType.System_Byte, SyntaxKind.ByteKeyword },
                                                                                          { SpecialType.System_Int16, SyntaxKind.ShortKeyword },
                                                                                          { SpecialType.System_UInt16, SyntaxKind.UShortKeyword },
                                                                                          { SpecialType.System_Int32, SyntaxKind.IntKeyword },
                                                                                          { SpecialType.System_UInt32, SyntaxKind.UIntKeyword },
                                                                                          { SpecialType.System_Int64, SyntaxKind.LongKeyword },
                                                                                          { SpecialType.System_UInt64, SyntaxKind.ULongKeyword },
                                                                                          { SpecialType.System_Decimal, SyntaxKind.DecimalKeyword },
                                                                                          { SpecialType.System_Single, SyntaxKind.FloatKeyword },
                                                                                          { SpecialType.System_Double, SyntaxKind.DoubleKeyword },
                                                                                          { SpecialType.System_String, SyntaxKind.StringKeyword },
                                                                                      };

        public override string FixableDiagnosticId => "MiKo_3085";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ConditionalExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            // detect if we are an arrow
            if (syntax is ConditionalExpressionSyntax conditional)
            {
                switch (conditional.Parent)
                {
                    case ArrowExpressionClauseSyntax clause: return UpdateArrowExpressionClause(root, conditional, clause);
                    case AssignmentExpressionSyntax assignment: return UpdateAssignment(root, conditional, assignment);
                    case EqualsValueClauseSyntax clause: return UpdateEqualsValueClause(document, root, conditional, clause);
                    case ReturnStatementSyntax statement: return UpdateReturn(root, conditional, statement);
                    case ThrowStatementSyntax statement: return UpdateThrow(root, conditional, statement);
                }
            }

            return root;
        }

        private static SyntaxNode UpdateArrowExpressionClause(SyntaxNode root, ConditionalExpressionSyntax conditional, ArrowExpressionClauseSyntax arrowClause)
        {
            var ifStatement = ConvertToIfStatement(conditional, SyntaxFactory.ReturnStatement);

            switch (arrowClause.Parent)
            {
                case MethodDeclarationSyntax method:
                {
                    var updatedMethod = method.WithBody(SyntaxFactory.Block(ifStatement))
                                              .WithExpressionBody(null)
                                              .WithSemicolonToken(default);

                    return root.ReplaceNode(method, updatedMethod);
                }

                case PropertyDeclarationSyntax property:
                {
                    var getter = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, SyntaxFactory.Block(ifStatement));
                    var updatedProperty = property.WithAccessorList(SyntaxFactory.AccessorList(getter.ToSyntaxList()))
                                                  .WithExpressionBody(null)
                                                  .WithSemicolonToken(default);

                    return root.ReplaceNode(property, updatedProperty);
                }

                case AccessorDeclarationSyntax accessor:
                {
                    var updatedAccessor = accessor.WithBody(SyntaxFactory.Block(ifStatement))
                                                  .WithExpressionBody(null)
                                                  .WithSemicolonToken(default);

                    return root.ReplaceNode(accessor, updatedAccessor);
                }

                default:
                    return root;
            }
        }

        private static SyntaxNode UpdateAssignment(SyntaxNode root, ConditionalExpressionSyntax conditional, AssignmentExpressionSyntax assignment)
        {
            if (assignment.Parent is ArrowExpressionClauseSyntax clause && clause.Parent is AccessorDeclarationSyntax accessor)
            {
                var ifStatement = ConvertToIfStatement(conditional, trueCase => AssignmentStatement(assignment, trueCase), falseCase => AssignmentStatement(assignment, falseCase));

                var updatedAccessor = accessor.WithBody(SyntaxFactory.Block(ifStatement))
                                              .WithExpressionBody(null)
                                              .WithSemicolonToken(default);

                return root.ReplaceNode(accessor, updatedAccessor);
            }

            return root;
        }

        private static SyntaxNode UpdateEqualsValueClause(Document document, SyntaxNode root, ConditionalExpressionSyntax conditional, EqualsValueClauseSyntax clause)
        {
            if (clause.Parent is VariableDeclaratorSyntax declarator && declarator.Parent is VariableDeclarationSyntax declaration && declaration.Parent is LocalDeclarationStatementSyntax localDeclaration)
            {
                var typeSyntax = GetTypeSyntax(declaration, document);

                var updatedDeclarator = SyntaxFactory.VariableDeclarator(declarator.Identifier).WithTriviaFrom(declarator);
                var updatedDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, updatedDeclarator.ToSeparatedSyntaxList()).WithTriviaFrom(declaration);
                var updatedLocalDeclaration = SyntaxFactory.LocalDeclarationStatement(updatedDeclaration).WithTriviaFrom(localDeclaration);

                var ifStatement = ConvertToIfStatement(conditional, trueCase => AssignmentStatement(declarator, trueCase), falseCase => AssignmentStatement(declarator, falseCase));

                return root.ReplaceNode(localDeclaration, new SyntaxNode[] { updatedLocalDeclaration, ifStatement });
            }

            return root;
        }

        private static ExpressionStatementSyntax AssignmentStatement(AssignmentExpressionSyntax assignment, ExpressionSyntax expression)
        {
            return SyntaxFactory.ExpressionStatement(assignment.WithRight(expression));
        }

        private static ExpressionStatementSyntax AssignmentStatement(VariableDeclaratorSyntax declarator, ExpressionSyntax expression)
        {
            return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(declarator.Identifier), expression));
        }

        private static SyntaxNode UpdateReturn(SyntaxNode root, ConditionalExpressionSyntax conditional, ReturnStatementSyntax returnStatement)
        {
            var ifStatement = ConvertToIfStatement(conditional, SyntaxFactory.ReturnStatement);

            return root.ReplaceNode(returnStatement, ifStatement);
        }

        private static SyntaxNode UpdateThrow(SyntaxNode root, ConditionalExpressionSyntax conditional, ThrowStatementSyntax throwStatement)
        {
            var ifStatement = ConvertToIfStatement(conditional, SyntaxFactory.ThrowStatement);

            return root.ReplaceNode(throwStatement, ifStatement);
        }

        private static IfStatementSyntax ConvertToIfStatement(ConditionalExpressionSyntax conditional, Func<ExpressionSyntax, StatementSyntax> statementCallback)
        {
            return ConvertToIfStatement(conditional, statementCallback, statementCallback);
        }

        private static IfStatementSyntax ConvertToIfStatement(ConditionalExpressionSyntax conditional, Func<ExpressionSyntax, StatementSyntax> trueStatementCallback, Func<ExpressionSyntax, StatementSyntax> falseStatementCallback)
        {
            var condition = conditional.Condition.WithoutParenthesis().WithoutTrivia();
            var ifBlock = SyntaxFactory.Block(trueStatementCallback(conditional.WhenTrue));
            var elseBlock = SyntaxFactory.Block(falseStatementCallback(conditional.WhenFalse));

            return SyntaxFactory.IfStatement(condition, ifBlock, SyntaxFactory.ElseClause(elseBlock));
        }

        private static TypeSyntax GetTypeSyntax(VariableDeclarationSyntax declaration, Document document)
        {
            var declarationType = declaration.Type;

            if (declarationType is IdentifierNameSyntax identifier && identifier.Identifier.ValueText == "var")
            {
                var type = declarationType.GetTypeSymbol(document);

                var updatedType = TypeMapping.TryGetValue(type.SpecialType, out var kind)
                                  ? PredefinedType(kind)
                                  : SyntaxFactory.ParseTypeName(type.Name);

                return updatedType.WithTriviaFrom(declarationType);
            }

            return declarationType;
        }
    }
}