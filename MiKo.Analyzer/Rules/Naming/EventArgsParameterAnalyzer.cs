﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Extensions;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EventArgsParameterAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1002";

        public EventArgsParameterAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.IsOverride) return Enumerable.Empty<Diagnostic>();

            var parameters = method.Parameters;
            if (parameters.Length == 2 && parameters[0].Type.ToString() == "object" && parameters[1].Type.InheritsFrom<System.EventArgs>())
            {
                // ignore the method as it is handled by EventHandlingMethodParametersAnalyzer
                return Enumerable.Empty<Diagnostic>();
            }

            var diagnostics = parameters
                                    .Where(_ => _.Type.InheritsFrom<System.EventArgs>() && _.Name != "e")
                                    .Select(_ => Diagnostic.Create(Rule, method.Locations[0], method.Name, _.Name, "e"))
                                    .ToList();
            return diagnostics;
        }
    }
}