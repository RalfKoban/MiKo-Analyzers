namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class MultiLineCommentAnalyzer : SingleLineCommentAnalyzer
    {
        protected MultiLineCommentAnalyzer(string diagnosticId) : base(diagnosticId) => IgnoreMultipleLines = false;
    }
}