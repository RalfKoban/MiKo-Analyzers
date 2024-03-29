﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6012_ForEachStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<ForEachStatementSyntax>
    {
        public const string Id = "MiKo_6012";

        public MiKo_6012_ForEachStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.ForEachStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(ForEachStatementSyntax node) => node.ForEachKeyword;
    }
}