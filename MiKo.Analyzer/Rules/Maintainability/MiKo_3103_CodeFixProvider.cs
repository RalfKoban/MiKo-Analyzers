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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3103_CodeFixProvider)), Shared]
    public class MiKo_3103_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public sealed override string FixableDiagnosticId => MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer.Id;

        protected sealed override string Title => "Use hard-coded GUID";

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            var invocation = syntaxNodes.OfType<InvocationExpressionSyntax>().First();

            return IsToStringCall(invocation.Parent)
                       ? invocation.Parent.Parent
                       : invocation;
        }

        protected sealed override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var guid = CreateGuid();
            var format = "D";

            var isToString = false;

            if (syntax is InvocationExpressionSyntax i && IsToStringCall(i.Expression))
            {
                isToString = true;

                var arguments = i.ArgumentList.Arguments;
                if (arguments.Count == 1)
                {
                    format = arguments[0].Expression.ToString().Without(@"""");
                }
            }

            var newGuid = guid.ToString(format).SurroundedWithDoubleQuote();

            if (isToString)
            {
                // we only want to have a GUID
                return CreateStringLiteralExpressionSyntax(newGuid);
            }

            return CreateInvocationSyntax(nameof(Guid), nameof(Guid.Parse), newGuid);
        }

        protected virtual Guid CreateGuid() => Guid.NewGuid();

        private static SyntaxNode CreateInvocationSyntax(string className, string methodName, string argument)
        {
            // that's for the method call
            var type = SyntaxFactory.IdentifierName(className);
            var method = SyntaxFactory.IdentifierName(methodName);
            var member = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);

            // that's for the argument
            var stringLiteral = CreateStringLiteralExpressionSyntax(argument);
            var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(stringLiteral) }));

            // combine both to complete call
            return SyntaxFactory.InvocationExpression(member, arguments);
        }

        private static LiteralExpressionSyntax CreateStringLiteralExpressionSyntax(string value)
        {
            var token = SyntaxFactory.Token(default, SyntaxKind.StringLiteralToken, value, value, default);
            return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, token);
        }

        private static bool IsToStringCall(SyntaxNode node)
        {
            if (node is MemberAccessExpressionSyntax m && m.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                if (m.Name is IdentifierNameSyntax i && i.Identifier.ValueText == nameof(ToString))
                {
                    return true;
                }
            }

            return false;
        }
    }
}