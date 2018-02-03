using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    public abstract class MetricsAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Metrics";

        protected MetricsAnalyzer(string diagnosticId)
        {
            // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
            var title = LocalizableResource("Title");
            var messageFormat = LocalizableResource("MessageFormat");
            var description = LocalizableResource("Description");

            Rule = new DiagnosticDescriptor(diagnosticId, title, messageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: description);
        }

        protected DiagnosticDescriptor Rule { get; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public override void Initialize(AnalysisContext context) => context.RegisterCodeBlockAction(AnalyzeCodeBlock);

        protected abstract Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol);

        private void AnalyzeCodeBlock(CodeBlockAnalysisContext context)
        {
            var body = GetBody(context);
            if (body is null) return;

            var diagnostic = AnalyzeBody(body, context.OwningSymbol);
            if (diagnostic is null) return;

            context.ReportDiagnostic(diagnostic);
        }

        private BlockSyntax GetBody(CodeBlockAnalysisContext context)
        {
            switch (context.CodeBlock)
            {
                case MethodDeclarationSyntax s: return s.Body;
                case ConstructorDeclarationSyntax s: return s.Body;
                case AccessorDeclarationSyntax s: return s.Body;
                default: return null;
            }
        }

        private LocalizableResourceString LocalizableResource(string suffix) => new LocalizableResourceString(GetType().Name + "_" + suffix, Resources.ResourceManager, typeof(Resources));
    }
}