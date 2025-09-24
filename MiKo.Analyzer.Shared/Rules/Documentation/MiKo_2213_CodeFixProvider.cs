using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2213_CodeFixProvider)), Shared]
    public sealed class MiKo_2213_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2213";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue)
        {
            return GetUpdatedSyntax(syntax, issue, Constants.Comments.NotContractionReplacementMap);
        }
    }
}