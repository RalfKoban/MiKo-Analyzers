using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6060_CodeFixProvider)), Shared]
    public sealed class MiKo_6060_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6060";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax(syntax);

        private static T GetUpdatedSyntax<T>(T syntax) where T : SyntaxNode
        {
            switch (syntax)
            {
                case null:
                    return null;

                case CaseSwitchLabelSyntax label:
                    return label.WithKeyword(label.Keyword.WithoutTrailingTrivia())
                                .WithValue(GetUpdatedSyntax(label.Value).WithLeadingSpace().WithoutTrailingTrivia())
                                .WithColonToken(label.ColonToken.WithoutLeadingTrivia()) as T;

                case CasePatternSwitchLabelSyntax patternLabel:
                    return patternLabel.WithKeyword(patternLabel.Keyword.WithoutTrailingTrivia())
                                       .WithPattern(GetUpdatedSyntax(patternLabel.Pattern).WithLeadingSpace().WithoutTrailingTrivia())
                                       .WithWhenClause(GetUpdatedSyntax(patternLabel.WhenClause))
                                       .WithColonToken(patternLabel.ColonToken.WithoutLeadingTrivia()) as T;

                case MemberAccessExpressionSyntax maes:
                    return maes.WithName(maes.Name.WithoutTrivia())
                               .WithOperatorToken(maes.OperatorToken.WithoutTrivia())
                               .WithExpression(GetUpdatedSyntax(maes.Expression)) as T;

                case BinaryExpressionSyntax binary:
                    return binary.WithLeft(GetUpdatedSyntax(binary.Left))
                                 .WithOperatorToken(binary.OperatorToken.WithLeadingSpace().WithoutTrailingTrivia())
                                 .WithRight(GetUpdatedSyntax(binary.Right)) as T;

                case IsPatternExpressionSyntax pattern:
                    return pattern.WithPattern(GetUpdatedSyntax(pattern.Pattern))
                                  .WithIsKeyword(pattern.IsKeyword.WithLeadingSpace().WithoutTrailingTrivia())
                                  .WithExpression(GetUpdatedSyntax(pattern.Expression)) as T;

                case InvocationExpressionSyntax invocation:
                    return invocation.WithExpression(GetUpdatedSyntax(invocation.Expression))
                                     .WithArgumentList(GetUpdatedSyntax(invocation.ArgumentList)) as T;

                case WhenClauseSyntax clause:
                    return clause?.WithWhenKeyword(clause.WhenKeyword.WithLeadingSpace().WithoutTrailingTrivia())
                                  .WithCondition(GetUpdatedSyntax(clause.Condition)) as T;

                case DeclarationPatternSyntax declaration:
                    return declaration.WithType(declaration.Type.WithoutTrailingTrivia())
                                      .WithDesignation(GetUpdatedSyntax(declaration.Designation)) as T;

                case SingleVariableDesignationSyntax singleVariable:
                    return singleVariable.WithIdentifier(singleVariable.Identifier.WithoutTrivia()) as T;

                case ArgumentListSyntax argumentList:
                    return argumentList.WithOpenParenToken(argumentList.OpenParenToken.WithoutTrivia())
                                       .WithArguments(GetUpdatedSyntax(argumentList.Arguments))
                                       .WithCloseParenToken(argumentList.CloseParenToken.WithoutTrivia())
                                       .WithoutTrivia() as T;

                case ArgumentSyntax argument:
                    return argument.WithRefKindKeyword(argument.RefKindKeyword.WithoutLeadingTrivia().WithTrailingSpace())
                                   .WithRefOrOutKeyword(argument.RefOrOutKeyword.WithoutLeadingTrivia().WithTrailingSpace())
                                   .WithNameColon(GetUpdatedSyntax(argument.NameColon))
                                   .WithExpression(GetUpdatedSyntax(argument.Expression)) as T;

                default:
                    return syntax.WithoutTrivia();
            }
        }

        private static SeparatedSyntaxList<ArgumentSyntax> GetUpdatedSyntax(SeparatedSyntaxList<ArgumentSyntax> syntax)
        {
            var updatedItems = syntax.GetWithSeparators()
                                     .Select(token =>
                                                     {
                                                         if (token.IsNode)
                                                         {
                                                             return GetUpdatedSyntax(token.AsNode());
                                                         }

                                                         if (token.IsToken)
                                                         {
                                                             return token.AsToken().WithLeadingSpace().WithoutTrailingTrivia();
                                                         }

                                                         return token;
                                                     });

            return SyntaxFactory.SeparatedList<ArgumentSyntax>(updatedItems);
        }
    }
}