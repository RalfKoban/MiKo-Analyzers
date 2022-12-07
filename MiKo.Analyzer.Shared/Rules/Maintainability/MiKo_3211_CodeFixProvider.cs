﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3211_CodeFixProvider)), Shared]
    public sealed class MiKo_3211_CodeFixProvider : SurroundedByBlankLinesCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3211_LockStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override string Title => Resources.MiKo_3211_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<LockStatementSyntax>().FirstOrDefault();
    }
}