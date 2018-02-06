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

        protected Analyzer(string category, string diagnosticId, SymbolKind symbolKind) : this(category, diagnosticId) => SymbolKind = symbolKind;

        protected Analyzer(string category, string diagnosticId) => Rule = KnownRules.GetOrAdd(
                                                                                           GetType().Name,
                                                                                           name => new DiagnosticDescriptor(
                                                                                                                            diagnosticId,
                                                                                                                            LocalizableResource(name, "Title"),
                                                                                                                            LocalizableResource(name, "MessageFormat"),
                                                                                                                            category,
                                                                                                                            DiagnosticSeverity.Warning,
                                                                                                                            isEnabledByDefault: true,
                                                                                                                            description: LocalizableResource(name, "Description")));

        protected DiagnosticDescriptor Rule { get; }

        protected SymbolKind SymbolKind { get; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public override void Initialize(AnalysisContext context)
        {
            var action = GetAnalyzeMethod(SymbolKind);
            if (action != null) context.RegisterSymbolAction(action, SymbolKind);
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeType(SymbolAnalysisContext context)
        {
            var diagnostics = AnalyzeType((INamedTypeSymbol)context.Symbol);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeMethod(SymbolAnalysisContext context)
        {
            if (context.Symbol.GetAttributes().Any(_ => _.AttributeClass.Name == typeof(System.CodeDom.Compiler.GeneratedCodeAttribute).FullName)) return;

            var diagnostics = AnalyzeMethod((IMethodSymbol)context.Symbol);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        protected Diagnostic ReportIssue(ISymbol symbol, params object[] messageArgs)
        {
            var args = new List<object> { symbol.Name };
            args.AddRange(messageArgs);

            return Diagnostic.Create(Rule, symbol.Locations[0], args);
        }

        private Action<SymbolAnalysisContext> GetAnalyzeMethod(SymbolKind symbolKind)
        {
            switch (symbolKind)
            {
                case SymbolKind.Method: return AnalyzeMethod;
                case SymbolKind.NamedType: return AnalyzeType;
                default: return null;
            }
        }

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static LocalizableResourceString LocalizableResource(string prefix, string suffix) => new LocalizableResourceString(prefix + "_" + suffix, Resources.ResourceManager, typeof(Resources));
    }
}