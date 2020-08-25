using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2029_CodeFixProvider)), Shared]
    public sealed class MiKo_2029_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2029_InheritdocUsesWrongCrefAnalyzer.Id;

        protected override string Title => "Remove ref value from <inheritdoc/>";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;

            var wrongInheritDocs = syntax.DescendantNodes().OfType<XmlEmptyElementSyntax>()
                                         .Where(_ => _.Name.LocalName.ValueText == Constants.XmlTag.Inheritdoc
                                                  && _.Attributes.Any(__ => __.Name.LocalName.ValueText == "cref"))
                                         .ToList();

            return comment.ReplaceNodes(wrongInheritDocs, (_, __) => SyntaxFactory.XmlEmptyElement(Constants.XmlTag.Inheritdoc));
        }
    }
}