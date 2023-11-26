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
    public sealed class MiKo_3214_BeginEndScopeMethodsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3214";

        private static readonly string[] ScopeIndicators = { "Begin", "End", "Enter", "Exit", "Leave" };

        public MiKo_3214_BeginEndScopeMethodsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInterface, SyntaxKind.InterfaceDeclaration);

        private void AnalyzeInterface(SyntaxNodeAnalysisContext context)
        {
            var issues = Analyze((InterfaceDeclarationSyntax)context.Node);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> Analyze(TypeDeclarationSyntax syntax)
        {
            foreach (var method in syntax.Members.OfType<MethodDeclarationSyntax>())
            {
                var identifier = method.Identifier;
                var name = identifier.ValueText;

                var indicator = ScopeIndicators.FirstOrDefault(_ => name.StartsWith(_, StringComparison.OrdinalIgnoreCase));

                if (indicator != null)
                {
                    yield return Issue(identifier, indicator);
                }
            }
        }
    }
}