using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3120_CodeFixProvider)), Shared]
    public sealed class MiKo_3120_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3120";

        protected override string Title => Resources.MiKo_3120_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ArgumentSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ArgumentSyntax argument && argument.Expression is InvocationExpressionSyntax i && i.ArgumentList.Arguments.FirstOrDefault()?.Expression is LambdaExpressionSyntax lambda)
            {
                switch (lambda.ExpressionBody)
                {
                    case BinaryExpressionSyntax binary:
                        return Argument(binary.Right);

                    case IsPatternExpressionSyntax pattern when pattern.Expression is IdentifierNameSyntax && pattern.Pattern is ConstantPatternSyntax c && c.Expression is LiteralExpressionSyntax literal:
                        return Argument(literal);
                }
            }

            return syntax;
        }
    }
}