using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_0003_LinesOfCodeInClassAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0003";

        public MiKo_0003_LinesOfCodeInClassAnalyzer() : base(Id, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.RecordDeclaration, SyntaxKind.RecordStructDeclaration)
        {
        }

        public int MaxLinesOfCode { get; set; } = 220;

        protected override void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is TypeDeclarationSyntax declaration)
            {
                if (declaration.IsGenerated())
                {
                    // ignore generated code
                    return;
                }

                if (declaration.IsTestClass())
                {
                    // ignore test classes
                    return;
                }

                var issues = AnalyzeDeclarations(declaration);

                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeDeclarations(TypeDeclarationSyntax declaration)
        {
            var loc = 0;

            foreach (var member in declaration.Members)
            {
                switch (member)
                {
                    case BaseMethodDeclarationSyntax _:
                    case BasePropertyDeclarationSyntax _:
                    {
                        foreach (var block in member.DescendantNodes<BlockSyntax>())
                        {
                            loc += Counter.CountLinesOfCode(block);
                        }

                        break;
                    }
                }
            }

            if (loc > MaxLinesOfCode)
            {
                return new[] { Issue(declaration.Identifier.GetLocation(), loc, MaxLinesOfCode) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}