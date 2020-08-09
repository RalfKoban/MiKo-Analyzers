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
    public sealed class MiKo_3103_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer.Id;

        protected override string Title => "Use hard-coded GUID";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            // that's for the method call
            var type = SyntaxFactory.IdentifierName(nameof(Guid));
            var method = SyntaxFactory.IdentifierName(nameof(Guid.Parse));
            var member = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);

            // that's for the argument
            var guid = Guid.NewGuid().ToString("D").SurroundedWithDoubleQuote();
            var token = SyntaxFactory.Token(default, SyntaxKind.StringLiteralToken, guid, guid, default);
            var stringLiteral = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, token);
            var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(stringLiteral) }));

            // combine both to complete call
            return SyntaxFactory.InvocationExpression(member, arguments);
        }
    }
}