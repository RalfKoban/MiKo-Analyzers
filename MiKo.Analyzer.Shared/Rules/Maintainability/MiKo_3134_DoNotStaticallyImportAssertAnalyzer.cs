using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3134_DoNotStaticallyImportAssertAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3134";

        public MiKo_3134_DoNotStaticallyImportAssertAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);

        private void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is UsingDirectiveSyntax directive && directive.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
            {
                var name = directive.LastDescendant<IdentifierNameSyntax>().GetName();

                if (Constants.Names.AssertionTypes.Contains(name))
                {
                    ReportDiagnostics(context, Issue(directive));
                }
            }
        }
    }
}