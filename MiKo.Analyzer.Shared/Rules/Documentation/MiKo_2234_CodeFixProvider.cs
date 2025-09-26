using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2234_CodeFixProvider)), Shared]
    public sealed class MiKo_2234_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => "MiKo_2234";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue) => GetUpdatedSyntax(syntax, issue, ReplacementMap);

//// ncrunch: rdi off

        private static Pair[] CreateReplacementMap() => Constants.Comments.WhichIsToTerms.ToArray(_ => new Pair(_, Constants.Comments.ToTerm));

//// ncrunch: rdi default
    }
}