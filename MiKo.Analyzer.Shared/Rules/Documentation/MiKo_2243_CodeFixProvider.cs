using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2243_CodeFixProvider)), Shared]
    public sealed class MiKo_2243_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2243";

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlTextAttributeSyntax)
            {
                var reference = issue.Properties[Constants.AnalyzerCodeFixSharedData.TextReplacementKey];

                return XmlCref(Cref(reference)).WithTriviaFrom(syntax);
            }

            return syntax;
        }
    }
}