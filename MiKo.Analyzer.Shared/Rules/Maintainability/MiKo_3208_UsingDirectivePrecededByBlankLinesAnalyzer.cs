using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3208_UsingDirectivePrecededByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3208";

        public MiKo_3208_UsingDirectivePrecededByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeUsingDirectiveSyntax, SyntaxKind.UsingDirective);
        }

        private void AnalyzeUsingDirectiveSyntax(SyntaxNodeAnalysisContext context)
        {
            var node = (UsingDirectiveSyntax)context.Node;

            if (HasIssue(node))
            {
                var issue = Issue(node, true, false);

                ReportDiagnostics(context, issue);
            }
        }

        private static bool HasIssue(UsingDirectiveSyntax node)
        {
            var otherUsings = node.Siblings<UsingDirectiveSyntax>();

            var nodeIndex = otherUsings.IndexOf(node);
            if (nodeIndex > 0)
            {
                var previous = otherUsings[nodeIndex - 1];

                var nodeName = GetName(node);
                var previousName = GetName(previous);

                if (nodeName != previousName)
                {
                    return HasNoBlankLinesBefore(node, previous);
                }
            }

            return false;
        }

        private static string GetName(UsingDirectiveSyntax node)
        {
            return node.Name is QualifiedNameSyntax q ? q.Left.GetName() : node.Name.GetName();
        }
    }
}