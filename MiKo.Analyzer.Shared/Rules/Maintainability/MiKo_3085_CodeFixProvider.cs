﻿using System;
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
                    case ArgumentSyntax argument: return UpdateArgument(document, root, conditional, argument);
                    case AssignmentExpressionSyntax assignment: return UpdateAssignment(root, conditional, assignment);
                    case EqualsValueClauseSyntax clause: return UpdateEqualsValueClause(document, root, conditional, clause);
                    case ParenthesizedExpressionSyntax parenthesized: return UpdateParenthesized(root, conditional, parenthesized);
                    case ReturnStatementSyntax statement: return UpdateReturn(root, conditional, statement);
                    case ThrowStatementSyntax statement: return UpdateThrow(root, conditional, statement);
                }
            }

            return root;
        }

        private static SyntaxNode UpdateArrowExpressionClause(SyntaxNode root, ConditionalExpressionSyntax conditional, ArrowExpressionClauseSyntax arrowClause)
        {
            var ifStatement = ConvertToIfStatement(conditional, SyntaxFactory.ReturnStatement);

            return UpdateArrowExpressionClause(root, arrowClause, SyntaxFactory.Block(ifStatement));
        }

        private static SyntaxNode UpdateArrowExpressionClause(SyntaxNode root, ArrowExpressionClauseSyntax arrowClause, BlockSyntax block)
        {
            switch (arrowClause.Parent)
            {
                case MethodDeclarationSyntax method:
                {
                    var updatedMethod = method.WithBody(block)
                                              .WithExpressionBody(null)
                                              .WithSemicolonToken(default);

                    return root.ReplaceNode(method, updatedMethod);
                }

                case PropertyDeclarationSyntax property:
                {
                    var getter = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, block);
                    var updatedProperty = property.WithAccessorList(SyntaxFactory.AccessorList(getter.ToSyntaxList()))
                                                  .WithExpressionBody(null)
                                                  .WithSemicolonToken(default);

                    return root.ReplaceNode(property, updatedProperty);
                }

                case AccessorDeclarationSyntax accessor:
                {
                    var updatedAccessor = accessor.WithBody(block)
                                                  .WithExpressionBody(null)
                                                  .WithSemicolonToken(default);

                    return root.ReplaceNode(accessor, updatedAccessor);
                }

                default:
                    return root;
            }
        }

        private static SyntaxNode UpdateArgument(Document document, SyntaxNode root, ConditionalExpressionSyntax conditional, ArgumentSyntax argument)
        {
            if (argument.Parent is ArgumentListSyntax argumentList && argumentList.Parent is InvocationExpressionSyntax invocation)
            {
                var symbol = invocation.GetSymbol(document);

                if (symbol is IMethodSymbol methodSymbol)
                {
                    var argumentIndex = argumentList.Arguments.IndexOf(argument);
                    var parameterSymbol = methodSymbol.Parameters[argumentIndex];

                    var typeSyntax = GetTypeSyntax(parameterSymbol.Type);
                    var variableName = parameterSymbol.Name;

                    var declarator = SyntaxFactory.VariableDeclarator(variableName);
                    var declaration = SyntaxFactory.VariableDeclaration(typeSyntax, declarator.ToSeparatedSyntaxList());

                    var ifStatement = ConvertToIfStatement(conditional, trueCase => AssignmentStatement(declarator, trueCase), falseCase => AssignmentStatement(declarator, falseCase));

                    var updatedInvocation = invocation.ReplaceNode(argument, Argument(variableName).WithTriviaFrom(argument));

                    switch (invocation.Parent)
                    {
                        case ReturnStatementSyntax returnStatement:
                        {
                            var localDeclaration = SyntaxFactory.LocalDeclarationStatement(declaration).WithLeadingTriviaFrom(returnStatement);
                            var updatedReturnStatement = returnStatement.ReplaceNode(invocation, updatedInvocation);

                            return root.ReplaceNode(returnStatement, new SyntaxNode[] { localDeclaration, ifStatement, updatedReturnStatement });
                        }

                        case ArrowExpressionClauseSyntax arrowClause:
                        {
                            var localDeclaration = SyntaxFactory.LocalDeclarationStatement(declaration);

                            var statement = arrowClause.HasReturnValue()
                                            ? (StatementSyntax)SyntaxFactory.ReturnStatement(updatedInvocation)
                                            : SyntaxFactory.ExpressionStatement(updatedInvocation);

                            return UpdateArrowExpressionClause(root, arrowClause, SyntaxFactory.Block(localDeclaration, ifStatement, statement));
                        }
                    }
                }
            }

            return root;
        }

        private static SyntaxNode UpdateAssignment(SyntaxNode root, ConditionalExpressionSyntax conditional, AssignmentExpressionSyntax assignment)
        {
            switch (assignment.Parent)
            {
                case ArrowExpressionClauseSyntax clause when clause.Parent is AccessorDeclarationSyntax accessor:
                {
                    var ifStatement = ConvertToIfStatement(conditional, trueCase => AssignmentStatement(assignment, trueCase), falseCase => AssignmentStatement(assignment, falseCase));

                    var updatedAccessor = accessor.WithBody(SyntaxFactory.Block(ifStatement))
                                                  .WithExpressionBody(null)
                                                  .WithSemicolonToken(default);

                    return root.ReplaceNode(accessor, updatedAccessor);
                }

                case ExpressionStatementSyntax expression:
                {
                    var ifStatement = ConvertToIfStatement(conditional, trueCase => AssignmentStatement(assignment, trueCase), falseCase => AssignmentStatement(assignment, falseCase));

                    return root.ReplaceNode(expression, ifStatement.WithTriviaFrom(expression));
                }

                default:
                    return root;
            }
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

        private static SyntaxNode UpdateParenthesized(SyntaxNode root, ConditionalExpressionSyntax conditional, ParenthesizedExpressionSyntax parenthesized)
        {
            if (parenthesized.Parent is BinaryExpressionSyntax binary)
            {
                switch (binary.Parent)
                {
                    case ReturnStatementSyntax returnStatement:
                        return ReplaceReturn(returnStatement);

                    case ParenthesizedExpressionSyntax redundant when redundant.Parent is ReturnStatementSyntax returnStatement:
                        return ReplaceReturn(returnStatement);
                }
            }

            return root;

            SyntaxNode ReplaceReturn(ReturnStatementSyntax returnStatement)
            {
                var ifStatement = ConvertToIfStatement(conditional, SyntaxFactory.ReturnStatement);

                if (ifStatement.Statement is BlockSyntax block && block.Statements.First() is ReturnStatementSyntax leftReturn && leftReturn.Expression is ExpressionSyntax leftExpression)
                {
                    ifStatement = ifStatement.ReplaceNode(leftReturn, leftReturn.WithExpression(binary.WithLeft(leftExpression)));
                }

                return root.ReplaceNode(returnStatement, ifStatement);
            }
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

        private static IfStatementSyntax ConvertToIfStatement(ConditionalExpressionSyntax conditional, Func<ExpressionSyntax, StatementSyntax> callback)
        {
            return ConvertToIfStatement(conditional, callback, callback);
        }

        private static IfStatementSyntax ConvertToIfStatement(ConditionalExpressionSyntax conditional, Func<ExpressionSyntax, StatementSyntax> trueCallback, Func<ExpressionSyntax, StatementSyntax> falseCallback)
        {
            var condition = conditional.Condition.WithoutParenthesis().WithoutTrivia();
            var ifBlock = SyntaxFactory.Block(trueCallback(conditional.WhenTrue));
            var elseBlock = SyntaxFactory.Block(falseCallback(conditional.WhenFalse));

            return SyntaxFactory.IfStatement(condition, ifBlock, SyntaxFactory.ElseClause(elseBlock));
        }

        private static TypeSyntax GetTypeSyntax(ITypeSymbol type)
        {
            if (TypeMapping.TryGetValue(type.SpecialType, out var kind))
            {
                return PredefinedType(kind);
            }

            var name = type.IsGeneric()
                       ? type.FullyQualifiedName().GetNameOnlyPart()
                       : type.Name;

            return SyntaxFactory.ParseTypeName(name);
        }

        private static TypeSyntax GetTypeSyntax(VariableDeclarationSyntax declaration, Document document)
        {
            var declarationType = declaration.Type;

            if (declarationType is IdentifierNameSyntax identifier && identifier.Identifier.ValueText == "var")
            {
                var updatedType = GetTypeSyntax(declarationType.GetTypeSymbol(document));

                return updatedType.WithTriviaFrom(declarationType);
            }

            return declarationType;
        }
    }
}