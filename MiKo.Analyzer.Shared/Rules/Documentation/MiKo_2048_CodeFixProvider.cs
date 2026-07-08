using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2048_CodeFixProvider)), Shared]
    public sealed class MiKo_2048_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private const string Phrase = Constants.Comments.ValueConverterSummaryStartingPhrase;

//// ncrunch: rdi off

        private static readonly ReplacementMap ReplacementMap = new ReplacementMap("MiKo_2048_Replace", CreatePhrases().OrderDescendingByLengthAndText(_ => _.Key), _ => GetTermsForQuickLookup(_));

        private static readonly ReplacementMap CleanUpMap = new ReplacementMap("MiKo_2048_Cleanup", CreateCleanupPhrases().OrderDescendingByLengthAndText(_ => _.Key), _ => GetTermsForQuickLookup(_));

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2048";

        protected override string Title => Resources.MiKo_2048_CodeFixTitle.FormatWith(Phrase);

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax((XmlElementSyntax)syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static XmlElementSyntax GetUpdatedSyntax(XmlElementSyntax syntax)
        {
            var preparedComment = Comment(syntax, ReplacementMap, FirstWordAdjustment.StartLowerCase);
            var fixedComment = CommentStartingWith(preparedComment, Phrase);
            var cleanedComment = Comment(fixedComment, CleanUpMap);

            return cleanedComment;
        }

        private static IEnumerable<Pair> CreatePhrases()
        {
            yield return new Pair("Convert ");
            yield return new Pair("Converts ");
        }

        private static IEnumerable<Pair> CreateCleanupPhrases() => Enumerable.Empty<Pair>();
    }
}