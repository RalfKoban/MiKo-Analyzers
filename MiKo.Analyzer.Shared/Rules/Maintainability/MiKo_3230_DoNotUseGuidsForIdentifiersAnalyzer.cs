using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3230_DoNotUseGuidsForIdentifiersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3230";

        private const string WrongParameterName = "id";

        private static readonly string[] WrongNameSuffixes =
                                                             {
                                                                 "Id",
                                                                 "ID",
                                                                 "dentifier", // ignore the starting 'I' to avoid ignore-case comparisons
                                                                 "dentifer", // typo in name, ignore the starting 'I' to avoid ignore-case comparisons
                                                             };

        private static readonly SyntaxKind[] SyntaxKinds = { SyntaxKind.PropertyDeclaration, SyntaxKind.Parameter };

        public MiKo_3230_DoNotUseGuidsForIdentifiersAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(Analyze, SyntaxKinds);

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case PropertyDeclarationSyntax property:
                    Analyze(context, property);

                    break;

                case ParameterSyntax parameter:
                    Analyze(context, parameter);

                    break;
            }
        }

        private void Analyze(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property)
        {
            var type = property.Type;

            if (type.IsGuid() && property.GetName().EndsWithAny(WrongNameSuffixes, StringComparison.Ordinal))
            {
                context.ReportDiagnostic(Issue(type));
            }
        }

        private void Analyze(SyntaxNodeAnalysisContext context, ParameterSyntax parameter)
        {
            var type = parameter.Type;

            if (type.IsGuid())
            {
                var name = parameter.GetName();

                if (name == WrongParameterName || name.EndsWithAny(WrongNameSuffixes, StringComparison.Ordinal))
                {
                    context.ReportDiagnostic(Issue(type));
                }
            }
        }
    }
}