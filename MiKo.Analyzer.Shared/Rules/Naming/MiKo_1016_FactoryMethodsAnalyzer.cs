﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1016_FactoryMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1016";

        public MiKo_1016_FactoryMethodsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsFactory();

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.IsConstructor())
            {
                return false;
            }

            // ignore private methods or those that do not have any accessibility (means they are also private)
            return symbol.IsPubliclyVisible() && base.ShallAnalyze(symbol);
        }

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => false; // do not consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => symbol.GetNamedMethods().SelectMany(_ => AnalyzeMethod(_, compilation));

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.Name.StartsWith(Constants.Names.Create, StringComparison.Ordinal))
            {
                return Array.Empty<Diagnostic>();
            }

            var betterName = FindBetterName(symbol);

            return new[] { Issue(symbol, CreateBetterNameProposal(betterName)) };
        }

        private static string FindBetterName(IMethodSymbol symbol)
        {
            var returnTypeName = symbol.ReturnType.Name;

            if (symbol.ContainingType.Name == returnTypeName + Constants.Names.Factory)
            {
                // we have a factory that has the name of the type to return, so the method shall be 'Create' only
                return Constants.Names.Create;
            }

            if (symbol.ReturnType.IsGeneric())
            {
                // we have a generic type, so we do not know an exact name
                return Constants.Names.Create;
            }

            // we have a concrete type but a multiple-types factory
            return Constants.Names.Create + returnTypeName;
        }
    }
}