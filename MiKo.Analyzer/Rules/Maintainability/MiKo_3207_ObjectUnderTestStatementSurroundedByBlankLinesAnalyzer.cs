using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3207_ObjectUnderTestStatementSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3207";

        private static readonly HashSet<string> ObjectUnderTestNames = Enumerable.Empty<string>()
                                                                                 .Concat(Constants.Names.TypeUnderTestFieldNames)
                                                                                 .Concat(Constants.Names.TypeUnderTestVariableNames)
                                                                                 .Concat(Constants.Names.TypeUnderTestPropertyNames)
                                                                                 .OrderBy(_ => _)
                                                                                 .ToHashSet();

        public MiKo_3207_ObjectUnderTestStatementSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeExpressionStatement, SyntaxKind.ExpressionStatement);
        }

        private static bool HasNoOtherObjectUnderTestExpression(IEnumerable<StatementSyntax> statements)
        {
            foreach (var statement in statements)
            {
                if (statement is ExpressionStatementSyntax ess && IsInvocationOnObjectUnderTest(ess))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private static bool IsInvocationOnObjectUnderTest(ExpressionStatementSyntax node)
        {
            var found = node.Expression is InvocationExpressionSyntax i
                     && i.Expression is MemberAccessExpressionSyntax mae
                     && mae.Expression is IdentifierNameSyntax ins
                     && ObjectUnderTestNames.Contains(ins.GetName());

            return found;
        }

        private void AnalyzeExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (ExpressionStatementSyntax)context.Node;

            var diagnostic = AnalyzeExpressionStatement1(node);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeExpressionStatement1(ExpressionStatementSyntax node)
        {
            if (IsInvocationOnObjectUnderTest(node))
            {
                return AnalyzeExpressionStatementBlock(node);
            }

            return null;
        }

        private Diagnostic AnalyzeExpressionStatementBlock(CSharpSyntaxNode node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case BlockSyntax block:
                        return AnalyzeExpressionStatement(block.Statements, node);

                    case SwitchSectionSyntax section:
                        return AnalyzeExpressionStatement(section.Statements, node);

                    case MethodDeclarationSyntax _:
                    case ClassDeclarationSyntax _:
                        return null; // stop lookup as there is no valid ancestor anymore
                }
            }

            return null;
        }

        private Diagnostic AnalyzeExpressionStatement(SyntaxList<StatementSyntax> statements, CSharpSyntaxNode node)
        {
            var callLineSpan = node.GetLocation().GetLineSpan();

            var noBlankLinesBefore = HasNoOtherObjectUnderTestExpression(statements.Where(_ => HasNoBlankLinesBefore(callLineSpan, _)));
            var noBlankLinesAfter = HasNoOtherObjectUnderTestExpression(statements.Where(_ => HasNoBlankLinesAfter(callLineSpan, _)));

            if (noBlankLinesBefore || noBlankLinesAfter)
            {
                return Issue(node, noBlankLinesBefore, noBlankLinesAfter);
            }

            return null;
        }
    }
}