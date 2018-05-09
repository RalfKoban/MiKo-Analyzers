using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules
{
    public abstract class Analyzer : DiagnosticAnalyzer
    {
        private static readonly ConcurrentDictionary<string, DiagnosticDescriptor> KnownRules = new ConcurrentDictionary<string, DiagnosticDescriptor>();

        protected Analyzer(string category, string diagnosticId, SymbolKind symbolKind, bool isEnabledByDefault = true) : this(category, diagnosticId, isEnabledByDefault) => SymbolKind = symbolKind;

        protected Analyzer(string category, string diagnosticId, bool isEnabledByDefault = true)
        {
            DiagnosticId = diagnosticId;

            Rule = KnownRules.GetOrAdd(
                                       diagnosticId,
                                       id => new DiagnosticDescriptor(
                                                                      id,
                                                                      LocalizableResource(id, "Title"),
                                                                      LocalizableResource(id, "MessageFormat"),
                                                                      category,
                                                                      DiagnosticSeverity.Warning,
                                                                      isEnabledByDefault,
                                                                      LocalizableResource(id, "Description"),
                                                                      LocalizableResource(id, "HelpLinkUri")?.ToString()));
        }

        public string DiagnosticId { get; }

        protected DiagnosticDescriptor Rule { get; }

        private SymbolKind SymbolKind { get; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            InitializeCore(context);
        }

        protected virtual void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind);

        protected void InitializeCore(AnalysisContext context, params SymbolKind[] symbolKinds)
        {
            foreach (var symbolKind in symbolKinds)
            {
                var action = GetAnalyzeMethod(symbolKind);
                if (action != null)
                    context.RegisterSymbolAction(action, symbolKind);
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeType(SymbolAnalysisContext context) => ReportDiagnostics<INamedTypeSymbol>(context, AnalyzeType);

        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeEvent(SymbolAnalysisContext context) => ReportDiagnostics<IEventSymbol>(context, AnalyzeEvent);

        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeField(SymbolAnalysisContext context) => ReportDiagnostics<IFieldSymbol>(context, AnalyzeField);

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeMethod(SymbolAnalysisContext context) => ReportDiagnostics<IMethodSymbol>(context, AnalyzeMethod);

        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeProperty(SymbolAnalysisContext context) => ReportDiagnostics<IPropertySymbol>(context, AnalyzeProperty);

        protected virtual IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeParameter(SymbolAnalysisContext context) => ReportDiagnostics<IParameterSymbol>(context, AnalyzeParameter);

        protected Diagnostic ReportIssue(ISymbol symbol, params object[] messageArgs)
        {
            var prefix = string.Empty;

            if (symbol is IMethodSymbol m && (m.MethodKind == MethodKind.StaticConstructor || m.MethodKind == MethodKind.Constructor))
                prefix = symbol.ContainingSymbol.Name;

            return ReportIssue(prefix + symbol.Name, symbol.Locations[0], messageArgs);
        }

        protected Diagnostic ReportIssue(string name, Location location, params object[] messageArgs)
        {
            var args = new object[messageArgs.Length + 1];
            args[0] = name;
            messageArgs.CopyTo(args, 1);

            return Diagnostic.Create(Rule, location, args);
        }

        private Action<SymbolAnalysisContext> GetAnalyzeMethod(SymbolKind symbolKind)
        {
            switch (symbolKind)
            {
                case SymbolKind.Event: return AnalyzeEvent;
                case SymbolKind.Field: return AnalyzeField;
                case SymbolKind.Method: return AnalyzeMethod;
                case SymbolKind.NamedType: return AnalyzeType;
                case SymbolKind.Property: return AnalyzeProperty;
                case SymbolKind.Parameter: return AnalyzeParameter;

                default: return null;
            }
        }

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static LocalizableResourceString LocalizableResource(string id, string suffix) => new LocalizableResourceString(id + "_" + suffix, Resources.ResourceManager, typeof(Resources));

        private static readonly string GeneratedCodeMarker = typeof(System.CodeDom.Compiler.GeneratedCodeAttribute).FullName;

        private static void ReportDiagnostics<T>(SymbolAnalysisContext context, Func<T, IEnumerable<Diagnostic>> analyzer) where T : ISymbol
        {
            var symbol = context.Symbol;

            // filter generated code
            // var symbols = new[] { symbol, symbol.ContainingSymbol, symbol.ContainingType };
            // if (symbols.Any(s => s?.GetAttributes().Any(_ => _.AttributeClass.Name == GeneratedCodeMarker) == true)) return;

            var diagnostics = analyzer((T)symbol);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}