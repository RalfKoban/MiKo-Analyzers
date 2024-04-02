using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2220_CodeFixProvider)), Shared]
    public sealed class MiKo_2220_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2220";

        protected override string Title => Resources.MiKo_2220_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, Constants.Comments.FindTerms, ReplacementMap);
        }

//// ncrunch: rdi off

        private static Dictionary<string, string> CreateReplacementMap()
        {
            return Constants.Comments.FindTerms.ToDictionary(_ => _, _ => Constants.Comments.ToSeekTerm);
        }

//// ncrunch: rdi default
    }
}