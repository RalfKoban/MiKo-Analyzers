using System.Collections.Concurrent;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    public abstract class MetricsAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Metrics";

        private static readonly ConcurrentDictionary<string, DiagnosticDescriptor> KnownRules = new ConcurrentDictionary<string, DiagnosticDescriptor>();

        protected MetricsAnalyzer(string diagnosticId)
        {
            Rule = KnownRules.GetOrAdd(
                                       GetType().Name,
                                       name => new DiagnosticDescriptor(
                                                                        diagnosticId,
                                                                        LocalizableResource(name, "Title"),
                                                                        LocalizableResource(name, "MessageFormat"),
                                                                        Category,
                                                                        DiagnosticSeverity.Warning,
                                                                        isEnabledByDefault: true,
                                                                        description: LocalizableResource(name, "Description")));
        }

        protected DiagnosticDescriptor Rule { get; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public override void Initialize(AnalysisContext context) => context.RegisterCodeBlockAction(AnalyzeCodeBlock);

        protected abstract Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol);

        protected bool TryCreateDiagnostic(ISymbol owningSymbol, int metric, int limit, out Diagnostic diagnostic)
        {
            diagnostic = metric > limit ? Diagnostic.Create(Rule, owningSymbol.Locations[0], owningSymbol.Name, metric, limit) : null;
            return diagnostic != null;
        }

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

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private LocalizableResourceString LocalizableResource(string prefix, string suffix) => new LocalizableResourceString(prefix + "_" + suffix, Resources.ResourceManager, typeof(Resources));
    }
}