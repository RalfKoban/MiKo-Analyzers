using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3121_ObjectUnderTestIsNoInterfaceAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3121";

        public MiKo_3121_ObjectUnderTestIsNoInterfaceAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            base.InitializeCore(context);

            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsTestClass();

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            var members = symbol.GetTypeUnderTestMembers();

            foreach (var member in members)
            {
                var type = member.GetReturnTypeSymbol();

                if (type?.TypeKind == TypeKind.Interface)
                {
                    var syntax = GetTypeSyntax(member);

                    if (syntax != null)
                    {
                        yield return Issue(syntax);
                    }
                }
            }
        }

        private static SyntaxNode GetTypeSyntax(ISymbol symbol)
        {
            switch (symbol)
            {
                case IMethodSymbol method: return method.GetSyntax<MethodDeclarationSyntax>()?.ReturnType;
                case IPropertySymbol property: return property.GetSyntax<BasePropertyDeclarationSyntax>()?.Type;
                case IFieldSymbol field: return field.GetSyntax<BaseFieldDeclarationSyntax>()?.Declaration.Type;
                default: return null;
            }
        }

        private void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (LocalDeclarationStatementSyntax)context.Node;
            var declaration = node.Declaration;

            if (declaration.Variables.Any(_ => _.IsTypeUnderTestVariable()))
            {
                // inspect associated test method
                var method = context.GetEnclosingMethod();

                if (method.IsTestMethod())
                {
                    var typeUnderTest = declaration.GetTypeSymbol(context.SemanticModel);

                    if (typeUnderTest?.TypeKind == TypeKind.Interface)
                    {
                        var issue = Issue(declaration.Type);

                        ReportDiagnostics(context, issue);
                    }
                }
            }
        }
    }
}