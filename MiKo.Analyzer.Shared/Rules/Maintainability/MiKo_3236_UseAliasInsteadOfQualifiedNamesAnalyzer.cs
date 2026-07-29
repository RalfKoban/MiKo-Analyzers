using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3236_UseAliasInsteadOfQualifiedNamesAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3236";

        public MiKo_3236_UseAliasInsteadOfQualifiedNamesAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeQualifiedName, SyntaxKind.QualifiedName);

        private static bool HasIssue(QualifiedNameSyntax name, in SyntaxNodeAnalysisContext context)
        {
            switch (name.Parent)
            {
                case UsingDirectiveSyntax _: // usings are allowed
                case QualifiedNameSyntax _: // nested qualified names are allowed
                case QualifiedCrefSyntax _: // qualified names in XML documentations are allowed
                case NamespaceDeclarationSyntax _: // namespaces are allowed
#if VS2022 || VS2026
                case FileScopedNamespaceDeclarationSyntax _: // namespaces are allowed
#endif
                    return false;
            }

            var identifier = name.FirstDescendant<IdentifierNameSyntax>();
            var type = identifier.GetTypeSymbol(context.SemanticModel);

            switch (type?.TypeKind)
            {
                case TypeKind.Class: // nested classes are allowed
                case TypeKind.Struct: // nested structs are allowed
                case TypeKind.Enum: // enums are allowed
                    return false;
            }

            if (name.FirstAncestor<UsingDirectiveSyntax>() is UsingDirectiveSyntax directive && directive.Alias != null)
            {
                return false; // aliases using full qualified types are allowed
            }

            return true;
        }

        private void AnalyzeQualifiedName(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is QualifiedNameSyntax node && HasIssue(node, context))
            {
                ReportDiagnostics(context, Issue(node));
            }
        }
    }
}