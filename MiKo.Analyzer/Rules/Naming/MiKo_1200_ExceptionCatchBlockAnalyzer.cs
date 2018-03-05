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

        private const string ExceptionIdentifier = "ex";

        public MiKo_1200_ExceptionCatchBlockAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeCatchBlock, SyntaxKind.CatchClause);
        }

        private void AnalyzeCatchBlock(SyntaxNodeAnalysisContext context)
        {
            var node = (CatchClauseSyntax)context.Node;
            var diagnostic = AnalyzeCatchClause(node);
            if (diagnostic != null) context.ReportDiagnostic(diagnostic);
        }

        private Diagnostic AnalyzeCatchClause(CatchClauseSyntax node)
        {
            if (node.Declaration is null) return null; // we don't have an exception

            var identifier = node.Declaration.Identifier;
            switch (identifier.ValueText)
            {
                case null: // we don't have one
                case ExceptionIdentifier: // correct identifier
                    return null;

                default:
                    return ReportIssue(identifier.ValueText, identifier.GetLocation(), ExceptionIdentifier);
            }
        }
    }
}