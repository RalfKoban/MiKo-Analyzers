using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2075_CodeFixProvider)), Shared]
    public sealed class MiKo_2075_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => "MiKo_2075";

        protected override string Title => Resources.MiKo_2075_CodeFixTitle.FormatWith(Constants.Comments.CallbackTerm);

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, Constants.Comments.ActionTerms, ReplacementMap);
        }

        private static Dictionary<string, string> CreateReplacementMap()
        {
            return Constants.Comments.ActionTerms.ToDictionary(_ => _, _ => Constants.Comments.CallbackTerm);
        }
    }
}