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

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().First();

        protected sealed override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax) => CreateInvocationSyntax(
                                                                                                       nameof(Guid),
                                                                                                       nameof(Guid.Parse),
                                                                                                       CreateGuid().ToString("D").SurroundedWithDoubleQuote());

        protected virtual Guid CreateGuid() => Guid.NewGuid();

        private static SyntaxNode CreateInvocationSyntax(string className, string methodName, string argument)
        {
            // that's for the method call
            var type = SyntaxFactory.IdentifierName(className);
            var method = SyntaxFactory.IdentifierName(methodName);
            var member = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);

            // that's for the argument
            var token = SyntaxFactory.Token(default, SyntaxKind.StringLiteralToken, argument, argument, default);
            var stringLiteral = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, token);
            var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(stringLiteral) }));

            // combine both to complete call
            return SyntaxFactory.InvocationExpression(member, arguments);
        }
    }
}