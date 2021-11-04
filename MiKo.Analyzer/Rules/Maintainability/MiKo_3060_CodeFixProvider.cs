using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3060_CodeFixProvider)), Shared]
    public sealed class MiKo_3060_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3060_DebugTraceAssertAnalyzer.Id;

        protected override string Title => Resources.MiKo_3060_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ExpressionStatementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic) => null; // we want to remove the syntax

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic)
        {
            // remove unused "using System.Diagnostics;"
            return root.DescendantNodes().OfType<UsingDirectiveSyntax>()
                       .Where(_ => _.Name.ToFullString() == "System.Diagnostics")
                       .Select(root.Without)
                       .FirstOrDefault();
        }
    }
}