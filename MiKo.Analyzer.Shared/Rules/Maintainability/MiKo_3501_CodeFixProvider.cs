using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3501_CodeFixProvider)), Shared]
    public sealed class MiKo_3501_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3501_DoNotUseSuppressNullableWarningOnConditionalAccessAnalyzer.Id;

        protected override string Title => Resources.MiKo_3501_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault(_ => _.IsKind(SyntaxKind.SuppressNullableWarningExpression));

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax is PostfixUnaryExpressionSyntax suppression
                                                                                                                  ? suppression.Operand
                                                                                                                  : syntax;
    }
}