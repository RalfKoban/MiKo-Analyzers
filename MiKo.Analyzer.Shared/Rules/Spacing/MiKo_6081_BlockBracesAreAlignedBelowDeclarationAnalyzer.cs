using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6081";

        public MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeBlock, SyntaxKind.Block);
            context.RegisterSyntaxNodeAction(AnalyzeSwitch, SyntaxKind.SwitchStatement); // SwitchExpressions are already addressed by MiKo_6047
            context.RegisterSyntaxNodeAction(AnalyzeAccessorList, SyntaxKind.AccessorList);

            context.RegisterSyntaxNodeAction(AnalyzeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDeclaration, SyntaxKind.RecordDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDeclaration, SyntaxKind.RecordStructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDeclaration, SyntaxKind.InterfaceDeclaration);

#if VS2026
            context.RegisterSyntaxNodeAction(AnalyzeDeclaration, SyntaxKind.ExtensionBlockDeclaration);
#endif

            context.RegisterSyntaxNodeAction(AnalyzeNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
        }

        private static bool Ignore(SyntaxNode parent)
        {
            switch (parent)
            {
                case BlockSyntax _: // ignore nested blocks
                case LambdaExpressionSyntax _: // already addressed by MiKo_6063
                case SwitchSectionSyntax _: // already addressed by MiKo_6033
                case AnonymousMethodExpressionSyntax _: // already addressed by MiKo_6079
                {
                    return true;
                }

                default:
                    return false;
            }
        }

        private void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BlockSyntax node && node.Parent is SyntaxNode parent)
            {
                if (Ignore(parent))
                {
                    return;
                }

                // find the left-most token
                if (parent is IfStatementSyntax i && i.Parent is ElseClauseSyntax clause)
                {
                    parent = clause;
                }

                var orientationToken = parent.FirstDescendantToken();

                AnalyzeBraces(context, orientationToken, node.OpenBraceToken, node.CloseBraceToken);
            }
        }

        private void AnalyzeSwitch(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is SwitchStatementSyntax node)
            {
                AnalyzeBraces(context, node.SwitchKeyword, node.OpenBraceToken, node.CloseBraceToken);
            }
        }

        private void AnalyzeAccessorList(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is AccessorListSyntax node && node.Parent is SyntaxNode parent)
            {
                // find the left-most token
                var orientationToken = parent.FirstDescendantToken();

                AnalyzeBraces(context, orientationToken, node.OpenBraceToken, node.CloseBraceToken);
            }
        }

        private void AnalyzeDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BaseTypeDeclarationSyntax node)
            {
                // find the left-most token
                var orientationToken = node.FirstDescendantToken();

                AnalyzeBraces(context, orientationToken, node.OpenBraceToken, node.CloseBraceToken);
            }
        }

        private void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is NamespaceDeclarationSyntax node)
            {
                AnalyzeBraces(context, node.NamespaceKeyword, node.OpenBraceToken, node.CloseBraceToken);
            }
        }

        private void AnalyzeBraces(in SyntaxNodeAnalysisContext context, in SyntaxToken orientationToken, in SyntaxToken openBrace, in SyntaxToken closeBrace)
        {
            if (openBrace.IsOnSameLineAs(orientationToken))
            {
                if (openBrace.IsOnSameLineAs(closeBrace))
                {
                    // do not report one-liners
                    return;
                }
            }
            else
            {
                if (openBrace.IsOnSameLineAs(closeBrace))
                {
                    // do not report one-liners
                    return;
                }
            }

            var orientationPosition = orientationToken.GetPositionWithinStartLine();
            var openBracePosition = openBrace.GetPositionWithinStartLine();

            if (openBracePosition != orientationPosition)
            {
                ReportDiagnostics(context, Issue(openBrace, CreateProposalForSpaces(orientationPosition, orientationPosition - openBracePosition)));
            }
        }
    }
}