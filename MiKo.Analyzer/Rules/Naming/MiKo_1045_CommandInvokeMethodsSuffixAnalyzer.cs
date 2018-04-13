using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1045_CommandInvokeMethodsSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1045";

        private const string Suffix = "Command";

        public MiKo_1045_CommandInvokeMethodsSuffixAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);

        private void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var node = (ObjectCreationExpressionSyntax)context.Node;
            var type = node.Type.ToString();
            if (!type.Contains(Suffix)) return;

            var diagnostics = AnalyzeObjectCreation(node);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node)
        {
            var arguments = node.ArgumentList.Arguments;
            return arguments.Any()
                       ? arguments.Where(_ => _.ToString().EndsWith(Suffix, StringComparison.Ordinal))
                                  .Select(_ => ReportIssue(_.ToString(), _.GetLocation(), Suffix))
                                  .ToList()
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}