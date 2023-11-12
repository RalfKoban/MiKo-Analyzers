using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6050_CodeFixProvider)), Shared]
    public sealed class MiKo_6050_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6050_MultilineArgumentsAreIndentedToRightAnalyzer.Id;

        protected override string Title => Resources.MiKo_6050_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ArgumentSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ArgumentSyntax argument)
            {
                var position = MiKo_6050_MultilineArgumentsAreIndentedToRightAnalyzer.GetOutdentedStartPosition(argument.FirstAncestor<ArgumentListSyntax>());
                var additionalSpaces = position.Character - argument.GetPositionWithinStartLine();

                var descendants = SelfAndDescendantsOnSeparateLines(argument);

                return GetNodeAndDescendantsWithAdditionalSpaces(argument, descendants, additionalSpaces);
            }

            return syntax;
        }
    }
}