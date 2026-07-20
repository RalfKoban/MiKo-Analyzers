using System;
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

            if (localFunctions.Length is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var semanticModel = compilation.GetSemanticModel(localFunctions[0].SyntaxTree);

            List<Diagnostic> issues = null;

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var function in localFunctions)
            {
                var localSymbol = function.GetSymbol(semanticModel);

                if (ShallAnalyzeLocalFunction(localSymbol))
                {
                    foreach (var issue in AnalyzeName(localSymbol, compilation))
                    {
                        if (issues is null)
                        {
                            issues = new List<Diagnostic>(1);
                        }

                        issues.Add(issue);
                    }
                }
            }

            return (IEnumerable<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
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

            if (symbol.IsOverride)
            {
                return false;
            }

            if (symbol.CanBeReferencedByName is false)
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

            if (symbol.ContainingSymbol is IMethodSymbol method)
            {
                if (method.IsOverride)
                {
                    var overriddenMethod = method.OverriddenMethod;

                    // analyze it in case the names do not match (in such case the user has given the parameter a different name)
                    return overriddenMethod != null && overriddenMethod.Parameters[method.Parameters.IndexOf(symbol)].Name != symbol.Name;
                }

                if (method.IsConstructor())
                {
                    if (method.HasAttribute("System.Text.Json.Serialization.JsonConstructorAttribute")
                     || method.HasAttribute("Newtonsoft.Json.JsonConstructorAttribute"))
                    {
                        // ignore Json constructors
                        return false;
                    }
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
                var betterName = NamesFinder.FindBetterNameForEntityMarker(symbolName);

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
            var betterName = NamesFinder.FindBetterNameForCollectionSuffix(symbol.Name);

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
        protected virtual void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
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
        /// Analyzes a single variable designation.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        protected virtual void AnalyzeVariableDesignation(SyntaxNodeAnalysisContext context)
        {
            var node = (VariableDesignationSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            var type = node.Parent.GetTypeSymbol(semanticModel);

            if (ShallAnalyze(type))
            {
                var issues = Analyze(semanticModel, type, node);

                ReportDiagnostics(context, issues);
            }
        }

        /// <summary>
        /// Analyzes a <see langword="foreach"/> statement.
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
        /// Analyzes a <see langword="for"/> statement.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        protected virtual void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (ForStatementSyntax)context.Node;

            AnalyzeVariableDeclaration(node.Declaration, context);
        }

        /// <summary>
        /// Analyzes a <see langword="using"/> statement.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        protected virtual void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (UsingStatementSyntax)context.Node;

            AnalyzeVariableDeclaration(node.Declaration, context);
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

        private void AnalyzeVariableDeclaration(VariableDeclarationSyntax variableDeclaration, in SyntaxNodeAnalysisContext context)
        {
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
    }
}