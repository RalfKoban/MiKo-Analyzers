using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class TypeSyntaxNamingAnalyzer : NamingAnalyzer
    {
        private static readonly SyntaxKind[] TypeKinds =
                                                         {
                                                             SyntaxKind.ClassDeclaration,
                                                             SyntaxKind.InterfaceDeclaration,
                                                             SyntaxKind.StructDeclaration,
                                                             SyntaxKind.RecordDeclaration,
                                                             SyntaxKind.RecordStructDeclaration,
                                                             SyntaxKind.EnumDeclaration,
                                                         };

        protected TypeSyntaxNamingAnalyzer(string diagnosticId) : base(diagnosticId)
        {
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSyntax, TypeKinds);

        protected abstract Diagnostic[] AnalyzeName(string typeName, in SyntaxToken typeNameIdentifier, BaseTypeDeclarationSyntax declaration);

        protected virtual bool ShallAnalyze(string typeName, BaseTypeDeclarationSyntax declaration) => true;

        protected Diagnostic Issue(in SyntaxToken typeNameIdentifier, string betterName)
        {
            var typeName = typeNameIdentifier.ValueText;

            return Issue(typeName, typeNameIdentifier, betterName, CreateBetterNameProposal(betterName));
        }

        protected Diagnostic Issue(in SyntaxToken typeNameIdentifier, string betterName, string issue)
        {
            var typeName = typeNameIdentifier.ValueText;

            return Issue(typeName, typeNameIdentifier, issue, CreateBetterNameProposal(betterName));
        }

        private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BaseTypeDeclarationSyntax declaration)
            {
                var typeNameIdentifier = declaration.Identifier;
                var typeName = typeNameIdentifier.ValueText;

                if (ShallAnalyze(typeName, declaration))
                {
                    var issues = AnalyzeName(typeName, typeNameIdentifier, declaration);

                    if (issues.Length > 0)
                    {
                        ReportDiagnostics(context, issues);
                    }
                }
            }
        }
    }
}