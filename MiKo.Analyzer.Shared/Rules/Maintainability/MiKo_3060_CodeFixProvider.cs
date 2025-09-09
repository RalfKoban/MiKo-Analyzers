using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3060_CodeFixProvider)), Shared]
    public sealed class MiKo_3060_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3060";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ExpressionStatementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => null; // we want to remove the syntax

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (root.DescendantNodes<IdentifierNameSyntax>().None(_ => IsDebugOrTrace(_.GetName())))
            {
                return root.WithoutUsing("System.Diagnostics"); // remove unused "using System.Diagnostics;"
            }

            return root;
        }

        private static bool IsDebugOrTrace(string name)
        {
            switch (name)
            {
                case nameof(Debug):
                case nameof(Trace):
                    return true;

                default:
                    return false;
            }
        }
    }
}