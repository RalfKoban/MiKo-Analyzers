using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2302_CommentedOutCodeAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2302";

        public MiKo_2302_CommentedOutCodeAnalyzer() : base(Id)
        {
        }

        protected override bool CanRunConcurrently => true;

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel) => CodeDetector.IsCommentedOutCodeLine(comment, semanticModel);
    }
}