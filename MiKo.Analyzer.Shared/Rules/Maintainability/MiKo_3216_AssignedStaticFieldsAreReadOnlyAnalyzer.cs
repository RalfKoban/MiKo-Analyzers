using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3216_AssignedStaticFieldsAreReadOnlyAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3216";

        public MiKo_3216_AssignedStaticFieldsAreReadOnlyAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);

        private void AnalyzeField(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is FieldDeclarationSyntax field)
            {
                var modifiers = field.Modifiers;

                if (modifiers.Any(SyntaxKind.StaticKeyword) && modifiers.None(SyntaxKind.ReadOnlyKeyword))
                {
                    foreach (var variable in field.Declaration.Variables)
                    {
                        if (variable.Initializer != null)
                        {
                            ReportDiagnostics(context, Issue(variable.Identifier));
                        }
                    }
                }
            }
        }
    }
}