using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6080";

        public MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeBlock, SyntaxKind.Block);
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

        private void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BlockSyntax node)
            {
                AnalyzeBraces(context, node.OpenBraceToken, node.CloseBraceToken);
            }
        }

        private void AnalyzeDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BaseTypeDeclarationSyntax node)
            {
                AnalyzeBraces(context, node.OpenBraceToken, node.CloseBraceToken);
            }
        }

        private void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is NamespaceDeclarationSyntax node)
            {
                AnalyzeBraces(context, node.OpenBraceToken, node.CloseBraceToken);
            }
        }

        private void AnalyzeBraces(in SyntaxNodeAnalysisContext context, in SyntaxToken openBrace, in SyntaxToken closeBrace)
        {
            if (openBrace.IsOnSameLineAs(closeBrace))
            {
                return;
            }

            var openBracePosition = openBrace.GetPositionWithinStartLine();
            var closeBracePosition = closeBrace.GetPositionWithinStartLine();

            if (openBracePosition != closeBracePosition)
            {
                ReportDiagnostics(context, Issue(closeBrace, CreateProposalForSpaces(openBracePosition, openBracePosition - closeBracePosition)));
            }
        }
    }
}