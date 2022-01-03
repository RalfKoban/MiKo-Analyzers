using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3103_CodeFixProvider)), Shared]
    public class MiKo_3103_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        private const string DefaultFormat = "D";

        public sealed override string FixableDiagnosticId => MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer.Id;

        protected sealed override string Title => Resources.MiKo_3103_CodeFixTitle;

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            foreach (var syntax in syntaxNodes)
            {
                if (syntax is InvocationExpressionSyntax invocation)
                {
                    return IsToStringCall(invocation.Parent)
                               ? invocation.Parent?.Parent
                               : invocation;
                }

                if (syntax is ArgumentSyntax argument)
                {
                    // we have a method group, so we have to identify the argument
                    return argument.Expression;
                }
            }

            return null;
        }

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            if (syntax is InvocationExpressionSyntax i)
            {
                if (IsToStringCall(i.Expression))
                {
                    var arguments = i.ArgumentList.Arguments;
                    var format = arguments.Count == 1
                                 ? arguments[0].Expression.ToString().WithoutQuotes()
                                 : DefaultFormat;

                    // we only want to have a GUID
                    return Guid(format);
                }

                return GuidParse(Guid());
            }

            if (syntax is MemberAccessExpressionSyntax)
            {
                return SyntaxFactory.ParenthesizedLambdaExpression(SyntaxFactory.ParameterList(), GuidParse(Guid()));
            }

            return null;
        }

        protected virtual Guid CreateGuid() => System.Guid.NewGuid();

        private static bool IsToStringCall(SyntaxNode node) => node is MemberAccessExpressionSyntax m
                                                               && m.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                                                               && m.GetName() == nameof(ToString);

        private static InvocationExpressionSyntax GuidParse(ExpressionSyntax literal) => Invocation(nameof(System.Guid), nameof(System.Guid.Parse), Argument(literal));

        private LiteralExpressionSyntax Guid(string format = DefaultFormat)
        {
            var guid = CreateGuid();

            return StringLiteral(guid.ToString(format));
        }
    }
}