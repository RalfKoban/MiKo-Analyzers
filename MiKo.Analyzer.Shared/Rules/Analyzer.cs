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
    /// <summary>
    /// Provides a base class for analyzers that enforce the different rules.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Analyzer"/> class.
        /// </summary>
        /// <param name="category">
        /// The category of the diagnostic.
        /// </param>
        /// <param name="diagnosticId">
        /// The unique identifier of the diagnostic.
        /// </param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Analyzer"/> class with the category and unique identifier of the diagnostic, as well as the kind of symbol to analyze.
        /// </summary>
        /// <param name="category">
        /// The category of the diagnostic.
        /// </param>
        /// <param name="diagnosticId">
        /// The unique identifier of the diagnostic.
        /// </param>
        /// <param name="symbolKind">
        /// One of the enumeration members that specifies the kind of symbol to analyze.
        /// </param>
        protected Analyzer(string category, string diagnosticId, in SymbolKind symbolKind) : this(category, diagnosticId) => SymbolKind = symbolKind;

        /// <summary>
        /// Gets the descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        /// <value>
        /// A collection of diagnostic descriptors that contains the diagnostic descriptor this analyzer is capable of producing.
        /// </value>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(m_rule);

        /// <summary>
        /// Gets the unique identifier of the diagnostic.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> that contains the unique identifier of the diagnostic.
        /// </value>
        public string DiagnosticId { get; }

        /// <summary>
        /// Gets the kind of symbol to analyze.
        /// </summary>
        /// <value>
        /// The enumerated constant that is the kind of symbol to analyze. The default is <see cref="SymbolKind.Alias"/>.
        /// </value>
        protected SymbolKind SymbolKind { get; } = SymbolKind.Alias;

        /// <summary>
        /// Gets the severity of the diagnostic.
        /// </summary>
        /// <value>
        /// The enumerated constant that is the severity of the diagnostic. The default is <see cref="DiagnosticSeverity.Warning"/>.
        /// </value>
        protected virtual DiagnosticSeverity Severity => DiagnosticSeverity.Warning;

        /// <summary>
        /// Gets a value indicating whether the analyzer is enabled by default.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the analyzer is enabled by default; otherwise, <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        protected virtual bool IsEnabledByDefault => true;

        /// <summary>
        /// Gets a value indicating whether the analyzer can run concurrently.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the analyzer can run concurrently; otherwise, <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        protected virtual bool CanRunConcurrently => true;

        /// <summary>
        /// Gets a value indicating whether the analyzer is specific to unit tests.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the analyzer is specific to unit tests; otherwise, <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        protected virtual bool IsUnitTestAnalyzer => false;

        /// <summary>
        /// Gets a value indicating whether the analyzer supports NUnit.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the analyzer supports NUnit; otherwise, <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        protected virtual bool SupportsNUnit => true;

        /// <summary>
        /// Gets a value indicating whether the analyzer supports xUnit.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the analyzer supports xUnit; otherwise, <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        protected virtual bool SupportsXUnit => true;

        /// <summary>
        /// Gets a value indicating whether the analyzer supports MSTest.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the analyzer supports MSTest; otherwise, <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        protected virtual bool SupportsMSTest => true;

        /// <summary>
        /// Clears all known diagnostic rules.
        /// </summary>
        public static void Reset() => KnownRules.Clear();

        /// <summary>
        /// Initializes the analyzer by registering appropriate analysis actions.
        /// </summary>
        /// <param name="context">
        /// The context for initializing the analyzer.
        /// </param>
        public sealed override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
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

        /// <summary>
        /// Creates a location from a syntax trivia list.
        /// </summary>
        /// <param name="trivia">
        /// The syntax trivia list.
        /// </param>
        /// <returns>
        /// A location that spans the entire trivia list, or <see cref="Location.None"/> if the list is empty.
        /// </returns>
        protected static Location CreateLocation(in SyntaxTriviaList trivia)
        {
            if (trivia.Count > 0)
            {
                var span = trivia.FullSpan;

                return CreateLocation(trivia[0].SyntaxTree, span.Start, span.End);
            }

            return Location.None;
        }

        /// <summary>
        /// Creates a location from a syntax node with specified bounds.
        /// </summary>
        /// <param name="node">
        /// The syntax node.
        /// </param>
        /// <param name="start">
        /// The start position of the location.
        /// </param>
        /// <param name="end">
        /// The end position of the location.
        /// </param>
        /// <returns>
        /// A location that spans from the start to the end position.
        /// </returns>
        protected static Location CreateLocation(SyntaxNode node, in int start, in int end) => CreateLocation(node.SyntaxTree, start, end);

        /// <summary>
        /// Creates a location from a syntax token with specified bounds.
        /// </summary>
        /// <param name="token">
        /// The syntax token.
        /// </param>
        /// <param name="start">
        /// The start position of the location.
        /// </param>
        /// <param name="end">
        /// The end position of the location.
        /// </param>
        /// <returns>
        /// A location that spans from the start to the end position.
        /// </returns>
        protected static Location CreateLocation(in SyntaxToken token, in int start, in int end) => CreateLocation(token.SyntaxTree, start, end);

        /// <summary>
        /// Creates a location from a syntax tree with specified bounds.
        /// </summary>
        /// <param name="syntaxTree">
        /// The syntax tree.
        /// </param>
        /// <param name="start">
        /// The start position of the location.
        /// </param>
        /// <param name="end">
        /// The end position of the location.
        /// </param>
        /// <returns>
        /// A location that spans from the start to the end position.
        /// </returns>
        protected static Location CreateLocation(SyntaxTree syntaxTree, in int start, in int end) => Location.Create(syntaxTree, TextSpan.FromBounds(start, end));

        /// <summary>
        /// Creates a location for a character within a syntax tree.
        /// </summary>
        /// <param name="value">
        /// The character value.
        /// </param>
        /// <param name="syntaxTree">
        /// The syntax tree.
        /// </param>
        /// <param name="spanStart">
        /// The start position of the containing span.
        /// </param>
        /// <param name="position">
        /// The position of the character relative to the span start, or <c>-1</c> if not found.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A location for the character, or <see langword="null"/> if the position is <c>-1</c> or the calculated bounds are invalid.
        /// </returns>
        protected static Location CreateLocation(in char value, SyntaxTree syntaxTree, in int spanStart, in int position, in int startOffset = 0, in int endOffset = 0)
        {
            if (position is -1)
            {
                return null;
            }

            var start = spanStart + position + startOffset; // find start position for underlining
            var end = start + sizeof(char) - startOffset - endOffset; // find end position

            if (end < start)
            {
                // seems we did not find a proper location here
                return null;
            }

            return CreateLocation(syntaxTree, start, end);
        }

        /// <summary>
        /// Creates a location for a <see cref="string"/> within a syntax tree.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> value.
        /// </param>
        /// <param name="syntaxTree">
        /// The syntax tree.
        /// </param>
        /// <param name="spanStart">
        /// The start position of the containing span.
        /// </param>
        /// <param name="position">
        /// The position of the <see cref="string"/> relative to the span start, or <c>-1</c> if not found.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A location for the <see cref="string"/>, or <see langword="null"/> if the position is <c>-1</c> or the calculated bounds are invalid.
        /// </returns>
        protected static Location CreateLocation(string value, SyntaxTree syntaxTree, in int spanStart, in int position, in int startOffset = 0, in int endOffset = 0)
        {
            if (position is -1)
            {
                return null;
            }

            var start = spanStart + position + startOffset; // find start position for underlining
            var end = start + value.Length - startOffset - endOffset; // find end position

            if (end < start)
            {
                // seems we did not find a proper location here
                return null;
            }

            return CreateLocation(syntaxTree, start, end);
        }

        /// <summary>
        /// Reports a diagnostic issue to the analysis context.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        /// <param name="issue">
        /// The diagnostic issue to report.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void ReportDiagnostics(in SyntaxNodeAnalysisContext context, Diagnostic issue)
        {
            if (issue != null)
            {
                context.ReportDiagnostic(issue);
            }
        }

        /// <summary>
        /// Reports diagnostic issues to the analysis context.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        /// <param name="issues">
        /// The diagnostic issues to report.
        /// </param>
        protected static void ReportDiagnostics(in SyntaxNodeAnalysisContext context, IEnumerable<Diagnostic> issues)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                // seems that we should cancel and not report further issues
                return;
            }

            if (issues is Diagnostic[] array)
            {
                if (array.Length > 0)
                {
                    ReportDiagnostics(context, array);
                }
            }
            else
            {
                ReportDiagnosticsEnumerable(context, issues);
            }
        }

        /// <summary>
        /// Reports diagnostic issues to the analysis context.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        /// <param name="issues">
        /// The diagnostic issues to report.
        /// </param>
        protected static void ReportDiagnostics(in SyntaxNodeAnalysisContext context, Diagnostic[] issues)
        {
            for (int index = 0, length = issues.Length; index < length; index++)
            {
                var issue = issues[index];

                if (issue != null)
                {
                    context.ReportDiagnostic(issue);
                }
            }
        }

        /// <summary>
        /// Reports diagnostic issues to the analysis context.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        /// <param name="issues">
        /// The diagnostic issues to report.
        /// </param>
        protected static void ReportDiagnostics(in SyntaxNodeAnalysisContext context, IReadOnlyList<Diagnostic> issues)
        {
            for (int index = 0, count = issues.Count; index < count; index++)
            {
                ReportDiagnostics(context, issues[index]);
            }
        }

        /// <summary>
        /// Determines whether the analyzer is applicable to the specified compilation.
        /// </summary>
        /// <param name="compilation">
        /// The compilation to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the analyzer is applicable; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool IsApplicable(Compilation compilation) => true;

        /// <summary>
        /// Initializes the analyzer core functionality.
        /// </summary>
        /// <param name="context">
        /// The compilation start analysis context.
        /// </param>
        protected virtual void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind);

        /// <summary>
        /// Initializes the analyzer core functionality for a specific symbol kind.
        /// </summary>
        /// <param name="context">
        /// The compilation start analysis context.
        /// </param>
        /// <param name="symbolKind">
        /// One of the enumeration members that specifies the kind of symbol to analyze.
        /// </param>
        protected void InitializeCore(CompilationStartAnalysisContext context, in SymbolKind symbolKind)
        {
            var callback = GetAnalyzeMethod(symbolKind);

            if (callback != null)
            {
                context.RegisterSymbolAction(callback, symbolKind);
            }
        }

        /// <summary>
        /// Initializes the analyzer core functionality for multiple symbol kinds.
        /// </summary>
        /// <param name="context">
        /// The compilation start analysis context.
        /// </param>
        /// <param name="symbolKinds">
        /// The kinds of symbols to analyze.
        /// </param>
        protected void InitializeCore(CompilationStartAnalysisContext context, params SymbolKind[] symbolKinds)
        {
            for (int index = 0, length = symbolKinds.Length; index < length; index++)
            {
                InitializeCore(context, symbolKinds[index]);
            }
        }

        /// <summary>
        /// Analyzes a namespace symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="context">
        /// The symbol analysis context.
        /// </param>
        protected void AnalyzeNamespace(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzeNamespaceCallback);

        /// <summary>
        /// Analyzes a namespace symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="symbol">
        /// The namespace symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation context.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes a type symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="context">
        /// The symbol analysis context.
        /// </param>
        protected void AnalyzeType(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzeTypeCallback);

        /// <summary>
        /// Analyzes a type symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="symbol">
        /// The type symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation context.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes an event symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="context">
        /// The symbol analysis context.
        /// </param>
        protected void AnalyzeEvent(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzeEventCallback);

        /// <summary>
        /// Analyzes an event symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="symbol">
        /// The event symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation context.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes a field symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="context">
        /// The symbol analysis context.
        /// </param>
        protected void AnalyzeField(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzeFieldCallback);

        /// <summary>
        /// Analyzes a field symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="symbol">
        /// The field symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation context.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes a method symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="context">
        /// The symbol analysis context.
        /// </param>
        protected void AnalyzeMethod(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzeMethodCallback);

        /// <summary>
        /// Analyzes a method symbol.
        /// </summary>
        /// <param name="symbol">
        /// The method symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation context.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes a property symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="context">
        /// The symbol analysis context.
        /// </param>
        protected void AnalyzeProperty(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzePropertyCallback);

        /// <summary>
        /// Analyzes a property symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="symbol">
        /// The property symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation context.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes a parameter symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="context">
        /// The symbol analysis context.
        /// </param>
        protected void AnalyzeParameter(SymbolAnalysisContext context) => ReportDiagnostics(context, m_analyzeParameterCallback);

        /// <summary>
        /// Analyzes a parameter symbol and returns any diagnostics found.
        /// </summary>
        /// <param name="symbol">
        /// The parameter symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation context.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Creates a diagnostic issue for a cast expression.
        /// </summary>
        /// <param name="cast">
        /// The cast expression syntax to create the issue for.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the cast expression.
        /// </returns>
        protected Diagnostic Issue(CastExpressionSyntax cast)
        {
            // underline only the cast itself, not the complete expression
            var location = CreateLocation(cast, cast.OpenParenToken.SpanStart, cast.CloseParenToken.Span.End);

            return Issue(location);
        }

        /// <summary>
        /// Creates a diagnostic issue for a symbol.
        /// </summary>
        /// <param name="symbol">
        /// The symbol to create the issue for.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the symbol.
        /// </returns>
        protected Diagnostic Issue(ISymbol symbol) => Issue(symbol, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax node.
        /// </summary>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue(SyntaxNode node) => Issue(node, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax token.
        /// </summary>
        /// <param name="token">
        /// The syntax token to create the issue for.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax token.
        /// </returns>
        protected Diagnostic Issue(in SyntaxToken token) => Issue(token, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax trivia.
        /// </summary>
        /// <param name="trivia">
        /// The syntax trivia to create the issue for.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax trivia.
        /// </returns>
        protected Diagnostic Issue(in SyntaxTrivia trivia) => Issue(trivia, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a location.
        /// </summary>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue(Location location) => Issue(location, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a symbol with a single argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="symbol">
        /// The symbol to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the symbol.
        /// </returns>
        protected Diagnostic Issue<T>(ISymbol symbol, T arg1) => Issue(symbol, arg1, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax node with a single argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue<T>(SyntaxNode node, T arg1) => Issue(node, arg1, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax token with a single argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="token">
        /// The syntax token to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax token.
        /// </returns>
        protected Diagnostic Issue<T>(in SyntaxToken token, T arg1) => Issue(token, arg1, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax trivia with a single argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="trivia">
        /// The syntax trivia to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax trivia.
        /// </returns>
        protected Diagnostic Issue<T>(in SyntaxTrivia trivia, T arg1) => Issue(trivia, arg1, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a location with a single argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue<T>(Location location, T arg1) => Issue(location, arg1, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a symbol with a custom name.
        /// </summary>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="symbol">
        /// The symbol to create the issue for.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the symbol.
        /// </returns>
        protected Diagnostic Issue(string name, ISymbol symbol) => Issue(name, symbol, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax node with a custom name.
        /// </summary>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue(string name, SyntaxNode node) => Issue(name, node, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax token with a custom name.
        /// </summary>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="token">
        /// The syntax token to create the issue for.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax token.
        /// </returns>
        protected Diagnostic Issue(string name, in SyntaxToken token) => Issue(name, token, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax trivia with a custom name.
        /// </summary>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="trivia">
        /// The syntax trivia to create the issue for.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax trivia.
        /// </returns>
        protected Diagnostic Issue(string name, in SyntaxTrivia trivia) => Issue(name, trivia, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a location with a custom name.
        /// </summary>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue(string name, Location location) => Issue(name, location, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax node with a custom name and a single argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue<T>(string name, SyntaxNode node, T arg1) => Issue(name, node, arg1, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax token with a custom name and a single argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="token">
        /// The syntax token to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax token.
        /// </returns>
        protected Diagnostic Issue<T>(string name, in SyntaxToken token, T arg1) => Issue(name, token, arg1, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a location with a custom name and a single argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue<T>(string name, Location location, T arg1) => Issue(name, location, arg1, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a symbol with two arguments.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="symbol">
        /// The symbol to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the symbol.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(ISymbol symbol, T1 arg1, T2 arg2) => Issue(symbol, arg1, arg2, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax node with two arguments.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(SyntaxNode node, T1 arg1, T2 arg2) => Issue(node.GetLocation(), arg1, arg2, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a location with two arguments.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(Location location, T1 arg1, T2 arg2) => Issue(location, arg1, arg2, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a symbol with a custom name and two arguments.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="symbol">
        /// The symbol to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the symbol.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(string name, ISymbol symbol, T1 arg1, T2 arg2) => Issue(name, symbol, arg1, arg2, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax node with a custom name and two arguments.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(string name, SyntaxNode node, T1 arg1, T2 arg2) => Issue(name, node, arg1, arg2, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax token with a custom name and two arguments.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="token">
        /// The syntax token to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax token.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(string name, in SyntaxToken token, T1 arg1, T2 arg2) => Issue(name, token, arg1, arg2, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a location with a custom name and two arguments.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(string name, Location location, T1 arg1, T2 arg2) => Issue(name, location, arg1, arg2, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a symbol with three arguments.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the third argument.
        /// </typeparam>
        /// <param name="symbol">
        /// The symbol to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="arg3">
        /// The third argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the symbol.
        /// </returns>
        protected Diagnostic Issue<T1, T2, T3>(ISymbol symbol, T1 arg1, T2 arg2, T3 arg3) => Issue(symbol, arg1, arg2, arg3, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax node with a custom name and three arguments.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the third argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="arg3">
        /// The third argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue<T1, T2, T3>(string name, SyntaxNode node, T1 arg1, T2 arg2, T3 arg3) => Issue(name, node, arg1, arg2, arg3, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a syntax token with a custom name and three arguments.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the third argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="token">
        /// The syntax token to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="arg3">
        /// The third argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax token.
        /// </returns>
        protected Diagnostic Issue<T1, T2, T3>(string name, in SyntaxToken token, T1 arg1, T2 arg2, T3 arg3) => Issue(name, token, arg1, arg2, arg3, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a location with a custom name and three arguments.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the third argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="arg3">
        /// The third argument for the diagnostic message.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue<T1, T2, T3>(string name, Location location, T1 arg1, T2 arg2, T3 arg3) => Issue(name, location, arg1, arg2, arg3, Array.Empty<Pair>());

        /// <summary>
        /// Creates a diagnostic issue for a symbol with additional properties.
        /// </summary>
        /// <param name="symbol">
        /// The symbol to create the issue for.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the symbol.
        /// </returns>
        protected Diagnostic Issue(ISymbol symbol, params Pair[] properties) => CreateIssue(symbol.Locations[0], properties, GetSymbolName(symbol));

        /// <summary>
        /// Creates a diagnostic issue for a syntax node with additional properties.
        /// </summary>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue(SyntaxNode node, params Pair[] properties) => CreateIssue(node.GetLocation(), properties, node.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a syntax token with additional properties.
        /// </summary>
        /// <param name="token">
        /// The syntax token to create the issue for.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax token.
        /// </returns>
        protected Diagnostic Issue(in SyntaxToken token, params Pair[] properties) => CreateIssue(token.GetLocation(), properties, token.ValueText);

        /// <summary>
        /// Creates a diagnostic issue for a syntax trivia with additional properties.
        /// </summary>
        /// <param name="trivia">
        /// The syntax trivia to create the issue for.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax trivia.
        /// </returns>
        protected Diagnostic Issue(in SyntaxTrivia trivia, params Pair[] properties) => CreateIssue(trivia.GetLocation(), properties);

        /// <summary>
        /// Creates a diagnostic issue for a syntax trivia list with additional properties.
        /// </summary>
        /// <param name="trivia">
        /// The syntax trivia list to create the issue for.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax trivia list.
        /// </returns>
        protected Diagnostic Issue(in SyntaxTriviaList trivia, params Pair[] properties) => CreateIssue(CreateLocation(trivia), properties);

        /// <summary>
        /// Creates a diagnostic issue for a location with additional properties.
        /// </summary>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue(Location location, params Pair[] properties) => CreateIssue(location, properties, location.GetText());

        /// <summary>
        /// Creates a diagnostic issue for a symbol with a single argument and additional properties.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="symbol">
        /// The symbol to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the symbol.
        /// </returns>
        protected Diagnostic Issue<T>(ISymbol symbol, T arg1, params Pair[] properties) => CreateIssue(symbol.Locations[0], properties, GetSymbolName(symbol), arg1.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a syntax node with a single argument and additional properties.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue<T>(SyntaxNode node, T arg1, params Pair[] properties) => CreateIssue(node.GetLocation(), properties, arg1.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a syntax token with a single argument and additional properties.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="token">
        /// The syntax token to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax token.
        /// </returns>
        protected Diagnostic Issue<T>(in SyntaxToken token, T arg1, params Pair[] properties) => CreateIssue(token.GetLocation(), properties, arg1.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a syntax trivia with a single argument and additional properties.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="trivia">
        /// The syntax trivia to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax trivia.
        /// </returns>
        protected Diagnostic Issue<T>(in SyntaxTrivia trivia, T arg1, params Pair[] properties) => CreateIssue(trivia.GetLocation(), properties, arg1.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a location with a single argument and additional properties.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue<T>(Location location, T arg1, params Pair[] properties) => CreateIssue(location, properties, location.GetText(), arg1.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a symbol with a custom name and additional properties.
        /// </summary>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="symbol">
        /// The symbol to create the issue for.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the symbol.
        /// </returns>
        protected Diagnostic Issue(string name, ISymbol symbol, params Pair[] properties) => CreateIssue(symbol.Locations[0], properties, name);

        /// <summary>
        /// Creates a diagnostic issue for a syntax node with a custom name and additional properties.
        /// </summary>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue(string name, SyntaxNode node, params Pair[] properties) => CreateIssue(node.GetLocation(), properties, name);

        /// <summary>
        /// Creates a diagnostic issue for a syntax token with a custom name and additional properties.
        /// </summary>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="token">
        /// The syntax token to create the issue for.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax token.
        /// </returns>
        protected Diagnostic Issue(string name, in SyntaxToken token, params Pair[] properties) => CreateIssue(token.GetLocation(), properties, name);

        /// <summary>
        /// Creates a diagnostic issue for a syntax trivia with a custom name and additional properties.
        /// </summary>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="trivia">
        /// The syntax trivia to create the issue for.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax trivia.
        /// </returns>
        protected Diagnostic Issue(string name, in SyntaxTrivia trivia, params Pair[] properties) => CreateIssue(trivia.GetLocation(), properties, name);

        /// <summary>
        /// Creates a diagnostic issue for a location with a custom name and additional properties.
        /// </summary>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue(string name, Location location, params Pair[] properties) => CreateIssue(location, properties, name);

        /// <summary>
        /// Creates a diagnostic issue for a syntax node with a custom name, a single argument and additional properties.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue<T>(string name, SyntaxNode node, T arg1, params Pair[] properties) => CreateIssue(node.GetLocation(), properties, name, arg1.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a syntax token with a custom name, a single argument and additional properties.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="token">
        /// The syntax token to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax token.
        /// </returns>
        protected Diagnostic Issue<T>(string name, in SyntaxToken token, T arg1, params Pair[] properties) => CreateIssue(token.GetLocation(), properties, name, arg1.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a location with a custom name, a single argument and additional properties.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue<T>(string name, Location location, T arg1, params Pair[] properties) => CreateIssue(location, properties, name, arg1.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a symbol with two arguments and additional properties.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="symbol">
        /// The symbol to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the symbol.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(ISymbol symbol, T1 arg1, T2 arg2, params Pair[] properties) => CreateIssue(symbol.Locations[0], properties, GetSymbolName(symbol), arg1.ToString(), arg2.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a location with two arguments and additional properties.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(Location location, T1 arg1, T2 arg2, params Pair[] properties) => CreateIssue(location, properties, location.GetText(), arg1.ToString(), arg2.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a symbol with a custom name, two arguments and additional properties.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="symbol">
        /// The symbol to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the symbol.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(string name, ISymbol symbol, T1 arg1, T2 arg2, params Pair[] properties) => CreateIssue(symbol.Locations[0], properties, name, arg1.ToString(), arg2.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a syntax node with a custom name, two arguments and additional properties.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(string name, SyntaxNode node, T1 arg1, T2 arg2, params Pair[] properties) => CreateIssue(node.GetLocation(), properties, name, arg1.ToString(), arg2.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a syntax token with a custom name, two arguments and additional properties.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="token">
        /// The syntax token to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax token.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(string name, in SyntaxToken token, T1 arg1, T2 arg2, params Pair[] properties) => CreateIssue(token.GetLocation(), properties, name, arg1.ToString(), arg2.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a location with a custom name, two arguments and additional properties.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
        protected Diagnostic Issue<T1, T2>(string name, Location location, T1 arg1, T2 arg2, params Pair[] properties) => CreateIssue(location, properties, name, arg1.ToString(), arg2.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a symbol with three arguments and additional properties.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the third argument.
        /// </typeparam>
        /// <param name="symbol">
        /// The symbol to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="arg3">
        /// The third argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the symbol.
        /// </returns>
        protected Diagnostic Issue<T1, T2, T3>(ISymbol symbol, T1 arg1, T2 arg2, T3 arg3, params Pair[] properties) => CreateIssue(symbol.Locations[0], properties, GetSymbolName(symbol), arg1.ToString(), arg2.ToString(), arg3.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a syntax node with a custom name, three arguments and additional properties.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the third argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="node">
        /// The syntax node to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="arg3">
        /// The third argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax node.
        /// </returns>
        protected Diagnostic Issue<T1, T2, T3>(string name, SyntaxNode node, T1 arg1, T2 arg2, T3 arg3, params Pair[] properties) => CreateIssue(node.GetLocation(), properties, name, arg1.ToString(), arg2.ToString(), arg3.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a syntax token with a custom name, three arguments and additional properties.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the third argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="token">
        /// The syntax token to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="arg3">
        /// The third argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the syntax token.
        /// </returns>
        protected Diagnostic Issue<T1, T2, T3>(string name, in SyntaxToken token, T1 arg1, T2 arg2, T3 arg3, params Pair[] properties) => CreateIssue(token.GetLocation(), properties, name, arg1.ToString(), arg2.ToString(), arg3.ToString());

        /// <summary>
        /// Creates a diagnostic issue for a location with a custom name, three arguments and additional properties.
        /// </summary>
        /// <typeparam name="T1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="T3">
        /// The type of the third argument.
        /// </typeparam>
        /// <param name="name">
        /// The custom name for the diagnostic message.
        /// </param>
        /// <param name="location">
        /// The location to create the issue for.
        /// </param>
        /// <param name="arg1">
        /// The first argument for the diagnostic message.
        /// </param>
        /// <param name="arg2">
        /// The second argument for the diagnostic message.
        /// </param>
        /// <param name="arg3">
        /// The third argument for the diagnostic message.
        /// </param>
        /// <param name="properties">
        /// The additional properties for the diagnostic.
        /// </param>
        /// <returns>
        /// A diagnostic issue for the location.
        /// </returns>
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

            switch (issues)
            {
                case Diagnostic[] array:
                {
                    if (array.Length > 0)
                    {
                        ReportDiagnostics(context, array);
                    }

                    return;
                }

                case IReadOnlyList<Diagnostic> list:
                {
                    if (list.Count > 0)
                    {
                        ReportDiagnostics(context, list);
                    }

                    return;
                }
            }

            ReportDiagnosticsEnumerable(context, issues);
        }

        private static void ReportDiagnostics(in SymbolAnalysisContext context, Diagnostic[] array)
        {
            for (int index = 0, length = array.Length; index < length; index++)
            {
                var issue = array[index];

                if (issue != null)
                {
                    context.ReportDiagnostic(issue);
                }
            }
        }

        private static void ReportDiagnostics(in SymbolAnalysisContext context, IReadOnlyList<Diagnostic> list)
        {
            for (int index = 0, count = list.Count; index < count; index++)
            {
                var issue = list[index];

                if (issue != null)
                {
                    context.ReportDiagnostic(issue);
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
            if (properties.Length is 0)
            {
                return Diagnostic.Create(m_rule, location, args);
            }

            var immutableProperties = ImmutableDictionary<string, string>.Empty;

            if (properties.Length is 1)
            {
                immutableProperties = immutableProperties.Add(properties[0].Key, properties[0].Value);
            }
            else
            {
                immutableProperties = immutableProperties.AddRange(properties.Select(_ => new KeyValuePair<string, string>(_.Key, _.Value)));
            }

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
            var compilation = context.Compilation;

            if (IsUnitTestAnalyzer)
            {
                if (ReferencesTestAssemblies(compilation) is false)
                {
                    // do not run analyzer if there are no tests contained
                    return;
                }
            }

            if (IsApplicable(compilation))
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

            if (compilation.GetTypeByMetadataName("NUnit.Framework.TestCaseAttribute") != null)
            {
                return SupportsNUnit;
            }

            if (compilation.GetTypeByMetadataName("Xunit.FactAttribute") != null)
            {
                return SupportsXUnit;
            }

            if (compilation.GetTypeByMetadataName("Xunit.TheoryAttribute") != null)
            {
                return SupportsXUnit;
            }

            if (compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute") != null)
            {
                return SupportsMSTest;
            }

            return false;
        }
    }
}