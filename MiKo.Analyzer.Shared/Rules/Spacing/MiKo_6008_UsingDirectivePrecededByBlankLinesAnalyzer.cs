using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6008_UsingDirectivePrecededByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6008";

        public MiKo_6008_UsingDirectivePrecededByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeUsingDirectiveSyntax, SyntaxKind.UsingDirective);
        }

        private static bool ShallAnalyze(UsingDirectiveSyntax node) => node.ChildNodes<NameEqualsSyntax>().None();

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

        private static string GetName(UsingDirectiveSyntax node) => GetName(node.Name);

        private static string GetName(NameSyntax node)
        {
            while (true)
            {
                if (node is QualifiedNameSyntax q)
                {
                    node = q.Left;
                }
                else
                {
                    return node.GetName();
                }
            }
        }

        private void AnalyzeUsingDirectiveSyntax(SyntaxNodeAnalysisContext context)
        {
            var node = (UsingDirectiveSyntax)context.Node;

            if (ShallAnalyze(node))
            {
                if (HasIssue(node))
                {
                    var issue = Issue(node, true, false);

                    ReportDiagnostics(context, issue);
                }
            }
        }
    }
}