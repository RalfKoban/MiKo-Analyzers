using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_0008_TooManyUsingsAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0008";

        private static readonly SyntaxKind[] SyntaxKinds =
                                                           {
                                                               SyntaxKind.CompilationUnit,
                                                               SyntaxKind.NamespaceDeclaration,
#if VS2022 || VS2026
                                                               SyntaxKind.FileScopedNamespaceDeclaration,
#endif
                                                           };

        public MiKo_0008_TooManyUsingsAnalyzer() : base(Id, SyntaxKinds)
        {
        }

        public int AllowedUsings { get; set; } = 15;

        protected override void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var usingDirectives = context.Node.ChildNodes<UsingDirectiveSyntax>().Skip(AllowedUsings).ToArray();
            var usingsAboveLimit = usingDirectives.Length;

            if (usingsAboveLimit > 0)
            {
                foreach (var usingDirective in usingDirectives)
                {
                    ReportDiagnostics(context, Issue(usingDirective, usingsAboveLimit, AllowedUsings));
                }
            }
        }
    }
}