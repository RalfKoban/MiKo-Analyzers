﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3072_MethodReturnsListAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3072";

        public MiKo_3072_MethodReturnsListAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol)
                                                                   && symbol.ReturnsVoid is false
                                                                   && symbol.DeclaredAccessibility != Accessibility.Private
                                                                   && symbol.ReturnType.TypeKind is TypeKind.Class
                                                                   && symbol.IsInterfaceImplementation() is false;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.ReturnType.ContainingNamespace.FullyQualifiedName() is "System.Collections.Generic")
            {
                var name = symbol.ReturnType.Name;

                switch (name)
                {
                    case "List":
                    case "Dictionary":
                        return new[] { Issue(symbol.ReturnType, name) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}