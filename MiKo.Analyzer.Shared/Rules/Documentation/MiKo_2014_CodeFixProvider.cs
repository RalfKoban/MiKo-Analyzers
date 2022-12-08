using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2014_CodeFixProvider)), Shared]
    public sealed class MiKo_2014_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2014_DisposeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2014_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(CodeFixContext context, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var method = syntax.FirstAncestorOrSelf<MethodDeclarationSyntax>();

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), MiKo_2014_DisposeDefaultPhraseAnalyzer.SummaryPhrase).WithTrailingXmlComment();

            var parameters = method.ParameterList.Parameters;

            if (parameters.Count == 0)
            {
                return SyntaxFactory.DocumentationComment(summary.WithEndOfLine());
            }

            var param = ParameterComment(parameters[0], MiKo_2014_DisposeDefaultPhraseAnalyzer.ParameterPhrase).WithEndOfLine();

            return SyntaxFactory.DocumentationComment(summary, param);
        }
    }
}