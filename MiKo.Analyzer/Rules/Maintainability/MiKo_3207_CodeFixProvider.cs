﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3207_CodeFixProvider)), Shared]
    public sealed class MiKo_3207_CodeFixProvider : SurroundedByBlankLinesCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3207_ObjectUnderTestStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override string Title => Resources.MiKo_3207_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<StatementSyntax>().FirstOrDefault();
    }
}