using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2016_CodeFixProvider)), Shared]
    public sealed class MiKo_2016_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly Pair[] ReplacementMap = { new Pair("Gets called to ") };

        private static readonly string[] ReplacementMapKeys = GetTermsForQuickLookup(ReplacementMap);

        public override string FixableDiagnosticId => "MiKo_2016";

        protected override string Title => Resources.MiKo_2016_CodeFixTitle.FormatWith(Constants.Comments.AsynchronouslyStartingPhrase);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlElementSyntax summary)
            {
                var preparedComment = MiKo_2019_CodeFixProvider.GetUpdatedSyntax(summary) as XmlElementSyntax ?? summary;

                preparedComment = Comment(preparedComment, ReplacementMapKeys, ReplacementMap);

                return CommentStartingWith(preparedComment, Constants.Comments.AsynchronouslyStartingPhrase, FirstWordAdjustment.StartLowerCase | FirstWordAdjustment.MakeThirdPersonSingular);
            }

            return syntax;
        }
    }
}