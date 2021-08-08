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

        protected sealed override string Title => Resources.MiKo_3103_CodeFixTitle;

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            var invocation = syntaxNodes.OfType<InvocationExpressionSyntax>().First();

            return IsToStringCall(invocation.Parent)
                       ? invocation.Parent.Parent
                       : invocation;
        }

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
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

            var literal = CreateStringLiteralExpressionSyntax(newGuid);

            // we only want to have a GUID
            if (isToString)
            {
                // we only want to have a GUID
                return literal;
            }

            return Invocation(nameof(Guid), nameof(Guid.Parse), Argument(literal));
        }

        protected virtual Guid CreateGuid() => Guid.NewGuid();

        private static LiteralExpressionSyntax CreateStringLiteralExpressionSyntax(string value)
        {
            var token = SyntaxFactory.Token(default, SyntaxKind.StringLiteralToken, value, value, default);

            return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, token);
        }

        private static bool IsToStringCall(SyntaxNode node) => node is MemberAccessExpressionSyntax m
                                                            && m.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                                                            && m.GetName() == nameof(ToString);
    }
}