using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1200_ExceptionCatchBlockAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1200";

        public MiKo_1200_ExceptionCatchBlockAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeCatchBlock, SyntaxKind.CatchClause);
        }

        private void AnalyzeCatchBlock(SyntaxNodeAnalysisContext context)
        {
            var node = (CatchClauseSyntax)context.Node;
            var diagnostic = AnalyzeCatchClause(node);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeCatchClause(CatchClauseSyntax node)
        {
            if (node.Declaration is null)
            {
                return null; // we don't have an exception
            }

            const string ExceptionIdentifier = "ex";

            var identifier = node.Declaration.Identifier;
            switch (identifier.ValueText)
            {
                case null: // we don't have one
                case ExceptionIdentifier: // correct identifier
                    return null;

                default:
                    return Issue(identifier.ValueText, identifier.GetLocation(), ExceptionIdentifier);
            }
        }
    }
}