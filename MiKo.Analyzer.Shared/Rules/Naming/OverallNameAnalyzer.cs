using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class OverallNameAnalyzer : LocalVariableNamingAnalyzer
    {
        protected OverallNameAnalyzer(string diagnosticId) : base(diagnosticId)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            InitializeCore(context, SymbolKind.Namespace, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field, SymbolKind.Parameter);

            base.InitializeCore(context);
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            List<Diagnostic> overallIssues = null;

            foreach (var identifier in identifiers)
            {
                var symbol = identifier.GetSymbol(semanticModel);

                if (symbol is null)
                {
                    // code seems to be obfuscated or contains no valid symbol, so ignore it silently
                    continue;
                }

                var issues = AnalyzeName(symbol);

                if (issues.Length is 0)
                {
                    continue;
                }

                if (overallIssues is null)
                {
                    overallIssues = new List<Diagnostic>(issues.Length);
                }

                overallIssues.AddRange(issues);
            }

            return (IEnumerable<Diagnostic>)overallIssues ?? Array.Empty<Diagnostic>();
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol?.IsExtern is false && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyze(IParameterSymbol symbol)
        {
            if (base.ShallAnalyze(symbol))
            {
                if (symbol.ContainingSymbol is IMethodSymbol method)
                {
                    if (method.IsExtern)
                    {
                        return false;
                    }

                    if (method.IsInterfaceImplementation())
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected abstract Diagnostic[] AnalyzeName(string symbolName, ISymbol symbol, string prefix = "");

        private Diagnostic[] AnalyzeName(ISymbol symbol) => AnalyzeName(symbol.Name, symbol);

        private Diagnostic[] AnalyzeName(IFieldSymbol symbol)
        {
            var symbolName = symbol.Name;

            // remove any field marker
            var fieldPrefix = GetFieldPrefix(symbolName);
            var prefixLength = fieldPrefix.Length;

            return prefixLength > 0
                   ? AnalyzeName(symbolName.Substring(prefixLength), symbol, fieldPrefix)
                   : AnalyzeName(symbolName, symbol);
        }
    }
}