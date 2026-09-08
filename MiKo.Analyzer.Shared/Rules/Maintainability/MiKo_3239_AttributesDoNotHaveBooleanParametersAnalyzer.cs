using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3239_AttributesDoNotHaveBooleanParametersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3239";

        public MiKo_3239_AttributesDoNotHaveBooleanParametersAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.ClassDeclaration);

        private static bool ShallAnalyze(ClassDeclarationSyntax node, SemanticModel semanticModel) => node.BaseList != null && node.GetTypeSymbol(semanticModel).InheritsFrom<Attribute>();

        private void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax node && ShallAnalyze(node, context.SemanticModel))
            {
                var issues = AnalyzeAttribute(node);

                if (issues.Length > 0)
                {
                    ReportDiagnostics(context, issues);
                }
            }
        }

        private Diagnostic[] AnalyzeAttribute(ClassDeclarationSyntax node)
        {
            List<Diagnostic> issues = null;

#if VS2022 || VS2026

            var parameters = node.ParameterList?.Parameters;

            if (parameters != null)
            {
                // this is a primary constructor
                foreach (var parameter in parameters)
                {
                    if (parameter.Type.IsBoolean())
                    {
                        if (issues is null)
                        {
                            issues = new List<Diagnostic>(1);
                        }

                        issues.Add(Issue(parameter));
                    }
                }
            }

#endif

            foreach (var ctor in node.ChildNodes<ConstructorDeclarationSyntax>())
            {
                var ctorParameters = ctor.ParameterList.Parameters;

                for (int index = 0, count = ctorParameters.Count; index < count; index++)
                {
                    ParameterSyntax parameter = ctorParameters[index];

                    if (parameter.Type.IsBoolean())
                    {
                        if (issues is null)
                        {
                            issues = new List<Diagnostic>(1);
                        }

                        issues.Add(Issue(parameter));
                    }
                }
            }

            return issues?.ToArray() ?? Array.Empty<Diagnostic>();
        }
    }
}