using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4007_OperatorsOrderedBeforeMethodsAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4007";

        private static readonly SyntaxKind[] OperatorDeclarations = { SyntaxKind.OperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration };

        private static readonly SyntaxKind[] TypeDeclarations = { SyntaxKind.ClassDeclaration, SyntaxKind.RecordDeclaration, SyntaxKind.StructDeclaration };

        public MiKo_4007_OperatorsOrderedBeforeMethodsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeType, TypeDeclarations);

        private void AnalyzeType(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BaseTypeDeclarationSyntax type)
            {
                var issues = AnalyzeType(type);

                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeType(SyntaxNode type)
        {
            var children = type.ChildNodes().ToList();

            var referenceMethod = children.FirstOrDefault(_ => _ is MethodDeclarationSyntax);

            if (referenceMethod != null)
            {
                var referenceStartingLine = referenceMethod.GetStartingLine();

                foreach (var child in children)
                {
                    if (child.IsAnyKind(OperatorDeclarations))
                    {
                        var startingLine = child.GetStartingLine();

                        if (startingLine > referenceStartingLine)
                        {
                            yield return Issue(child);
                        }
                    }
                }
            }
        }
    }
}