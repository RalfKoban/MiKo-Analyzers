using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_5002_CodeFixProvider)), Shared]
    public sealed class MiKo_5002_CodeFixProvider : PerformanceCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_5002";

        protected override string Title => Resources.MiKo_5002_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MemberAccessExpressionSyntax>().FirstOrDefault()?.Name;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var identifier = (SimpleNameSyntax)syntax;

            var name = identifier.GetName().WithoutSuffix(MiKo_5002_DebugFormatInsteadDebugLogAnalyzer.Format);

            return SyntaxFactory.IdentifierName(name);
        }
    }
}