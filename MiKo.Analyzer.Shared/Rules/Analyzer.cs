using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules
{
    public abstract class Analyzer : DiagnosticAnalyzer
    {
        private static readonly ConcurrentDictionary<string, DiagnosticDescriptor> KnownRules = new ConcurrentDictionary<string, DiagnosticDescriptor>();

        private readonly DiagnosticDescriptor m_rule;

        private Func<INamespaceSymbol, Compilation, IEnumerable<Diagnostic>> m_analyzeNamespaceCallback; // cached to prevent creation of multiple delegates
        private Func<INamedTypeSymbol, Compilation, IEnumerable<Diagnostic>> m_analyzeTypeCallback; // cached to prevent creation of multiple delegates
        private Func<IEventSymbol, Compilation, IEnumerable<Diagnostic>> m_analyzeEventCallback; // cached to prevent creation of multiple delegates
        private Func<IFieldSymbol, Compilation, IEnumerable<Diagnostic>> m_analyzeFieldCallback; // cached to prevent creation of multiple delegates
        private Func<IMethodSymbol, Compilation, IEnumerable<Diagnostic>> m_analyzeMethodCallback; // cached to prevent creation of multiple delegates
        private Func<IPropertySymbol, Compilation, IEnumerable<Diagnostic>> m_analyzePropertyCallback; // cached to prevent creation of multiple delegates
        private Func<IParameterSymbol, Compilation, IEnumerable<Diagnostic>> m_analyzeParameterCallback; // cached to prevent creation of multiple delegates

        private Action<SymbolAnalysisContext> m_analyzeNamespaceContextCallback; // cached to prevent creation of multiple delegates
        private Action<SymbolAnalysisContext> m_analyzeTypeContextCallback; // cached to prevent creation of multiple delegates
        private Action<SymbolAnalysisContext> m_analyzeEventContextCallback; // cached to prevent creation of multiple delegates
        private Action<SymbolAnalysisContext> m_analyzeFieldContextCallback; // cached to prevent creation of multiple delegates
        private Action<SymbolAnalysisContext> m_analyzeMethodContextCallback; // cached to prevent creation of multiple delegates
        private Action<SymbolAnalysisContext> m_analyzePropertyContextCallback; // cached to prevent creation of multiple delegates
        private Action<SymbolAnalysisContext> m_analyzeParameterContextCallback; // cached to prevent creation of multiple delegates

        protected Analyzer(string category, string diagnosticId)
        {
            DiagnosticId = diagnosticId;

            m_rule = KnownRules.GetOrAdd(
                                     diagnosticId,
                                     id => new DiagnosticDescriptor(
                                                                id,
                                                                LocalizableResource(id, "Title"),
                                                                LocalizableResource(id, "MessageFormat"),
                                                                category,
                                                                Severity,
                                                                IsEnabledByDefault,
                                                                LocalizableResource(id, "Description"),
                                                                LocalizableResource(id, "HelpLinkUri")?.ToString()));
        }

        protected Analyzer(string category, string diagnosticId, in SymbolKind symbolKind) : this(category, diagnosticId) => SymbolKind = symbolKind;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(m_rule);

        public string DiagnosticId { get; }

        protected SymbolKind SymbolKind { get; } = SymbolKind.Alias;

        protected virtual DiagnosticSeverity Severity => DiagnosticSeverity.Warning;

        protected virtual bool IsEnabledByDefault => true;

        protected virtual bool CanRunConcurrently => true;

        protected virtual bool IsUnitTestAnalyzer => false;

        protected virtual bool SupportsNUnit => true;

        protected virtual bool SupportsXUnit => true;

        protected virtual bool SupportsMSTest => true;

        public static void Reset() => KnownRules.Clear();

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public sealed override void Initialize(AnalysisContext context)
        {
            m_analyzeNamespaceCallback = m_analyzeNamespaceCallback ?? AnalyzeNamespace;
            m_analyzeNamespaceContextCallback = m_analyzeNamespaceContextCallback ?? AnalyzeNamespace;
            m_analyzeTypeCallback = m_analyzeTypeCallback ?? AnalyzeType;
            m_analyzeTypeContextCallback = m_analyzeTypeContextCallback ?? AnalyzeType;
            m_analyzeEventCallback = m_analyzeEventCallback ?? AnalyzeEvent;
            m_analyzeEventContextCallback = m_analyzeEventContextCallback ?? AnalyzeEvent;
            m_analyzeFieldCallback = m_analyzeFieldCallback ?? AnalyzeField;
            m_analyzeFieldContextCallback = m_analyzeFieldContextCallback ?? AnalyzeField;
            m_analyzeMethodCallback = m_analyzeMethodCallback ?? AnalyzeMethod;
            m_analyzeMethodContextCallback = m_analyzeMethodContextCallback ?? AnalyzeMethod;
            m_analyzePropertyCallback = m_analyzePropertyCallback ?? AnalyzeProperty;
            m_analyzePropertyContextCallback = m_analyzePropertyContextCallback ?? AnalyzeProperty;
            m_analyzeParameterCallback = m_analyzeParameterCallback ?? AnalyzeParameter;
            m_analyzeParameterContextCallback = m_analyzeParameterContextCallback ?? AnalyzeParameter;

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            if (CanRunConcurrently)
            {
                context.EnableConcurrentExecution();
            }

            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        protected static Location CreateLocation(SyntaxNode node, in int start, in int end) => CreateLocation(node.SyntaxTree, start, end);

        protected static Location CreateLocation(in SyntaxToken token, in int start, in int end) => CreateLocation(token.SyntaxTree, start, end);

        protected static Location CreateLocation(SyntaxTree syntaxTree, in int start, in int end) => Location.Create(syntaxTree, TextSpan.FromBounds(start, end));

        protected static Location CreateLocation(in char value, SyntaxTree syntaxTree, in int spanStart, in int position, in int startOffset = 0, in int endOffset = 0)
        {
            if (position == -1)
            {
                return null;
            }

            var start = spanStart + position + startOffset; // find start position for underlining
            var end = start + sizeof(char) - startOffset - endOffset; // find end position

            return CreateLocation(syntaxTree, start, end);
        }

        protected static Location CreateLocation(string value, SyntaxTree syntaxTree, in int spanStart, in int position, in int startOffset = 0, in int endOffset = 0)
        {
            if (position == -1)
            {
                return null;
            }

            var start = spanStart + position + startOffset; // find start position for underlining
            var end = start + value.Length - startOffset - endOffset; // find end position

            return CreateLocation(syntaxTree, start, end);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void ReportDiagnostics(in SyntaxNodeAnalysisContext context, Diagnostic issue)
        {
            if (issue != null)
            {
                context.ReportDiagnostic(issue);
            }
        }

        protected static void ReportDiagnostics(in SyntaxNodeAnalysisContext context, IEnumerable<Diagnostic> issues)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                // seems that we should cancel and not report further issues
                return;
            }

            if (issues is Diagnostic[] array)
            {
                ReportDiagnostics(context, array);
            }
            else if (issues is IReadOnlyList<Diagnostic> list)
            {
                ReportDiagnostics(context, list);
            }
            else
            {
                ReportDiagnosticsEnumerable(context, issues);
            }
        }

        protected static void ReportDiagnostics(in SyntaxNodeAnalysisContext context, Diagnostic[] issues)
        {
            var issuesLength = issues.Length;

            switch (issuesLength)
            {
                case 0: return;
                case 1:
                {
                    ReportDiagnostics(context, issues[0]);

                    return;
                }

                default:
                {
                    for (var index = 0; index < issuesLength; index++)
                    {
                        var issue = issues[index];

                        if (issue != null)
                        {
                            context.ReportDiagnostic(issue);
                        }
                    }

                    return;
                }
            }
        }

        protected static void ReportDiagnostics(in SyntaxNodeAnalysisContext context, IReadOnlyList<Diagnostic> issues)
        {
            var issuesCount = issues.Count;

            switch (issuesCount)
            {
                case 0: return;
                case 1:
                {
                    ReportDiagnostics(context, issues[0]);

                    return;
                }

                default:
                {
                    for (var index = 0; index < issuesCount; index++)
                    {
                        var issue = issues[index];

                        if (issue != null)
                        {
                            context.ReportDiagnostic(issue);
                        }
                    }

                    return;
                }
            }
        }

        protected virtual bool IsApplicable(CompilationStartAnalysisContext context) => true;

        protected virtual void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind);

        protected void InitializeCore(CompilationStartAnalysisContext context, in SymbolKind symbolKind)
        {
            var action = GetAnalyzeMethod(symbolKind);

            if (action != null)
            {
                context.RegisterSymbolAction(action, symbolKind);
            }
        }

        protected void InitializeCore(CompilationStartAnalysisContext context, params SymbolKind[] symbolKinds)
        {
            var length = symbolKinds.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    InitializeCore(context, symbolKinds[index]);
                }
            }
        }

        protected void AnalyzeNamespace(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzeNamespaceCallback);

        protected virtual IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected void AnalyzeType(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzeTypeCallback);

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected void AnalyzeEvent(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzeEventCallback);

        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected void AnalyzeField(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzeFieldCallback);

        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected void AnalyzeMethod(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzeMethodCallback);

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected void AnalyzeProperty(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzePropertyCallback);

        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected void AnalyzeParameter(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzeParameterCallback);

        protected virtual IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected Diagnostic Issue(CastExpressionSyntax cast)
        {
            // underline only the cast itself, not the complete expression
            var location = CreateLocation(cast, cast.OpenParenToken.SpanStart, cast.CloseParenToken.Span.End);

            return Issue(location);
        }

        protected Diagnostic Issue(ISymbol symbol) => Issue(symbol, Array.Empty<Pair>());

        protected Diagnostic Issue(SyntaxNode node) => Issue(node, Array.Empty<Pair>());

        protected Diagnostic Issue(in SyntaxToken token) => Issue(token, Array.Empty<Pair>());

        protected Diagnostic Issue(in SyntaxTrivia trivia) => Issue(trivia, Array.Empty<Pair>());

        protected Diagnostic Issue(Location location) => Issue(location, Array.Empty<Pair>());

        protected Diagnostic Issue<T>(ISymbol symbol, T arg1) => Issue(symbol, arg1, Array.Empty<Pair>());

        protected Diagnostic Issue<T>(SyntaxNode node, T arg1) => Issue(node, arg1, Array.Empty<Pair>());

        protected Diagnostic Issue<T>(in SyntaxToken token, T arg1) => Issue(token, arg1, Array.Empty<Pair>());

        protected Diagnostic Issue<T>(in SyntaxTrivia trivia, T arg1) => Issue(trivia, arg1, Array.Empty<Pair>());

        protected Diagnostic Issue<T>(Location location, T arg1) => Issue(location, arg1, Array.Empty<Pair>());

        protected Diagnostic Issue(string name, ISymbol symbol) => Issue(name, symbol, Array.Empty<Pair>());

        protected Diagnostic Issue(string name, SyntaxNode node) => Issue(name, node, Array.Empty<Pair>());

        protected Diagnostic Issue(string name, in SyntaxToken token) => Issue(name, token, Array.Empty<Pair>());

        protected Diagnostic Issue(string name, in SyntaxTrivia trivia) => Issue(name, trivia, Array.Empty<Pair>());

        protected Diagnostic Issue(string name, Location location) => Issue(name, location, Array.Empty<Pair>());

        protected Diagnostic Issue<T>(string name, SyntaxNode node, T arg1) => Issue(name, node, arg1, Array.Empty<Pair>());

        protected Diagnostic Issue<T>(string name, in SyntaxToken token, T arg1) => Issue(name, token, arg1, Array.Empty<Pair>());

        protected Diagnostic Issue<T>(string name, Location location, T arg1) => Issue(name, location, arg1, Array.Empty<Pair>());

        protected Diagnostic Issue<T1, T2>(ISymbol symbol, T1 arg1, T2 arg2) => Issue(symbol, arg1, arg2, Array.Empty<Pair>());

        protected Diagnostic Issue<T1, T2>(SyntaxNode node, T1 arg1, T2 arg2) => Issue(node.GetLocation(), arg1, arg2, Array.Empty<Pair>());

        protected Diagnostic Issue<T1, T2>(Location location, T1 arg1, T2 arg2) => Issue(location, arg1, arg2, Array.Empty<Pair>());

        protected Diagnostic Issue<T1, T2>(string name, ISymbol symbol, T1 arg1, T2 arg2) => Issue(name, symbol, arg1, arg2, Array.Empty<Pair>());

        protected Diagnostic Issue<T1, T2>(string name, SyntaxNode node, T1 arg1, T2 arg2) => Issue(name, node, arg1, arg2, Array.Empty<Pair>());

        protected Diagnostic Issue<T1, T2>(string name, in SyntaxToken token, T1 arg1, T2 arg2) => Issue(name, token, arg1, arg2, Array.Empty<Pair>());

        protected Diagnostic Issue<T1, T2>(string name, Location location, T1 arg1, T2 arg2) => Issue(name, location, arg1, arg2, Array.Empty<Pair>());

        protected Diagnostic Issue<T1, T2, T3>(ISymbol symbol, T1 arg1, T2 arg2, T3 arg3) => Issue(symbol, arg1, arg2, arg3, Array.Empty<Pair>());

        protected Diagnostic Issue<T1, T2, T3>(string name, SyntaxNode node, T1 arg1, T2 arg2, T3 arg3) => Issue(name, node, arg1, arg2, arg3, Array.Empty<Pair>());

        protected Diagnostic Issue<T1, T2, T3>(string name, in SyntaxToken token, T1 arg1, T2 arg2, T3 arg3) => Issue(name, token, arg1, arg2, arg3, Array.Empty<Pair>());

        protected Diagnostic Issue<T1, T2, T3>(string name, Location location, T1 arg1, T2 arg2, T3 arg3) => Issue(name, location, arg1, arg2, arg3, Array.Empty<Pair>());

        protected Diagnostic Issue(ISymbol symbol, params Pair[] properties) => CreateIssue(symbol.Locations[0], properties, GetSymbolName(symbol));

        protected Diagnostic Issue(SyntaxNode node, params Pair[] properties) => CreateIssue(node.GetLocation(), properties, node.ToString());

        protected Diagnostic Issue(in SyntaxToken token, params Pair[] properties) => CreateIssue(token.GetLocation(), properties, token.ValueText);

        protected Diagnostic Issue(in SyntaxTrivia trivia, params Pair[] properties) => CreateIssue(trivia.GetLocation(), properties);

        protected Diagnostic Issue(Location location, params Pair[] properties) => CreateIssue(location, properties, location.GetText());

        protected Diagnostic Issue<T>(ISymbol symbol, T arg1, params Pair[] properties) => CreateIssue(symbol.Locations[0], properties, GetSymbolName(symbol), arg1.ToString());

        protected Diagnostic Issue<T>(SyntaxNode node, T arg1, params Pair[] properties) => CreateIssue(node.GetLocation(), properties, arg1.ToString());

        protected Diagnostic Issue<T>(in SyntaxToken token, T arg1, params Pair[] properties) => CreateIssue(token.GetLocation(), properties, arg1.ToString());

        protected Diagnostic Issue<T>(in SyntaxTrivia trivia, T arg1, params Pair[] properties) => CreateIssue(trivia.GetLocation(), properties, arg1.ToString());

        protected Diagnostic Issue<T>(Location location, T arg1, params Pair[] properties) => CreateIssue(location, properties, location.GetText(), arg1.ToString());

        protected Diagnostic Issue(string name, ISymbol symbol, params Pair[] properties) => CreateIssue(symbol.Locations[0], properties, name);

        protected Diagnostic Issue(string name, SyntaxNode node, params Pair[] properties) => CreateIssue(node.GetLocation(), properties, name);

        protected Diagnostic Issue(string name, in SyntaxToken token, params Pair[] properties) => CreateIssue(token.GetLocation(), properties, name);

        protected Diagnostic Issue(string name, in SyntaxTrivia trivia, params Pair[] properties) => CreateIssue(trivia.GetLocation(), properties, name);

        protected Diagnostic Issue(string name, Location location, params Pair[] properties) => CreateIssue(location, properties, name);

        protected Diagnostic Issue<T>(string name, SyntaxNode node, T arg1, params Pair[] properties) => CreateIssue(node.GetLocation(), properties, name, arg1.ToString());

        protected Diagnostic Issue<T>(string name, in SyntaxToken token, T arg1, params Pair[] properties) => CreateIssue(token.GetLocation(), properties, name, arg1.ToString());

        protected Diagnostic Issue<T>(string name, Location location, T arg1, params Pair[] properties) => CreateIssue(location, properties, name, arg1.ToString());

        protected Diagnostic Issue<T1, T2>(ISymbol symbol, T1 arg1, T2 arg2, params Pair[] properties) => CreateIssue(symbol.Locations[0], properties, GetSymbolName(symbol), arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2>(Location location, T1 arg1, T2 arg2, params Pair[] properties) => CreateIssue(location, properties, location.GetText(), arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2>(string name, ISymbol symbol, T1 arg1, T2 arg2, params Pair[] properties) => CreateIssue(symbol.Locations[0], properties, name, arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2>(string name, SyntaxNode node, T1 arg1, T2 arg2, params Pair[] properties) => CreateIssue(node.GetLocation(), properties, name, arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2>(string name, in SyntaxToken token, T1 arg1, T2 arg2, params Pair[] properties) => CreateIssue(token.GetLocation(), properties, name, arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2>(string name, Location location, T1 arg1, T2 arg2, params Pair[] properties) => CreateIssue(location, properties, name, arg1.ToString(), arg2.ToString());

        protected Diagnostic Issue<T1, T2, T3>(ISymbol symbol, T1 arg1, T2 arg2, T3 arg3, params Pair[] properties) => CreateIssue(symbol.Locations[0], properties, GetSymbolName(symbol), arg1.ToString(), arg2.ToString(), arg3.ToString());

        protected Diagnostic Issue<T1, T2, T3>(string name, SyntaxNode node, T1 arg1, T2 arg2, T3 arg3, params Pair[] properties) => CreateIssue(node.GetLocation(), properties, name, arg1.ToString(), arg2.ToString(), arg3.ToString());

        protected Diagnostic Issue<T1, T2, T3>(string name, in SyntaxToken token, T1 arg1, T2 arg2, T3 arg3, params Pair[] properties) => CreateIssue(token.GetLocation(), properties, name, arg1.ToString(), arg2.ToString(), arg3.ToString());

        protected Diagnostic Issue<T1, T2, T3>(string name, Location location, T1 arg1, T2 arg2, T3 arg3, params Pair[] properties) => CreateIssue(location, properties, name, arg1.ToString(), arg2.ToString(), arg3.ToString());

        private static void ReportDiagnostics<T>(in SymbolAnalysisContext context, Func<T, Compilation, IEnumerable<Diagnostic>> analyzer) where T : ISymbol
        {
            var symbol = context.Symbol;
            var compilation = context.Compilation;

            var issues = analyzer((T)symbol, compilation);

            if (context.CancellationToken.IsCancellationRequested)
            {
                // seems that we should cancel and not report further issues
                return;
            }

            if (issues is Diagnostic[] array)
            {
                ReportDiagnostics(context, array);
            }
            else if (issues is IReadOnlyList<Diagnostic> list)
            {
                ReportDiagnostics(context, list);
            }
            else
            {
                ReportDiagnosticsEnumerable(context, issues);
            }
        }

        private static void ReportDiagnostics(in SymbolAnalysisContext context, Diagnostic[] array)
        {
            var arrayLength = array.Length;

            if (arrayLength > 0)
            {
                for (var index = 0; index < arrayLength; index++)
                {
                    var issue = array[index];

                    if (issue != null)
                    {
                        context.ReportDiagnostic(issue);
                    }
                }
            }
        }

        private static void ReportDiagnostics(in SymbolAnalysisContext context, IReadOnlyList<Diagnostic> list)
        {
            var listCount = list.Count;

            if (listCount > 0)
            {
                for (var index = 0; index < listCount; index++)
                {
                    var issue = list[index];

                    if (issue != null)
                    {
                        context.ReportDiagnostic(issue);
                    }
                }
            }
        }

        private static void ReportDiagnosticsEnumerable(in SymbolAnalysisContext context, IEnumerable<Diagnostic> issues)
        {
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

        private static void ReportDiagnosticsEnumerable(in SyntaxNodeAnalysisContext context, IEnumerable<Diagnostic> issues)
        {
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

        private static string GetSymbolName(ISymbol symbol)
        {
            if (symbol is IMethodSymbol m)
            {
                switch (m.MethodKind)
                {
                    case MethodKind.Constructor:
                    case MethodKind.StaticConstructor:
                    {
                        return symbol.ContainingSymbol.Name + symbol.Name;
                    }
                }
            }

            return symbol.Name;
        }

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static LocalizableResourceString LocalizableResource(string id, string suffix) => new LocalizableResourceString(id + "_" + suffix, Resources.ResourceManager, typeof(Resources));

        private Diagnostic CreateIssue(Location location, Pair[] properties, params object[] args)
        {
            var immutableProperties = properties.Length == 0
                                      ? ImmutableDictionary<string, string>.Empty
                                      : ImmutableDictionary.CreateRange(properties.Select(_ => new KeyValuePair<string, string>(_.Key, _.Value)));

            return Diagnostic.Create(m_rule, location, immutableProperties, args);
        }

        private Action<SymbolAnalysisContext> GetAnalyzeMethod(in SymbolKind kind)
        {
            switch (kind)
            {
                case SymbolKind.Method: return m_analyzeMethodContextCallback;
                case SymbolKind.NamedType: return m_analyzeTypeContextCallback;
                case SymbolKind.Property: return m_analyzePropertyContextCallback;
                case SymbolKind.Event: return m_analyzeEventContextCallback;
                case SymbolKind.Field: return m_analyzeFieldContextCallback;
                case SymbolKind.Namespace: return m_analyzeNamespaceContextCallback;
                case SymbolKind.Parameter: return m_analyzeParameterContextCallback;

                default: return null;
            }
        }

        private void CompilationStartAction(CompilationStartAnalysisContext context)
        {
            if (IsUnitTestAnalyzer)
            {
                if (ReferencesTestAssemblies(context.Compilation) is false)
                {
                    // do not run analyzer if there are no tests contained
                    return;
                }
            }

            if (IsApplicable(context))
            {
                InitializeCore(context);
            }
        }

        private bool ReferencesTestAssemblies(Compilation compilation)
        {
            if (compilation.GetTypeByMetadataName("NUnit.Framework.TestAttribute") != null)
            {
                return SupportsNUnit;
            }

            if (compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute") != null)
            {
                return SupportsMSTest;
            }

            if (compilation.GetTypeByMetadataName("Xunit.FactAttribute") != null)
            {
                return SupportsXUnit;
            }

            return false;
        }
    }
}