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

        protected virtual bool CanRunConcurrently => true;

        protected virtual bool IsUnitTestAnalyzer => false;

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            if (CanRunConcurrently)
            {
                context.EnableConcurrentExecution();
            }

            context.RegisterCompilationStartAction(compilationContext =>
                                                       {
                                                           if (IsUnitTestAnalyzer)
                                                           {
                                                               if (ReferencesTestAssemblies(compilationContext.Compilation) is false)
                                                               {
                                                                   // do not run analyzer if there are no tests contained
                                                                   return;
                                                               }
                                                           }

                                                           InitializeCore(compilationContext);
                                                       });
        }

        protected static void ReportDiagnostics(SymbolAnalysisContext context, params Diagnostic[] issues) => ReportDiagnostics(context, (IEnumerable<Diagnostic>)issues);

        protected static void ReportDiagnostics(SymbolAnalysisContext context, IEnumerable<Diagnostic> issues)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                // seems that we should cancel and not report further issues
                return;
            }

            foreach (var issue in issues)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    // seems that we should cancel and not report further issues
                    return;
                }

                if (issue != null)
                {
                    context.ReportDiagnostic(issue);
                }
            }
        }

        protected static void ReportDiagnostics(SyntaxNodeAnalysisContext context, params Diagnostic[] issues) => ReportDiagnostics(context, (IEnumerable<Diagnostic>)issues);

        protected static void ReportDiagnostics(SyntaxNodeAnalysisContext context, IEnumerable<Diagnostic> issues)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                // seems that we should cancel and not report further issues
                return;
            }

            foreach (var issue in issues)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    // seems that we should cancel and not report further issues
                    return;
                }

                if (issue != null)
                {
                    context.ReportDiagnostic(issue);
                }
            }
        }

        protected virtual void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind);

        protected void InitializeCore(CompilationStartAnalysisContext context, params SymbolKind[] symbolKinds)
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

        protected void AnalyzeNamespace(SymbolAnalysisContext context) => ReportDiagnostics<INamespaceSymbol>(context, AnalyzeNamespace);

        protected virtual IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeType(SymbolAnalysisContext context) => ReportDiagnostics<INamedTypeSymbol>(context, AnalyzeType);

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeEvent(SymbolAnalysisContext context) => ReportDiagnostics<IEventSymbol>(context, AnalyzeEvent);

        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeField(SymbolAnalysisContext context) => ReportDiagnostics<IFieldSymbol>(context, AnalyzeField);

        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeMethod(SymbolAnalysisContext context) => ReportDiagnostics<IMethodSymbol>(context, AnalyzeMethod);

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeProperty(SymbolAnalysisContext context) => ReportDiagnostics<IPropertySymbol>(context, AnalyzeProperty);

        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected void AnalyzeParameter(SymbolAnalysisContext context) => ReportDiagnostics<IParameterSymbol>(context, AnalyzeParameter);

        protected virtual IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected Diagnostic Issue(ISymbol symbol, Dictionary<string, string> properties = null) => CreateIssue(symbol.Locations[0], properties, GetSymbolName(symbol));

        protected Diagnostic Issue(SyntaxNode node, Dictionary<string, string> properties = null) => CreateIssue(node.GetLocation(), properties, node.ToString());

        protected Diagnostic Issue(SyntaxToken token, Dictionary<string, string> properties = null) => CreateIssue(token.GetLocation(), properties, token.ValueText);

        protected Diagnostic Issue<T>(ISymbol symbol, T arg, Dictionary<string, string> properties = null) => CreateIssue(symbol.Locations[0], properties, GetSymbolName(symbol), arg.ToString());

        protected Diagnostic Issue(string name, ISymbol symbol, Dictionary<string, string> properties = null) => CreateIssue(symbol.Locations[0], properties, name);

        protected Diagnostic Issue(string name, SyntaxNode node, Dictionary<string, string> properties = null) => CreateIssue(node.GetLocation(), properties, name);

        protected Diagnostic Issue(string name, SyntaxToken token, Dictionary<string, string> properties = null) => CreateIssue(token.GetLocation(), properties, name);

        protected Diagnostic Issue(string name, SyntaxTrivia trivia, Dictionary<string, string> properties = null) => CreateIssue(trivia.GetLocation(), properties, name);

        protected Diagnostic Issue(string name, Location location, Dictionary<string, string> properties = null) => CreateIssue(location, properties, name);

        protected Diagnostic Issue<T>(string name, SyntaxNode node, T arg1, Dictionary<string, string> properties = null) => CreateIssue(node.GetLocation(), properties, name, arg1.ToString());

        protected Diagnostic Issue<T>(string name, SyntaxToken token, T arg1, Dictionary<string, string> properties = null) => CreateIssue(token.GetLocation(), properties, name, arg1.ToString());

        protected Diagnostic Issue<T>(string name, Location location, T arg1, Dictionary<string, string> properties = null) => CreateIssue(location, properties, name, arg1.ToString());

        protected Diagnostic Issue<T1, T2>(ISymbol symbol, T1 arg1, T2 arg2, Dictionary<string, string> properties = null) => CreateIssue(symbol.Locations[0], properties, GetSymbolName(symbol), arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2>(string name, ISymbol symbol, T1 arg1, T2 arg2, Dictionary<string, string> properties = null) => CreateIssue(symbol.Locations[0], properties, name, arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2>(string name, SyntaxNode node, T1 arg1, T2 arg2, Dictionary<string, string> properties = null) => CreateIssue(node.GetLocation(), properties, name, arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2>(string name, SyntaxToken token, T1 arg1, T2 arg2, Dictionary<string, string> properties = null) => CreateIssue(token.GetLocation(), properties, name, arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2, T3>(ISymbol symbol, T1 arg1, T2 arg2, T3 arg3, Dictionary<string, string> properties = null) => CreateIssue(symbol.Locations[0], properties, GetSymbolName(symbol), arg1.ToString(), arg2.ToString(), arg3.ToString());

        private static void ReportDiagnostics<T>(SymbolAnalysisContext context, Func<T, Compilation, IEnumerable<Diagnostic>> analyzer) where T : ISymbol
        {
            var symbol = context.Symbol;
            var compilation = context.Compilation;

            ReportDiagnostics(context, analyzer((T)symbol, compilation));
        }

        private static bool ReferencesTestAssemblies(Compilation compilation)
        {
            if (compilation.GetTypeByMetadataName("NUnit.Framework.TestAttribute") != null)
            {
                return true;
            }

            if (compilation.GetTypeByMetadataName("NUnit.Framework.TestCaseAttribute") != null)
            {
                return true;
            }

            if (compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod") != null)
            {
                return true;
            }

            return false;
        }

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

        private Diagnostic CreateIssue(Location location, Dictionary<string, string> properties, params object[] args)
        {
            var immutableProperties = properties is null
                        ? ImmutableDictionary<string, string>.Empty
                        : ImmutableDictionary.CreateRange(properties);

            return Diagnostic.Create(Rule, location, immutableProperties, args);
        }

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