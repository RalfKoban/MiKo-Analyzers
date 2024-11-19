using System;
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
        private static readonly string[] ReplacementMapKeys = Constants.Comments.ActionTerms;

        private static readonly Pair[] ReplacementMap = ReplacementMapKeys.ToArray(_ => new Pair(_, Constants.Comments.CallbackTerm));

        public override string FixableDiagnosticId => "MiKo_2075";

        protected override string Title => Resources.MiKo_2075_CodeFixTitle.FormatWith(Constants.Comments.CallbackTerm);

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic) => Comment(syntax, ReplacementMapKeys, ReplacementMap);
    }
}