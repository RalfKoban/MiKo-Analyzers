using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2020_CodeFixProvider)), Shared]
    public sealed class MiKo_2020_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2020_InheritdocSummaryAnalyzer.Id;

        protected override string Title => "Use <inheritdoc/>";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(Constants.XmlTag.Summary, syntaxNodes).FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax) => SyntaxFactory.XmlEmptyElement(Constants.XmlTag.Inheritdoc);
    }
}