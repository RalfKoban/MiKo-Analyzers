using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2220_CodeFixProvider)), Shared]
    public sealed class MiKo_2220_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly Pair[] ReplacementMap = Constants.Comments.FindTerms.Select(_ => new Pair(_, Constants.Comments.ToSeekTerm))
                                                                 .ConcatenatedWith(new Pair(" to inspect for ", " " + Constants.Comments.ToSeekTerm + " for "))
                                                                 .OrderDescendingByLengthAndText(_ => _.Key);

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2220";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue) => GetUpdatedSyntax(syntax, issue, ReplacementMap);
    }
}