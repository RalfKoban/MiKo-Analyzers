using System.Collections.Concurrent;

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

        private static readonly ConcurrentDictionary<string, TypeKind> Cache = new ConcurrentDictionary<string, TypeKind>();

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

            if (name.FirstAncestor<UsingDirectiveSyntax>() is UsingDirectiveSyntax directive && directive.Alias != null)
            {
                return false; // aliases using full qualified types are allowed
            }

            var fullName = name.ToString();

            // assume that fully qualified names will not change their type, so we cache it here to avoid costly calls for the type symbols
            if (Cache.TryGetValue(fullName, out var typeKind) is false)
            {
                var identifier = name.FirstDescendant<IdentifierNameSyntax>();
                var type = identifier.GetTypeSymbol(context.SemanticModel);
                typeKind = type?.TypeKind ?? TypeKind.Unknown;

                Cache.TryAdd(fullName, typeKind);
            }

            switch (typeKind)
            {
                case TypeKind.Class: // nested classes are allowed
                case TypeKind.Struct: // nested structs are allowed
                case TypeKind.Enum: // enums are allowed
                    return false;

                default:
                    return true;
            }
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