using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_5001_CodeFixProvider)), Shared]
    public sealed class MiKo_5001_CodeFixProvider : PerformanceCodeFixProvider
    {
        private enum MovePosition
        {
            Start = 0,
            End = 1,
        }

        public override string FixableDiagnosticId => MiKo_5001_DebugLogIsEnabledAnalyzer.Id;

        protected override string Title => Resources.MiKo_5001_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ExpressionStatementSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var lambda = syntax.FirstDescendant<LambdaExpressionSyntax>();

            if (lambda != null)
            {
                // fix inside lambda
                var ifStatement = CreateIfStatement(lambda.ExpressionBody);

                // nest call in block
                var block = SyntaxFactory.Block(ifStatement);

                return syntax.ReplaceNode(lambda, lambda.WithBody(block));
            }

            return CreateIfStatement((ExpressionStatementSyntax)syntax);
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            var parent = syntax.Parent;

            if (parent is null)
            {
                // should not happen
                return root;
            }

            if (syntax is IfStatementSyntax insertedIf)
            {
                return GetWithMergedIfStatements(root, insertedIf, annotationOfSyntax, parent);
            }

            return root;
        }

        private static IfStatementSyntax CreateIfStatement(ExpressionStatementSyntax statement) => CreateIfStatement(statement.Expression).WithTriviaFrom(statement);

        private static IfStatementSyntax CreateIfStatement(ExpressionSyntax syntax)
        {
            var call = (InvocationExpressionSyntax)syntax;
            var expression = (MemberAccessExpressionSyntax)call.Expression;

            var condition = CreateCondition(expression);

            // nest call in block
            var block = SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(call));

            return SyntaxFactory.IfStatement(condition, block);
        }

        private static ExpressionSyntax CreateCondition(MemberAccessExpressionSyntax expression)
        {
            var identifier = GetIdentifier(expression);
            var method = SyntaxFactory.IdentifierName(Constants.ILog.IsDebugEnabled);
            var condition = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, identifier, method);

            return condition;
        }

        private static SyntaxNode GetWithMergedIfStatements(SyntaxNode root, IfStatementSyntax insertedIf, SyntaxAnnotation annotation, SyntaxNode parent)
        {
            var children = parent.ChildNodes().ToList();
            var childrenCount = children.Count;

            var position = children.IndexOf(insertedIf);

            if (position == 0)
            {
                // first node
                if (childrenCount > 1)
                {
                    var nextChild = children[position + 1];

                    return MoveIntoIfStatement(root, insertedIf, annotation, nextChild, MovePosition.Start);
                }
            }
            else if (position == childrenCount - 1)
            {
                // last node
                var previousChild = children[position - 1];

                return MoveIntoIfStatement(root, insertedIf, annotation, previousChild, MovePosition.End);
            }
            else
            {
                // in between
                var previousChild = children[position - 1];
                var potentiallyUpdatedRoot = MoveIntoIfStatement(root, insertedIf, annotation, previousChild, MovePosition.End);

                if (ReferenceEquals(potentiallyUpdatedRoot, root))
                {
                    // not replaced, so try with next child
                    var nextChild = children[position + 1];

                    return MoveIntoIfStatement(root, insertedIf, annotation, nextChild, MovePosition.Start);
                }

                return potentiallyUpdatedRoot;
            }

            return root;
        }

        private static SyntaxNode MoveIntoIfStatement(SyntaxNode root, IfStatementSyntax insertedIf, SyntaxAnnotation annotation, SyntaxNode otherChild, MovePosition position)
        {
            if (insertedIf.Statement is BlockSyntax insertedBlock)
            {
                var updatedRoot = MoveIntoIfStatement(root, insertedBlock.Statements, annotation, otherChild, position);

                // see if we have 2 consecutive statements
                return MergeConsecutiveIfStatements(updatedRoot);
            }

            return root;
        }

        private static SyntaxNode MoveIntoIfStatement(SyntaxNode root, IEnumerable<StatementSyntax> insertedStatements, SyntaxAnnotation annotation, SyntaxNode otherChild, MovePosition position)
        {
            if (otherChild is IfStatementSyntax ifStatement && IsDebugEnabledCall(ifStatement))
            {
                var spaces = ifStatement.GetStartPosition().Character;

                var statementsOfInsertedBlock = insertedStatements.Select(_ => _.WithoutLeadingEndOfLine() // Delete left-over empty line at beginning
                                                                                .WithLeadingSpaces(spaces + Constants.Indentation)
                                                                                .WithEndOfLine());

                var existingStatement = ifStatement.Statement;

                var statements = new List<StatementSyntax>();

                // add to block to next statement
                if (existingStatement is BlockSyntax block)
                {
                    statements.AddRange(block.Statements);

                    if (position == MovePosition.Start)
                    {
                        // statements have to be positioned after the already existing one, so insert the existing ones to the beginning
                        statements.InsertRange(0, statementsOfInsertedBlock);
                    }
                    else
                    {
                        // statements have to be positioned before the already existing one, so add the existing ones to the end
                        statements.AddRange(statementsOfInsertedBlock);
                    }
                }
                else
                {
                    // statement is no block, so add it as block
                    statements.AddRange(statementsOfInsertedBlock);

                    if (position == MovePosition.Start)
                    {
                        // statements have to be positioned before the already existing one, so add the existing one to the end
                        statements.Add(existingStatement);
                    }
                    else
                    {
                        // statements have to be positioned after the already existing one, so insert the existing one to the beginning
                        statements.Insert(0, existingStatement);
                    }
                }

                var updatedRoot = root.ReplaceNode(ifStatement, ifStatement.WithoutLeadingEndOfLine() // Delete left-over empty line at beginning
                                                                           .WithLeadingSpaces(spaces)
                                                                           .WithStatement(SyntaxFactory.Block(statements)));

                // now remove the original node as it had been placed inside if
                return updatedRoot.Without(updatedRoot.GetAnnotatedNodes(annotation).First());
            }

            return root;
        }

        private static SyntaxNode MergeConsecutiveIfStatements(SyntaxNode updatedRoot)
        {
            var calls = updatedRoot.DescendantNodes<IfStatementSyntax>(IsDebugEnabledCall).ToList();

            if (calls.Count > 1)
            {
                var firstCall = calls[0];

                var nodesToRemove = new HashSet<IfStatementSyntax>();
                var nodesToReplace = new Dictionary<IfStatementSyntax, IfStatementSyntax>();

                for (var index = 1; index < calls.Count; index++)
                {
                    var secondCall = calls[index];

                    if (ReferenceEquals(firstCall.Parent, secondCall.Parent) && firstCall.NextSibling() == secondCall)
                    {
                        // same parents, so adjust
                        var firstStatement = firstCall.Statement;
                        var secondStatement = secondCall.Statement;

                        var finalStatements = new List<StatementSyntax>();

                        if (secondStatement is BlockSyntax secondBlock)
                        {
                            finalStatements.AddRange(secondBlock.Statements);
                        }
                        else
                        {
                            finalStatements.Add(secondStatement);
                        }

                        if (firstStatement is BlockSyntax firstBlock)
                        {
                            // add statements to block
                            finalStatements.InsertRange(0, firstBlock.Statements);
                        }
                        else
                        {
                            // create block with combined statements
                            finalStatements.Insert(0, firstStatement);
                        }

                        // remove second if and replace first if
                        nodesToRemove.Add(secondCall);

                        nodesToReplace[firstCall] = firstCall.WithStatement(SyntaxFactory.Block(finalStatements));
                    }
                    else
                    {
                        // other stuff, do not merge
                        firstCall = secondCall;
                    }
                }

                const string DeleteAnnotation = "delete me";

                updatedRoot = updatedRoot.ReplaceNodes(
                                                       calls,
                                                       (original, rewritten) =>
                                                       {
                                                           if (nodesToRemove.Contains(original))
                                                           {
                                                               // annotate it so that we can find it again as left-over when it comes to deleting it
                                                               return original.WithAnnotation(new SyntaxAnnotation(DeleteAnnotation));
                                                           }

                                                           if (nodesToReplace.TryGetValue(original, out var replacement))
                                                           {
                                                               return replacement;
                                                           }

                                                           return rewritten;
                                                       });

                // remove the left-overs
                return updatedRoot.RemoveNodes(updatedRoot.GetAnnotatedNodes(DeleteAnnotation), SyntaxRemoveOptions.KeepNoTrivia);
            }

            return updatedRoot;
        }

        private static ExpressionSyntax GetIdentifier(MemberAccessExpressionSyntax expression)
        {
            switch (expression.Expression)
            {
                case IdentifierNameSyntax i:
                    return SyntaxFactory.IdentifierName(i.GetName());

                case MemberAccessExpressionSyntax m:
                    return m.WithoutLeadingTrivia();

                default:
                    return null;
            }
        }

        private static bool IsDebugEnabledCall(IfStatementSyntax ifStatement) => ifStatement.IsCallTo(Constants.ILog.IsDebugEnabled);
    }
}