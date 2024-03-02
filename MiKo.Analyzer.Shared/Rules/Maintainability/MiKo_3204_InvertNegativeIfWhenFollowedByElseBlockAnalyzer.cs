using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3204_InvertNegativeIfWhenFollowedByElseBlockAnalyzer : InvertNegativeIfAnalyzer
    {
        public const string Id = "MiKo_3204";

        public MiKo_3204_InvertNegativeIfWhenFollowedByElseBlockAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeIfStatement(IfStatementSyntax node, SyntaxNodeAnalysisContext context) => HasIssue(node)
                                                                                                                                    ? new[] { Issue(node) }
                                                                                                                                    : Enumerable.Empty<Diagnostic>();

        private static bool HasIssue(IfStatementSyntax node)
        {
            var elseClause = node.Else;

            return elseClause != null
                && node.Parent is BlockSyntax
                && IsMainlyNegative(node.Condition)
                && elseClause.Statement.IsKind(SyntaxKind.IfStatement) is false;
        }
    }
}