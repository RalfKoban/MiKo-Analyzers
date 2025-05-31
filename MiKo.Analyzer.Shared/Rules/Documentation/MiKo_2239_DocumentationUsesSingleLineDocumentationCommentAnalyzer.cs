using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2239_DocumentationUsesSingleLineDocumentationCommentAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2239";

        public MiKo_2239_DocumentationUsesSingleLineDocumentationCommentAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.MultiLineDocumentationCommentTrivia);

        private void AnalyzeComment(SyntaxNodeAnalysisContext context) => ReportDiagnostics(context, Issue(context.Node));
    }
}