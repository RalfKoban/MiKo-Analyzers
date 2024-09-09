using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3099_CodeFixProvider)), Shared]
    public sealed class MiKo_3099_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3099";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            switch (syntax)
            {
                case BinaryExpressionSyntax binary:
                    return binary.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken)
                           ? FalseLiteral()
                           : TrueLiteral();

                case IsPatternExpressionSyntax pattern:
                    return pattern.Pattern is UnaryPatternSyntax
                           ? TrueLiteral()
                           : FalseLiteral();

                default:
                    return base.GetUpdatedSyntax(document, syntax, issue);
            }
        }
    }
}