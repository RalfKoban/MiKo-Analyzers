using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2029_CodeFixProvider)), Shared]
    public sealed class MiKo_2029_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2029";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var wrongInheritDocs = syntax.DescendantNodes<XmlEmptyElementSyntax>(_ => _.GetName() == Constants.XmlTag.Inheritdoc && _.Attributes.OfType<XmlCrefAttributeSyntax>().Any());

            return syntax.ReplaceNodes(wrongInheritDocs, (_, __) => Inheritdoc());
        }
    }
}