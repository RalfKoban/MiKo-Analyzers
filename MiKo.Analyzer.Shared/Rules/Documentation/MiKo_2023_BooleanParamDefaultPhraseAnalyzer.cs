using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2023_BooleanParamDefaultPhraseAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2023";

        private static readonly string[] StartingPhrases = Constants.Comments.BooleanParameterStartingPhrase;
        private static readonly string StartingPhrase = StartingPhrases[0];

        private static readonly string[] EndingPhrases = Constants.Comments.BooleanParameterEndingPhrase;
        private static readonly string EndingPhrase = EndingPhrases[0];

        public MiKo_2023_BooleanParamDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter) => parameter.RefKind != RefKind.Out
                                                                                  && parameter.Type.IsBoolean()
                                                                                  && parameter.GetEnclosingMethod().Name != nameof(IDisposable.Dispose);

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            if (CommentHasIssue(comment))
            {
                return new[] { Issue(parameter.Name, parameterComment.GetContentsLocation(), StartingPhrase, EndingPhrase) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static bool CommentHasIssue(string comment)
        {
            const StringComparison Comparison = StringComparison.Ordinal;

            if (comment.StartsWithAny(StartingPhrases, Comparison) && comment.ContainsAny(EndingPhrases, Comparison))
            {
                return comment.Contains("to value indicating", Comparison);
            }

            return true;
        }
    }
}