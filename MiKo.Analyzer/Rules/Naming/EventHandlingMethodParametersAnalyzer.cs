using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Extensions;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EventHandlingMethodParametersAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1001";

        public EventHandlingMethodParametersAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            var parameters = method.Parameters;
            if (parameters.Length == 2)
            {
                var parameter1 = parameters[0];
                var parameter2 = parameters[1];

                if (parameter1.Type.ToString() == "object" && parameter2.Type.InheritsFrom<System.EventArgs>())
                {
                    var diagnostics = new List<Diagnostic>();
                    if (parameter1.Name != "sender") diagnostics.Add(Diagnostic.Create(Rule, method.Locations[0], method.Name, parameter1.Name, "sender"));
                    if (parameter2.Name != "e") diagnostics.Add(Diagnostic.Create(Rule, method.Locations[0], method.Name, parameter2.Name, "e"));
                    return diagnostics;
                }
            }

            return base.AnalyzeMethod(method);
        }
    }
}