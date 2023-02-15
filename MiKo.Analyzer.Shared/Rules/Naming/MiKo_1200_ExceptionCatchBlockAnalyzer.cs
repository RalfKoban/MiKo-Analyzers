using System.Collections.Generic;
using System.Linq;

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

        private const string ExceptionIdentifier = "IDENTIFIER";

        public MiKo_1200_ExceptionCatchBlockAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        internal static string FindBetterName(ISymbol symbol, Diagnostic diagnostic) => diagnostic.Properties[ExceptionIdentifier];

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeCatchBlock, SyntaxKind.CatchClause);
        }

        private void AnalyzeCatchBlock(SyntaxNodeAnalysisContext context)
        {
            var node = (CatchClauseSyntax)context.Node;
            var issue = AnalyzeCatchClause(node);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeCatchClause(CatchClauseSyntax node) => AnalyzeCatchDeclaration(node.Declaration);

        private Diagnostic AnalyzeCatchDeclaration(CatchDeclarationSyntax node)
        {
            if (node is null)
            {
                return null; // we do not have an exception
            }

            var identifier = node.Identifier;
            var name = identifier.ValueText;

            switch (name)
            {
                case null: // we do not have one
                case "": // we do not have one
                case Constants.ExceptionIdentifier: // correct identifier
                    return null;

                default:

                    var expectedIdentifier = Constants.ExceptionIdentifier;

                    if (node.Parent.Ancestors<CatchClauseSyntax>().Any(_ => _.Declaration?.Identifier.ValueText == Constants.ExceptionIdentifier))
                    {
                        if (name == Constants.InnerExceptionIdentifier)
                        {
                            return null;
                        }

                        expectedIdentifier = Constants.InnerExceptionIdentifier;
                    }

                    return Issue(name, identifier, expectedIdentifier, new Dictionary<string, string> { { ExceptionIdentifier, expectedIdentifier } });
            }
        }
    }
}