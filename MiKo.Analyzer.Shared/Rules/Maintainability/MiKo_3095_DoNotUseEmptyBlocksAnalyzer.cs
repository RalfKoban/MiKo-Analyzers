using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3095_DoNotUseEmptyBlocksAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3095";

        public MiKo_3095_DoNotUseEmptyBlocksAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeBlock, SyntaxKind.Block);

        private void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;

            if (block.Parent is ConstructorDeclarationSyntax)
            {
                // do not report blocks of constructors
                return;
            }

            if (block.Statements.Count == 0 && block.DescendantTrivia().None(_ => _.IsComment()))
            {
                ReportDiagnostics(context, Issue(block));
            }
        }
    }
}