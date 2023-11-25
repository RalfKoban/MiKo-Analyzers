using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6052_CodeFixProvider)), Shared]
    public sealed class MiKo_6052_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6052_BaseListOperatorsAreOnSameLineAsBaseListTypeAnalyzer.Id;

        protected override string Title => Resources.MiKo_6052_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BaseListSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is BaseListSyntax baseList)
            {
                var updatedToken = baseList.ColonToken.WithoutLeadingTrivia().WithTrailingSpace();

                var types = baseList.Types;
                var firstType = types[0];
                var updatedBaseList = types.Replace(firstType, firstType.WithoutLeadingTrivia());

                return baseList.WithColonToken(updatedToken)
                               .WithTypes(updatedBaseList);
            }

            return syntax;
        }
    }
}