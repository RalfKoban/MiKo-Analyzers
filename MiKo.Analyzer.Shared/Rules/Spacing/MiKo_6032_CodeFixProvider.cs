using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6032_CodeFixProvider)), Shared]
    public sealed class MiKo_6032_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6032";

        protected override string Title => Resources.MiKo_6032_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ParameterSyntax parameter)
            {
                var position = MiKo_6032_MultilineParametersAreIndentedToRightAnalyzer.GetOutdentedStartPosition(parameter.FirstAncestor<ParameterListSyntax>());
                var spaces = position.Character;

                return parameter.WithLeadingSpaces(spaces);
            }

            return syntax;
        }
    }
}