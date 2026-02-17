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

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            // detect if we are an arrow
            if (syntax is ConditionalExpressionSyntax conditional)
            {
                switch (conditional.Parent)
                {
                    case ArrowExpressionClauseSyntax clause: return UpdateArrowExpressionClause(root, conditional, clause);
                    case ArgumentSyntax argument: return UpdateArgument(document, root, conditional, argument);
                    case AssignmentExpressionSyntax assignment: return UpdateAssignment(document, root, conditional, assignment);
                    case EqualsValueClauseSyntax clause: return UpdateEqualsValueClause(document, root, conditional, clause);
                    case ParenthesizedExpressionSyntax parenthesized: return UpdateParenthesized(root, conditional, parenthesized);
                    case ReturnStatementSyntax statement: return UpdateReturn(root, conditional, statement);
                    case ThrowStatementSyntax statement: return UpdateThrow(root, conditional, statement);
                }
            }

            return root;
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

                    var localDeclaration = LocalVariable(typeSyntax, variableName, out var declarator);

                    var ifStatement = ConvertToIfStatement(conditional, trueCase => AssignmentStatement(declarator, trueCase), falseCase => AssignmentStatement(declarator, falseCase));

                    var updatedInvocation = invocation.ReplaceNode(argument, Argument(variableName).WithTriviaFrom(argument));

                    switch (invocation.Parent)
                    {
                        case ReturnStatementSyntax returnStatement:
                        {
                            var updatedReturnStatement = returnStatement.ReplaceNode(invocation, updatedInvocation);

                            return root.ReplaceNode(returnStatement, new SyntaxNode[] { localDeclaration.WithLeadingTriviaFrom(returnStatement), ifStatement, updatedReturnStatement });
                        }

                        case ArrowExpressionClauseSyntax arrowClause:
                        {
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
                    var updatedMethod = method.WithBody(block).WithoutExpressionBody();

                    return root.ReplaceNode(method, updatedMethod);
                }

                case PropertyDeclarationSyntax property:
                {
                    var getter = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, block);
                    var updatedProperty = property.WithAccessorList(SyntaxFactory.AccessorList(getter.ToSyntaxList())).WithoutExpressionBody();

                    return root.ReplaceNode(property, updatedProperty);
                }

                case AccessorDeclarationSyntax accessor:
                {
                    var updatedAccessor = accessor.WithBody(block).WithoutExpressionBody();

                    return root.ReplaceNode(accessor, updatedAccessor);
                }

                default:
                    return root;
            }
        }

        private static SyntaxNode UpdateArrowExpressionClause(SyntaxNode root, ConditionalExpressionSyntax conditional, AssignmentExpressionSyntax assignment, AccessorDeclarationSyntax accessor)
        {
            var ifStatement = ConvertToIfStatement(conditional, trueCase => AssignmentStatement(assignment, trueCase), falseCase => AssignmentStatement(assignment, falseCase));

            var updatedAccessor = accessor.WithBody(SyntaxFactory.Block(ifStatement)).WithoutExpressionBody();

            return root.ReplaceNode(accessor, updatedAccessor);
        }

        private static SyntaxNode UpdateAssignment(Document document, SyntaxNode root, ConditionalExpressionSyntax conditional, AssignmentExpressionSyntax assignment)
        {
            switch (assignment.Parent)
            {
                case ArrowExpressionClauseSyntax clause when clause.Parent is AccessorDeclarationSyntax accessor:
                    return UpdateArrowExpressionClause(root, conditional, assignment, accessor);

                case ExpressionStatementSyntax expression:
                    return UpdateExpressionStatement(document, root, conditional, assignment, expression);

                case InitializerExpressionSyntax initializer:
                    return UpdateInitializer(document, root, conditional, assignment, initializer);

                default:
                    return root;
            }
        }

        private static SyntaxNode UpdateDeclarationExpression(Document document, SyntaxNode root, ConditionalExpressionSyntax conditional, AssignmentExpressionSyntax assignment, DeclarationExpressionSyntax declaration, ExpressionStatementSyntax expression)
        {
            var designations = new List<SingleVariableDesignationSyntax>();
            CollectSingleVariableDesignations(declaration.Designation, designations);

            var arguments = designations.Select(_ => Argument(_.Identifier.ValueText)).ToSeparatedSyntaxList();
            var updatedAssignment = assignment.WithLeft(SyntaxFactory.TupleExpression(arguments).WithTrailingTriviaFrom(declaration));

            var ifStatement = ConvertToIfStatement(conditional, trueCase => AssignmentStatement(updatedAssignment, trueCase), falseCase => AssignmentStatement(updatedAssignment, falseCase));

            var updatedNodes = new List<SyntaxNode>(designations.Select(ConvertToLocalDeclarationStatement)) { ifStatement };

            return root.ReplaceNode(expression, updatedNodes);

            void CollectSingleVariableDesignations(VariableDesignationSyntax designation, List<SingleVariableDesignationSyntax> nodes)
            {
                switch (designation)
                {
                    case SingleVariableDesignationSyntax single:
                    {
                        nodes.Add(single);

                        break;
                    }

                    case ParenthesizedVariableDesignationSyntax parenthesized:
                    {
                        foreach (var variable in parenthesized.Variables)
                        {
                            CollectSingleVariableDesignations(variable, nodes);
                        }

                        break;
                    }
                }
            }

            LocalDeclarationStatementSyntax ConvertToLocalDeclarationStatement(SingleVariableDesignationSyntax variable)
            {
                var typeSyntax = GetTypeSyntax(variable, document);

                var updatedDeclarator = SyntaxFactory.VariableDeclarator(variable.Identifier).WithLeadingTriviaFrom(declaration);
                var updatedDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, updatedDeclarator.ToSeparatedSyntaxList()).WithLeadingTriviaFrom(declaration);
                var updatedLocalDeclaration = SyntaxFactory.LocalDeclarationStatement(updatedDeclaration).WithLeadingTriviaFrom(declaration);

                return updatedLocalDeclaration;
            }
        }

        private static SyntaxNode UpdateEqualsValueClause(Document document, SyntaxNode root, ConditionalExpressionSyntax conditional, EqualsValueClauseSyntax clause)
        {
            if (clause.Parent is VariableDeclaratorSyntax declarator && declarator.Parent is VariableDeclarationSyntax declaration && declaration.Parent is LocalDeclarationStatementSyntax localDeclaration)
            {
                var typeSyntax = GetTypeSyntax(declaration, document);

                var updatedDeclarator = SyntaxFactory.VariableDeclarator(declarator.Identifier).WithLeadingTriviaFrom(declarator);
                var updatedDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, updatedDeclarator.ToSeparatedSyntaxList()).WithLeadingTriviaFrom(declaration);
                var updatedLocalDeclaration = SyntaxFactory.LocalDeclarationStatement(updatedDeclaration).WithLeadingTriviaFrom(localDeclaration);

                var ifStatement = ConvertToIfStatement(conditional, trueCase => AssignmentStatement(declarator, trueCase), falseCase => AssignmentStatement(declarator, falseCase));

                return root.ReplaceNode(localDeclaration, new SyntaxNode[] { updatedLocalDeclaration, ifStatement });
            }

            return root;
        }

        private static SyntaxNode UpdateExpressionStatement(Document document, SyntaxNode root, ConditionalExpressionSyntax conditional, AssignmentExpressionSyntax assignment, ExpressionStatementSyntax expression)
        {
            if (assignment.Left is DeclarationExpressionSyntax declaration)
            {
                return UpdateDeclarationExpression(document, root, conditional, assignment, declaration, expression);
            }

            var ifStatement = ConvertToIfStatement(conditional, trueCase => AssignmentStatement(assignment, trueCase), falseCase => AssignmentStatement(assignment, falseCase));

            return root.ReplaceNode(expression, ifStatement.WithTriviaFrom(expression));
        }

        private static SyntaxNode UpdateInitializer(Document document, SyntaxNode root, ConditionalExpressionSyntax conditional, AssignmentExpressionSyntax assignment, InitializerExpressionSyntax initializer)
        {
            var expression = assignment.Right;

            var typeSyntax = GetTypeSyntax(expression.GetTypeSymbol(document));
            var variableName = assignment.Left.GetName().ToLowerCaseAt(0);

            var localDeclaration = LocalVariable(typeSyntax, variableName, out var declarator).WithLeadingTriviaFrom(initializer).WithLeadingEmptyLine();

            var ifStatement = ConvertToIfStatement(conditional, trueCase => AssignmentStatement(declarator, trueCase), falseCase => AssignmentStatement(declarator, falseCase));

            var identifier = IdentifierName(variableName).WithTriviaFrom(expression);
            var updatedInitializer = initializer.ReplaceNode(expression, identifier);

            if (initializer.FirstAncestor<StatementSyntax>() is StatementSyntax statement)
            {
                return root.ReplaceNode(statement, new[] { localDeclaration, ifStatement, statement.ReplaceNode(initializer, updatedInitializer) });
            }

            // seems we have an expression body
            if (initializer.FirstAncestor<ArrowExpressionClauseSyntax>() is ArrowExpressionClauseSyntax arrowClause)
            {
                const int ReturnLength = 6;
                const int NewLength = 3;
                const int Spaces = 2;
                const int Offset = Constants.Indentation + ReturnLength + NewLength + Spaces;

                var spaces = arrowClause.FirstAncestor<MemberDeclarationSyntax>().GetPositionWithinStartLine() + Offset;
                var updatedAndIndentedInitializer = GetSyntaxWithLeadingSpaces(updatedInitializer, spaces);

                var expressionSyntax = arrowClause.Expression.ReplaceNode(initializer, updatedAndIndentedInitializer);

                var clauseStatement = arrowClause.HasReturnValue()
                                      ? (StatementSyntax)SyntaxFactory.ReturnStatement(expressionSyntax)
                                      : SyntaxFactory.ExpressionStatement(expressionSyntax);

                return UpdateArrowExpressionClause(root, arrowClause, SyntaxFactory.Block(localDeclaration, ifStatement, clauseStatement));
            }

            return root;
        }

        private static SyntaxNode UpdateParenthesized(SyntaxNode root, ConditionalExpressionSyntax conditional, ParenthesizedExpressionSyntax parenthesized)
        {
            if (parenthesized.Parent is BinaryExpressionSyntax binary)
            {
                switch (binary.Parent)
                {
                    case ReturnStatementSyntax statement:
                        return ReplaceReturn(statement);

                    case ParenthesizedExpressionSyntax redundant when redundant.Parent is ReturnStatementSyntax statement:
                        return ReplaceReturn(statement);
                }
            }

            return root;

            SyntaxNode ReplaceReturn(ReturnStatementSyntax statement)
            {
                var ifStatement = ConvertToIfStatement(conditional, SyntaxFactory.ReturnStatement);

                if (ifStatement.Statement is BlockSyntax block && block.Statements.First() is ReturnStatementSyntax leftReturn && leftReturn.Expression is ExpressionSyntax leftExpression)
                {
                    ifStatement = ifStatement.ReplaceNode(leftReturn, leftReturn.WithExpression(binary.WithLeft(leftExpression)));
                }

                return root.ReplaceNode(statement, ifStatement);
            }
        }

        private static SyntaxNode UpdateReturn(SyntaxNode root, ConditionalExpressionSyntax conditional, ReturnStatementSyntax statement)
        {
            var ifStatement = ConvertToIfStatement(conditional, SyntaxFactory.ReturnStatement);

            // check if we have an if/else block and update that as well
            switch (statement.Parent)
            {
                case IfStatementSyntax _:
                case ElseClauseSyntax _:
                    return root.ReplaceNode(statement, SyntaxFactory.Block(ifStatement));

                default:
                    return root.ReplaceNode(statement, ifStatement);
            }
        }

        private static SyntaxNode UpdateThrow(SyntaxNode root, ConditionalExpressionSyntax conditional, ThrowStatementSyntax statement)
        {
            var ifStatement = ConvertToIfStatement(conditional, SyntaxFactory.ThrowStatement);

            // check if we have an if/else block and update that as well
            switch (statement.Parent)
            {
                case IfStatementSyntax _:
                case ElseClauseSyntax _:
                    return root.ReplaceNode(statement, SyntaxFactory.Block(ifStatement));

                default:
                    return root.ReplaceNode(statement, ifStatement);
            }
        }

        private static ExpressionStatementSyntax AssignmentStatement(AssignmentExpressionSyntax assignment, ExpressionSyntax expression)
        {
            return SyntaxFactory.ExpressionStatement(assignment.WithRight(expression));
        }

        private static ExpressionStatementSyntax AssignmentStatement(VariableDeclaratorSyntax declarator, ExpressionSyntax expression)
        {
            return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(declarator.Identifier), expression));
        }

        private static IfStatementSyntax ConvertToIfStatement(ConditionalExpressionSyntax conditional, Func<ExpressionSyntax, StatementSyntax> callback)
        {
            return ConvertToIfStatement(conditional, callback, callback);
        }

        private static IfStatementSyntax ConvertToIfStatement(ConditionalExpressionSyntax conditional, Func<ExpressionSyntax, StatementSyntax> trueCallback, Func<ExpressionSyntax, StatementSyntax> falseCallback)
        {
            // create 'if' statement and move comments to proper positions
            var questionToken = conditional.QuestionToken;
            var trueCase = conditional.WhenTrue;
            var trueStatement = trueCallback(trueCase.HasComment() ? trueCase.WithoutTrivia() : trueCase);

            if (questionToken.HasComment())
            {
                trueStatement = trueStatement.WithAdditionalLeadingTrivia(questionToken.GetComment()).WithAdditionalLeadingEmptyLine();
            }

            if (trueCase.HasComment())
            {
                trueStatement = trueStatement.WithAdditionalLeadingTrivia(trueCase.GetComment()).WithAdditionalLeadingEmptyLine();
            }

            // create 'else' statement and move comments to proper positions
            var falseCase = conditional.WhenFalse;
            var colonToken = conditional.ColonToken;
            var falseStatement = falseCallback(falseCase.HasComment() ? falseCase.WithoutTrivia() : falseCase);

            if (colonToken.HasComment())
            {
                falseStatement = falseStatement.WithAdditionalLeadingTrivia(colonToken.GetComment()).WithAdditionalLeadingEmptyLine();
            }

            if (falseCase.HasComment())
            {
                falseStatement = falseStatement.WithAdditionalLeadingTrivia(falseCase.GetComment()).WithAdditionalLeadingEmptyLine();
            }

            var semicolonToken = conditional.FirstAncestor<StatementSyntax>()?.GetSemicolonToken() ?? conditional.FirstAncestor<MemberDeclarationSyntax>().GetSemicolonToken();

            if (semicolonToken.HasTrailingComment())
            {
                falseStatement = falseStatement.WithAdditionalLeadingTrivia(semicolonToken.GetComment()).WithAdditionalLeadingEmptyLine();
            }

            var condition = conditional.Condition.WithoutParenthesis().WithoutTrivia();

            var ifBlock = SyntaxFactory.Block(trueStatement);
            var elseBlock = SyntaxFactory.Block(falseStatement);

            return SyntaxFactory.IfStatement(condition, ifBlock, SyntaxFactory.ElseClause(elseBlock));
        }

        private static TypeSyntax GetTypeSyntax(ITypeSymbol type)
        {
            if (type is IArrayTypeSymbol arrayTypeSymbol)
            {
                var elementType = GetTypeSyntax(arrayTypeSymbol.ElementType);

                ExpressionSyntax size = SyntaxFactory.OmittedArraySizeExpression(); // needs to be the omitted size as otherwise it is not working to create an array with rank 1 (such as 'int[]')

                return SyntaxFactory.ArrayType(elementType, SyntaxFactory.ArrayRankSpecifier(size.ToSeparatedSyntaxList()).ToSyntaxList());
            }

            if (TypeMapping.TryGetValue(type.SpecialType, out var kind))
            {
                return PredefinedType(kind);
            }

            var name = type.HasGenericTypeArguments()
                       ? type.FullyQualifiedName().GetNameOnlyPart()
                       : type.Name;

            return name.AsTypeSyntax();
        }

        private static TypeSyntax GetTypeSyntax(VariableDeclarationSyntax declaration, Document document)
        {
            var declarationType = declaration.Type;

            if (declarationType is IdentifierNameSyntax identifier && identifier.Identifier.ValueText is "var")
            {
                var updatedType = GetTypeSyntax(declarationType.GetTypeSymbol(document));

                return updatedType.WithTriviaFrom(declarationType);
            }

            return declarationType;
        }

        private static TypeSyntax GetTypeSyntax(VariableDesignationSyntax designation, Document document)
        {
            var updatedType = GetTypeSyntax(designation.GetTypeSymbol(document));

            return updatedType.WithTriviaFrom(designation);
        }
    }
}