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

        private static bool HasIssue(UsingDirectiveSyntax node)
        {
            if (node.PreviousSibling() is UsingDirectiveSyntax previous && InvestigateForBlankLines(node, previous))
            {
                return HasNoBlankLinesBefore(node, previous);
            }

            return false;
        }

        private static bool InvestigateForBlankLines(UsingDirectiveSyntax current, UsingDirectiveSyntax previous)
        {
            var currentNameEqualsSyntax = current.FirstChild<NameEqualsSyntax>();
            var previousNameEqualsSyntax = previous.FirstChild<NameEqualsSyntax>();

            if (currentNameEqualsSyntax != null && previousNameEqualsSyntax != null)
            {
                // do not inspect using aliases that are side by side
                return false;
            }

            if (currentNameEqualsSyntax != null)
            {
                // inspect further as we have an alias but the previous node is no alias
                return true;
            }

            if (previousNameEqualsSyntax != null)
            {
                // inspect further as the previous node is an alias but the current node is not
                return true;
            }

            // we have both normal using directives
            var currentName = GetName(current);
            var previousName = GetName(previous);

            return currentName != previousName;
        }

        private static string GetName(UsingDirectiveSyntax node) => GetName(node.Name);

        private static string GetName(NameSyntax node)
        {
            var syntax = node;

            while (true)
            {
                if (syntax is QualifiedNameSyntax q)
                {
                    syntax = q.Left;
                }
                else
                {
                    return syntax.GetName();
                }
            }
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
    }
}