﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3114_CodeFixProvider)), Shared]
    public sealed class MiKo_3114_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3114_UseMockOfInsteadMockObjectAnalyzer.Id;

        protected override string Title => Resources.MiKo_3114_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MemberAccessExpressionSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is MemberAccessExpressionSyntax node && MiKo_3114_UseMockOfInsteadMockObjectAnalyzer.HasIssue(node, out var types))
            {
                return Invocation("Mock", "Of", types);
            }

            return syntax;
        }
    }
}