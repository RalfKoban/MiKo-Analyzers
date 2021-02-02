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

        protected Analyzer(string category, string diagnosticId)
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
                                                                      IsEnabledByDefault,
                                                                      LocalizableResource(id, "Description"),
                                                                      LocalizableResource(id, "HelpLinkUri")?.ToString()));
        }

        protected Analyzer(string category, string diagnosticId, SymbolKind symbolKind) : this(category, diagnosticId) => SymbolKind = symbolKind;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public string DiagnosticId { get; }

        protected DiagnosticDescriptor Rule { get; }

        protected SymbolKind SymbolKind { get; }

        protected virtual bool IsEnabledByDefault => true;

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            // TODO: RKN remove this if that's not possible
            context.EnableConcurrentExecution();

            InitializeCore(context);
        }

        protected virtual void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind);

        protected void InitializeCore(AnalysisContext context, params SymbolKind[] symbolKinds)
        {
            foreach (var symbolKind in symbolKinds)
            {
                var action = GetAnalyzeMethod(symbolKind);
                if (action != null)
                {
                    context.RegisterSymbolAction(action, symbolKind);
                }
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeNamespace(SymbolAnalysisContext context) => ReportDiagnostics<INamespaceSymbol>(context, AnalyzeNamespace);

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeType(SymbolAnalysisContext context) => ReportDiagnostics<INamedTypeSymbol>(context, AnalyzeType);

        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeEvent(SymbolAnalysisContext context) => ReportDiagnostics<IEventSymbol>(context, AnalyzeEvent);

        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeField(SymbolAnalysisContext context) => ReportDiagnostics<IFieldSymbol>(context, AnalyzeField);

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeMethod(SymbolAnalysisContext context) => ReportDiagnostics<IMethodSymbol>(context, AnalyzeMethod);

        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeProperty(SymbolAnalysisContext context) => ReportDiagnostics<IPropertySymbol>(context, AnalyzeProperty);

        protected virtual IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeParameter(SymbolAnalysisContext context) => ReportDiagnostics<IParameterSymbol>(context, AnalyzeParameter);

        protected Diagnostic Issue(ISymbol symbol) => CreateIssue(symbol.Locations[0], GetSymbolName(symbol));

        protected Diagnostic Issue(SyntaxNode node) => CreateIssue(node.GetLocation(), node.ToString());

        protected Diagnostic Issue(SyntaxToken token) => CreateIssue(token.GetLocation(), token.ValueText);

        protected Diagnostic Issue(SyntaxNode node, ImmutableDictionary<string, string> properties) => CreateIssue(node.GetLocation(), properties, node.ToString());

        protected Diagnostic Issue<T>(ISymbol symbol, T arg) => CreateIssue(symbol.Locations[0], GetSymbolName(symbol), arg.ToString());

        protected Diagnostic Issue(string name, ISymbol symbol) => CreateIssue(symbol.Locations[0], name);

        protected Diagnostic Issue(string name, SyntaxNode node) => CreateIssue(node.GetLocation(), name);

        protected Diagnostic Issue(string name, SyntaxToken token) => CreateIssue(token.GetLocation(), name);

        protected Diagnostic Issue(string name, SyntaxTrivia trivia) => CreateIssue(trivia.GetLocation(), name);

        protected Diagnostic Issue(string name, Location location) => CreateIssue(location, name);

        protected Diagnostic Issue<T>(string name, SyntaxNode node, T arg1) => CreateIssue(node.GetLocation(), name, arg1.ToString());

        protected Diagnostic Issue<T>(string name, SyntaxToken token, T arg1) => CreateIssue(token.GetLocation(), name, arg1.ToString());

        protected Diagnostic Issue<T>(string name, Location location, T arg1) => CreateIssue(location, name, arg1.ToString());

        protected Diagnostic Issue<T1, T2>(ISymbol symbol, T1 arg1, T2 arg2) => CreateIssue(symbol.Locations[0], GetSymbolName(symbol), arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2>(string name, ISymbol symbol, T1 arg1, T2 arg2) => CreateIssue(symbol.Locations[0], name, arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2>(string name, SyntaxNode node, T1 arg1, T2 arg2) => CreateIssue(node.GetLocation(), name, arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2>(string name, SyntaxToken token, T1 arg1, T2 arg2) => CreateIssue(token.GetLocation(), name, arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2, T3>(ISymbol symbol, T1 arg1, T2 arg2, T3 arg3) => CreateIssue(symbol.Locations[0], GetSymbolName(symbol), arg1.ToString(), arg2.ToString(), arg3.ToString());

        private static string GetSymbolName(ISymbol symbol)
        {
            if (symbol is IMethodSymbol m)
            {
                if (m.MethodKind == MethodKind.Constructor || m.MethodKind == MethodKind.StaticConstructor)
                {
                    return symbol.ContainingSymbol.Name + symbol.Name;
                }
            }

            return symbol.Name;
        }

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static LocalizableResourceString LocalizableResource(string id, string suffix) => new LocalizableResourceString(id + "_" + suffix, Resources.ResourceManager, typeof(Resources));

        private static void ReportDiagnostics<T>(SymbolAnalysisContext context, Func<T, IEnumerable<Diagnostic>> analyzer) where T : ISymbol
        {
            var symbol = context.Symbol;

            var diagnostics = analyzer((T)symbol);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic CreateIssue(Location location, ImmutableDictionary<string, string> properties, params object[] args) => Diagnostic.Create(Rule, location, properties, args);

        private Diagnostic CreateIssue(Location location, params object[] args) => Diagnostic.Create(Rule, location, args);

        private Action<SymbolAnalysisContext> GetAnalyzeMethod(SymbolKind symbolKind)
        {
            switch (symbolKind)
            {
                case SymbolKind.Event: return AnalyzeEvent;
                case SymbolKind.Field: return AnalyzeField;
                case SymbolKind.Method: return AnalyzeMethod;
                case SymbolKind.NamedType: return AnalyzeType;
                case SymbolKind.Namespace: return AnalyzeNamespace;
                case SymbolKind.Property: return AnalyzeProperty;
                case SymbolKind.Parameter: return AnalyzeParameter;

                default: return null;
            }
        }
    }
}