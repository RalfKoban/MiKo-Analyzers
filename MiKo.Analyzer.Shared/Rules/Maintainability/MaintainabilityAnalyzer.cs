﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MaintainabilityAnalyzer : Analyzer
    {
        protected MaintainabilityAnalyzer(string diagnosticId, in SymbolKind kind = SymbolKind.Method) : base(nameof(Maintainability), diagnosticId, kind)
        {
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                ? Analyze(symbol, compilation)
                                                                                                                                : Array.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                           ? Analyze(symbol, compilation)
                                                                                                                           : Array.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                          ? Analyze(symbol, compilation)
                                                                                                                          : Array.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                              ? Analyze(symbol, compilation)
                                                                                                                              : Array.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                        ? Analyze(symbol, compilation)
                                                                                                                        : Array.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                        ? Analyze(symbol, compilation)
                                                                                                                        : Array.Empty<Diagnostic>();

        protected virtual bool ShallAnalyze(INamespaceSymbol symbol) => true;

        protected virtual bool ShallAnalyze(INamedTypeSymbol symbol) => true;

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsOverride is false;

        protected virtual bool ShallAnalyze(IPropertySymbol symbol) => symbol.IsOverride is false;

        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => symbol.IsOverride is false;

        protected virtual bool ShallAnalyze(IEventSymbol symbol) => symbol.IsOverride is false;

        protected virtual IEnumerable<Diagnostic> Analyze(INamespaceSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> Analyze(IPropertySymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> Analyze(IEventSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected Diagnostic IssueOnType(ITypeSymbol type, ISymbol container)
        {
            // detect for same assembly to avoid AD0001 (which reports that the return type is in a different compilation than the method/property)
            if (type.Locations.IsEmpty || SymbolEqualityComparer.Default.Equals(container.ContainingAssembly, type.ContainingAssembly) is false)
            {
                switch (container.GetSyntax())
                {
                    case MethodDeclarationSyntax method: return Issue(type.ToString(), method.ReturnType);
                    case BasePropertyDeclarationSyntax property: return Issue(type.ToString(), property.Type);
                    case ParameterSyntax parameter: return Issue(type.ToString(), parameter.Type);
                }
            }

            return Issue(type);
        }
    }
}