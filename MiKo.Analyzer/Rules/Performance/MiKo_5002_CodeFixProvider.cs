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
        public override string FixableDiagnosticId => MiKo_5002_DebugFormatInsteadDebugLogAnalyzer.Id;

        protected override string Title => "Replace with non-'" + MiKo_5002_DebugFormatInsteadDebugLogAnalyzer.Format + "' method";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MemberAccessExpressionSyntax>().First().Name;

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var identifier = (SimpleNameSyntax)syntax;

            var name = identifier.GetName().WithoutSuffix(MiKo_5002_DebugFormatInsteadDebugLogAnalyzer.Format);

            return SyntaxFactory.IdentifierName(name);
        }
    }
}