using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2082_CodeFixProvider)), Shared]
    public sealed class MiKo_2082_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly string[] ReplacementMapKeys = CreateReplacementMapKeys().ToArray();

        private static readonly KeyValuePair<string, string>[] ReplacementMap = ReplacementMapKeys.Select(_ => new KeyValuePair<string, string>(_, string.Empty)).ToArray();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2082";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            return Comment(comment, ReplacementMapKeys, ReplacementMap, FirstWordHandling.MakeUpperCase | FirstWordHandling.KeepLeadingSpace);
        }

//// ncrunch: rdi off

        private static IEnumerable<string> CreateReplacementMapKeys()
        {
            var continuations = new[] { "that", "whether", "for" };

            foreach (var start in Constants.Comments.EnumMemberWrongStartingWords)
            {
                foreach (var continuation in continuations)
                {
                    yield return start + ", " + continuation + " ";
                    yield return start + " " + continuation + " ";
                }

                yield return start + " ";
            }
        }

//// ncrunch: rdi default
    }
}