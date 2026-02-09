using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    /// <summary>
    /// Provides an abstract base analyzer for naming rules.
    /// </summary>
    public abstract class NamingAnalyzer : Analyzer
    {
        private static readonly string[] Splitters = { "Of", "With", "To", "In", "From", "For" };

        private static readonly ConcurrentDictionary<string, Pair> PluralNamesCache = new ConcurrentDictionary<string, Pair>(StringComparer.Ordinal);

        /// <summary>
        /// Initializes a new instance of the <see cref="NamingAnalyzer"/> class with the unique identifier of the diagnostic and the kind of symbol to analyze.
        /// </summary>
        /// <param name="diagnosticId">
        /// The diagnostic identifier.
        /// </param>
        /// <param name="symbolKind">
        /// One of the enumeration members that specifies the kind of symbol to analyze.
        /// The default is <see cref="SymbolKind.Method"/>.
        /// </param>
        protected NamingAnalyzer(string diagnosticId, in SymbolKind symbolKind = SymbolKind.Method) : base(nameof(Naming), diagnosticId, symbolKind)
        {
        }

        /// <summary>
        /// Creates a proposal for a better name.
        /// </summary>
        /// <param name="betterName">
        /// The better name to propose.
        /// </param>
        /// <returns>
        /// An array of key-value pairs containing the better name proposal.
        /// </returns>
        protected static Pair[] CreateBetterNameProposal(string betterName) => new[] { new Pair(Constants.AnalyzerCodeFixSharedData.BetterName, betterName) };

        /// <summary>
        /// Finds a better name for a symbol that contains entity marker suffixes.
        /// </summary>
        /// <param name="symbolName">
        /// The name of the symbol to analyze.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the better name without entity marker suffixes.
        /// </returns>
        protected static string FindBetterNameForEntityMarker(string symbolName)
        {
            var expected = HandleSpecialEntityMarkerSituations(symbolName);

            if (expected.HasCollectionMarker())
            {
                var plural = FindBetterNameForCollectionSuffix(expected);

                // symbol may have both Entity and Collection marker, such as 'ModelCollection', so 'plural' may be null
                expected = plural ?? (symbolName[0].IsUpperCase() ? Constants.Entities : Constants.entities);
            }

            return expected;
        }

        /// <summary>
        /// Finds a better name for a symbol that uses collection suffixes instead of plural forms.
        /// </summary>
        /// <param name="name">
        /// The name to analyze.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the better name using plural forms, or <see langword="null"/> if the name is already in plural form.
        /// </returns>
        protected static string FindBetterNameForCollectionSuffix(string name)
        {
            var pluralName = FindPluralName(name, out _);

            return name.Equals(pluralName, StringComparison.Ordinal) ? null : pluralName;
        }

        /// <summary>
        /// Finds a better name for a symbol that uses structural design pattern suffixes.
        /// </summary>
        /// <param name="name">
        /// The name to analyze.
        /// </param>
        /// <param name="prefix">
        /// The prefix to preserve in the name.
        /// The default is <c>""</c>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the better name with corrected structural design pattern suffix.
        /// </returns>
        protected static string FindBetterNameForStructuralDesignPattern(string name, string prefix = "")
        {
            var startIndex = prefix.Length;

            foreach (var pair in Constants.Names.StructuralDesignPatternNames)
            {
                if (name.EndsWith(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    var count = name.Length - pair.Key.Length;

                    if (count is 0)
                    {
                        return pair.Value;
                    }

                    var builder = StringBuilderCache.Acquire();

                    if (startIndex > 0)
                    {
                        builder.Append(prefix);
                    }

                    builder.Append(pair.Value);
                    builder.Append(name, startIndex, count - startIndex);
                    builder.ToUpperCaseAt(pair.Value.Length + startIndex);

                    return builder.ToStringAndRelease();
                }
            }

            return name;
        }

#pragma warning disable CA1021

        /// <summary>
        /// Finds the plural form of a name.
        /// </summary>
        /// <param name="originalName">
        /// The original name to analyze.
        /// </param>
        /// <param name="singularName">
        /// On successful return, contains the singular form of the name.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the plural form of the name, or <see langword="null"/> if the name is already in plural form.
        /// </returns>
        protected static string FindPluralName(string originalName, out string singularName)
        {
            var found = PluralNamesCache.GetOrAdd(
                                              originalName,
                                              _ =>
                                                   {
                                                       var plural = FindPluralName(_.AsSpan(), out var singular);

                                                       return new Pair(plural, singular);
                                                   });

            singularName = found.Value;

            return found.Key;
        }

        /// <summary>
        /// Finds the plural form of a name.
        /// </summary>
        /// <param name="originalName">
        /// The original name to analyze.
        /// </param>
        /// <param name="singularName">
        /// On successful return, contains the singular form of the name.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the plural form of the name, or <see langword="null"/> if the name is already in plural form.
        /// </returns>
        protected static string FindPluralName(in ReadOnlySpan<char> originalName, out string singularName)
        {
            if (originalName.EndsWith('s'))
            {
                singularName = originalName.ToString();

                var pluralName = Pluralizer.GetPluralName(singularName, StringComparison.Ordinal);

                if (pluralName != null && originalName.SequenceEqual(pluralName.AsSpan()))
                {
                    singularName = originalName.Slice(0, originalName.Length - 1).ToString();

                    return null; // seems the original name is already the plural name, so we do not report that
                }

                return pluralName;
            }

            if (originalName.EndsWith("Map") || originalName.EndsWith("Cache"))
            {
                singularName = originalName.ToString();

                return null; // seems the original name is already the plural name, so we do not report that
            }

            var index = originalName.IndexOfAny(Splitters);

            if (index > 0)
            {
                var nameToInspect = originalName.Slice(0, index);

                var pluralName = FindPluralName(nameToInspect, out singularName);

                if (pluralName is null)
                {
                    return null; // seems the original name is already the plural name, so we do not report that
                }

                var remainingPart = originalName.Slice(index);

                singularName = singularName.ConcatenatedWith(remainingPart);

                return pluralName.ConcatenatedWith(remainingPart);
            }
            else
            {
                var pluralName = originalName.EndsWithNumber()
                                 ? originalName.WithoutNumberSuffix()
                                 : originalName;

                singularName = pluralName.ToString();

                if (pluralName.EndsWithAny(Constants.Markers.Collections, StringComparison.OrdinalIgnoreCase))
                {
                    singularName = singularName.AsCachedBuilder()
                                               .ReplaceWithProbe("lementNodeList", "lementList")
                                               .ReplaceWithProbe("lementReferenceNodeList", "lementList")
                                               .ToStringAndRelease();

                    return Pluralizer.GetPluralName(singularName, StringComparison.OrdinalIgnoreCase, Constants.Markers.Collections);
                }

                return Pluralizer.GetPluralName(singularName);
            }
        }
#pragma warning restore CA1021

        /// <summary>
        /// Gets the field prefix from a field name.
        /// </summary>
        /// <param name="fieldName">
        /// The field name to analyze.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the field prefix, or the <see cref="string.Empty"/> string ("") if no prefix is found.
        /// </returns>
        protected static string GetFieldPrefix(string fieldName) => GetFieldPrefix(fieldName.AsSpan());

        /// <summary>
        /// Gets the field prefix from a field name.
        /// </summary>
        /// <param name="fieldName">
        /// The field name to analyze.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the field prefix, or the <see cref="string.Empty"/> string ("") if no prefix is found.
        /// </returns>
        protected static string GetFieldPrefix(in ReadOnlySpan<char> fieldName)
        {
            var fieldPrefixes = Constants.Markers.FieldPrefixes;

            for (int index = 0, prefixesLength = fieldPrefixes.Length; index < prefixesLength; index++)
            {
                var prefix = fieldPrefixes[index];

                if (prefix.Length > 0 && fieldName.StartsWith(prefix))
                {
                    return prefix;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Determines whether a name ends with a structural design pattern suffix.
        /// </summary>
        /// <param name="name">
        /// The name to analyze.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the name ends with a structural design pattern suffix; otherwise, <see langword="false"/>.
        /// </returns>
        protected static bool IsNameForStructuralDesignPattern(string name) => name.EndsWithAny(Constants.Names.StructuralDesignPatternNames.Keys, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                ? AnalyzeName(symbol, compilation)
                                                                                                                                : Array.Empty<Diagnostic>();

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                           ? AnalyzeName(symbol, compilation)
                                                                                                                           : Array.Empty<Diagnostic>();

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation)
        {
            IEnumerable<Diagnostic> namingIssues = Array.Empty<Diagnostic>();
            IEnumerable<Diagnostic> localFunctionIssues = Array.Empty<Diagnostic>();

            if (ShallAnalyze(symbol))
            {
                namingIssues = AnalyzeName(symbol, compilation);
            }

            if (ShallAnalyzeLocalFunctions(symbol))
            {
                localFunctionIssues = AnalyzeLocalFunctions(symbol, compilation);
            }

            if (localFunctionIssues.IsEmptyArray())
            {
                // do not perform a check for empty arrays because in case 'namingIssues' is an empty array, we would return the empty array anyway and in case it is not, we would return it as well
                return namingIssues;
            }

            return namingIssues.Concat(localFunctionIssues);
        }

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                              ? AnalyzeName(symbol, compilation)
                                                                                                                              : Array.Empty<Diagnostic>();

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                        ? AnalyzeName(symbol, compilation)
                                                                                                                        : Array.Empty<Diagnostic>();

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                        ? AnalyzeName(symbol, compilation)
                                                                                                                        : Array.Empty<Diagnostic>();

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                ? AnalyzeName(symbol, compilation)
                                                                                                                                : Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes the local functions within a method.
        /// </summary>
        /// <param name="symbol">
        /// The method symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that provides access to the semantic model.
        /// </param>
        /// <returns>
        /// A collection of diagnostics for naming issues found in local functions.
        /// </returns>
        protected IEnumerable<Diagnostic> AnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation)
        {
            var localFunctions = symbol.GetLocalFunctions();

            if (localFunctions.Count is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var semanticModel = compilation.GetSemanticModel(localFunctions[0].SyntaxTree);

            return localFunctions.Select(_ => _.GetSymbol(semanticModel))
                                 .Where(ShallAnalyzeLocalFunction)
                                 .SelectMany(_ => AnalyzeName(_, compilation));
        }

        /// <summary>
        /// Determines whether the specified namespace shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The namespace symbol to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the namespace shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(INamespaceSymbol symbol)
        {
            if (symbol is null)
            {
                // code seems to be obfuscated or contains no valid symbol, so ignore it silently
                return false;
            }

            return symbol.CanBeReferencedByName && symbol.IsGlobalNamespace is false;
        }

        /// <summary>
        /// Determines whether the specified type shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The type symbol to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(ITypeSymbol symbol)
        {
            if (symbol is null)
            {
                // code seems to be obfuscated or contains no valid symbol, so ignore it silently
                return false;
            }

            return symbol.CanBeReferencedByName;
        }

        /// <summary>
        /// Determines whether the specified method shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The method symbol to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the method shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol is null)
            {
                // code seems to be obfuscated or contains no valid symbol, so ignore it silently
                return false;
            }

            if (symbol.CanBeReferencedByName is false)
            {
                return false;
            }

            if (symbol.IsOverride)
            {
                return false;
            }

            switch (symbol.MethodKind)
            {
                case MethodKind.Ordinary:
                    return symbol.IsInterfaceImplementation() is false;

                case MethodKind.LocalFunction:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the specified property shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The property symbol to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the property shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(IPropertySymbol symbol)
        {
            if (symbol is null)
            {
                // code seems to be obfuscated or contains no valid symbol, so ignore it silently
                return false;
            }

            return symbol.CanBeReferencedByName && symbol.IsOverride is false && symbol.IsInterfaceImplementation() is false;
        }

        /// <summary>
        /// Determines whether the specified event shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The event symbol to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the event shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(IEventSymbol symbol)
        {
            if (symbol is null)
            {
                // code seems to be obfuscated or contains no valid symbol, so ignore it silently
                return false;
            }

            return symbol.CanBeReferencedByName && symbol.IsOverride is false && symbol.IsInterfaceImplementation() is false;
        }

        /// <summary>
        /// Determines whether the specified field shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The field symbol to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the field shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(IFieldSymbol symbol)
        {
            if (symbol is null)
            {
                // code seems to be obfuscated or contains no valid symbol, so ignore it silently
                return false;
            }

            return symbol.CanBeReferencedByName && symbol.IsOverride is false;
        }

        /// <summary>
        /// Determines whether the specified parameter shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The parameter symbol to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the parameter shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(IParameterSymbol symbol)
        {
            if (symbol is null)
            {
                // code seems to be obfuscated or contains no valid symbol, so ignore it silently
                return false;
            }

            if (symbol.CanBeReferencedByName is false)
            {
                return false;
            }

            if (symbol.IsOverride)
            {
                return false;
            }

            if (symbol.ContainingSymbol is IMethodSymbol method && method.IsConstructor())
            {
                if (method.HasAttribute("System.Text.Json.Serialization.JsonConstructorAttribute")
                 || method.HasAttribute("Newtonsoft.Json.JsonConstructorAttribute"))
                {
                    // ignore Json constructors
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the local functions of the specified method shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The method symbol to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the local functions shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => false;

        /// <summary>
        /// Determines whether the specified local function shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The local function symbol to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the local function shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => false;

        /// <summary>
        /// Analyzes the name of a namespace.
        /// </summary>
        /// <param name="symbol">
        /// The namespace symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that provides access to the semantic model.
        /// </param>
        /// <returns>
        /// A collection of diagnostics for naming issues.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes the name of a type.
        /// </summary>
        /// <param name="symbol">
        /// The type symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that provides access to the semantic model.
        /// </param>
        /// <returns>
        /// A collection of diagnostics for naming issues.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes the name of a method.
        /// </summary>
        /// <param name="symbol">
        /// The method symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that provides access to the semantic model.
        /// </param>
        /// <returns>
        /// A collection of diagnostics for naming issues.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes the name of a property.
        /// </summary>
        /// <param name="symbol">
        /// The property symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that provides access to the semantic model.
        /// </param>
        /// <returns>
        /// A collection of diagnostics for naming issues.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes the name of an event.
        /// </summary>
        /// <param name="symbol">
        /// The event symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that provides access to the semantic model.
        /// </param>
        /// <returns>
        /// A collection of diagnostics for naming issues.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes the name of a field.
        /// </summary>
        /// <param name="symbol">
        /// The field symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that provides access to the semantic model.
        /// </param>
        /// <returns>
        /// A collection of diagnostics for naming issues.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes the name of a parameter.
        /// </summary>
        /// <param name="symbol">
        /// The parameter symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that provides access to the semantic model.
        /// </param>
        /// <returns>
        /// A collection of diagnostics for naming issues.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes entity marker suffixes in a symbol name.
        /// </summary>
        /// <param name="symbol">
        /// The symbol to analyze.
        /// </param>
        /// <returns>
        /// A collection of diagnostics for entity marker issues.
        /// </returns>
        protected IEnumerable<Diagnostic> AnalyzeEntityMarkers(ISymbol symbol)
        {
            var symbolName = symbol.Name;

            if (symbolName.HasEntityMarker())
            {
                var betterName = FindBetterNameForEntityMarker(symbolName);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }

        /// <summary>
        /// Analyzes collection suffixes in a symbol name.
        /// </summary>
        /// <param name="symbol">
        /// The symbol to analyze.
        /// </param>
        /// <returns>
        /// A diagnostic for collection suffix issues, or <see langword="null"/> if no issues are found.
        /// </returns>
        protected Diagnostic AnalyzeCollectionSuffix(ISymbol symbol)
        {
            var betterName = FindBetterNameForCollectionSuffix(symbol.Name);

            if (betterName.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Issue(symbol, betterName, CreateBetterNameProposal(betterName));
        }

        /// <summary>
        /// Analyzes a local declaration statement.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        protected void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (LocalDeclarationStatementSyntax)context.Node;

            if (node.IsConst)
            {
                return;
            }

            var semanticModel = context.SemanticModel;
            var type = node.Declaration.GetTypeSymbol(semanticModel);

            if (type is null)
            {
                // may happen for a "ref var xyz" value
                return;
            }

            if (ShallAnalyze(type))
            {
                var issues = AnalyzeIdentifiers(semanticModel, type, node.Declaration.Variables.ToArray(_ => _.Identifier));

                ReportDiagnostics(context, issues);
            }
        }

        /// <summary>
        /// Analyzes a declaration expression.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        protected void AnalyzeDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            var declaration = (DeclarationExpressionSyntax)context.Node;

            var issues = Analyze(context.SemanticModel, declaration);

            ReportDiagnostics(context, issues);
        }

        /// <summary>
        /// Analyzes a declaration pattern.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        protected virtual void AnalyzeDeclarationPattern(SyntaxNodeAnalysisContext context)
        {
            var node = (DeclarationPatternSyntax)context.Node;

            var semanticModel = context.SemanticModel;
            var type = node.Type.GetTypeSymbol(semanticModel);

            if (ShallAnalyze(type))
            {
                var issues = Analyze(semanticModel, type, node.Designation);

                ReportDiagnostics(context, issues);
            }
        }

        /// <summary>
        /// Analyzes a tuple element.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        protected virtual void AnalyzeTupleElement(SyntaxNodeAnalysisContext context)
        {
            var node = (TupleElementSyntax)context.Node;

            if (node.IsMissing)
            {
                // we cannot do something here
                return;
            }

            var semanticModel = context.SemanticModel;
            var type = node.Type.GetTypeSymbol(semanticModel);

            if (ShallAnalyze(type))
            {
                var issues = AnalyzeIdentifiers(semanticModel, type, node.Identifier);

                ReportDiagnostics(context, issues);
            }
        }

        /// <summary>
        /// Analyzes a tuple expression.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        protected virtual void AnalyzeTupleExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (TupleExpressionSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            foreach (var argument in node.Arguments)
            {
                if (argument.Expression is DeclarationExpressionSyntax declaration)
                {
                    var type = argument.GetTypeSymbol(semanticModel);

                    if (ShallAnalyze(type))
                    {
                        var issues = Analyze(semanticModel, declaration);

                        ReportDiagnostics(context, issues);
                    }
                }
            }
        }

        /// <summary>
        /// Analyzes a foreach statement.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        protected virtual void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (ForEachStatementSyntax)context.Node;

            var semanticModel = context.SemanticModel;
            var type = node.Type.GetTypeSymbol(semanticModel);

            if (ShallAnalyze(type))
            {
                var issues = AnalyzeIdentifiers(semanticModel, type, node.Identifier);

                ReportDiagnostics(context, issues);
            }
        }

        /// <summary>
        /// Analyzes a for statement.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        protected virtual void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (ForStatementSyntax)context.Node;
            var variableDeclaration = node.Declaration;

            if (variableDeclaration is null)
            {
                // ignore variables that are not set
                return;
            }

            var semanticModel = context.SemanticModel;
            var type = variableDeclaration.GetTypeSymbol(semanticModel);

            if (ShallAnalyze(type))
            {
                var issues = AnalyzeIdentifiers(semanticModel, type, variableDeclaration.Variables.ToArray(_ => _.Identifier));

                ReportDiagnostics(context, issues);
            }
        }

        /// <summary>
        /// Analyzes identifiers for naming issues.
        /// </summary>
        /// <param name="semanticModel">
        /// The semantic model.
        /// </param>
        /// <param name="type">
        /// The type of the identifiers.
        /// </param>
        /// <param name="identifiers">
        /// The identifiers to analyze.
        /// </param>
        /// <returns>
        /// A collection of diagnostics for naming issues.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers) => Array.Empty<Diagnostic>();

        private static string HandleSpecialEntityMarkerSituations(string symbolName)
        {
            var name = symbolName.Without(Constants.Markers.Models);

            switch (name.Length)
            {
                case 0:
                    return symbolName[0].IsUpperCase() ? Constants.Entity : Constants.entity;

                case 1:
                    switch (name)
                    {
                        case "s": return Constants.entities;
                        case Constants.Markers.MemberFieldPrefix: return Constants.Markers.MemberFieldPrefix + Constants.entity;
                        default: return name;
                    }

                case 2:
                    switch (name)
                    {
                        case Constants.Markers.AlternativeMemberFieldPrefix: return Constants.Markers.AlternativeMemberFieldPrefix + Constants.entity;
                        case Constants.Markers.StaticFieldPrefix: return Constants.Markers.StaticFieldPrefix + Constants.entity;
                        case Constants.Markers.ThreadStaticFieldPrefix: return Constants.Markers.ThreadStaticFieldPrefix + Constants.entity;
                        default: return name;
                    }

                default:
                {
                    var index = 0;

                    var fieldPrefixes = Constants.Markers.FieldPrefixes;

                    for (int i = 0, length = fieldPrefixes.Length; i < length; i++)
                    {
                        var prefix = fieldPrefixes[i];

                        if (symbolName.StartsWith(prefix, StringComparison.Ordinal))
                        {
                            index = prefix.Length;
                        }
                    }

                    if (symbolName[index].IsUpperCase() && name[index].IsLowerCase())
                    {
                        return name.ToUpperCaseAt(index);
                    }

                    if (symbolName[index].IsLowerCase() && name[index].IsUpperCase())
                    {
                        return name.ToLowerCaseAt(index);
                    }

                    return name;
                }
            }
        }

        private IEnumerable<Diagnostic> Analyze(SemanticModel semanticModel, DeclarationExpressionSyntax declaration)
        {
            if (ShallAnalyzeLocal(declaration))
            {
                var type = declaration.GetTypeSymbol(semanticModel);

                if (ShallAnalyze(type))
                {
                    return Analyze(semanticModel, type, declaration.Designation);
                }
            }

            return Array.Empty<Diagnostic>();

            bool ShallAnalyzeLocal(DeclarationExpressionSyntax d)
            {
                switch (d.Parent)
                {
                    case ArgumentSyntax _:
                    case AssignmentExpressionSyntax _: // deconstructions
                        return true;

                    default:
                        return false;
                }
            }
        }

        private IEnumerable<Diagnostic> Analyze(SemanticModel semanticModel, ITypeSymbol type, VariableDesignationSyntax node)
        {
            switch (node)
            {
                case SingleVariableDesignationSyntax s:
                    return AnalyzeIdentifiers(semanticModel, type, s.Identifier);

                case ParenthesizedVariableDesignationSyntax s:
                    return s.Variables.SelectMany(_ => Analyze(semanticModel, type, _));

                default:
                    return Array.Empty<Diagnostic>();
            }
        }
    }
}