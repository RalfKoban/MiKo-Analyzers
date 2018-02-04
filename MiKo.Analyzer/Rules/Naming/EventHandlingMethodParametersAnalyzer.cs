using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EventHandlingMethodParametersAnalyzer : DiagnosticAnalyzer
    {
        public const string Id = "MiKo_1001";

        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
                                                                                     Id,
                                                                                     LocalizableResource("Title"),
                                                                                     LocalizableResource("MessageFormat"),
                                                                                     Category,
                                                                                     DiagnosticSeverity.Warning,
                                                                                     isEnabledByDefault: true,
                                                                                     description: LocalizableResource("Description"));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);

        private static void AnalyzeMethod(SymbolAnalysisContext context)
        {
            var diagnostics = AnalyzeMethod((IMethodSymbol)context.Symbol);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }

        }

        private static IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            var parameters = method.Parameters;
            if (parameters.Length == 2)
            {
                var parameter1 = parameters[0];
                var parameter2 = parameters[1];

                if (parameter1.Type.ToString() == "object" && InheritsFrom<System.EventArgs>(parameter2.Type))
                {
                    var diagnostics = new List<Diagnostic>();
                    if (parameter1.Name != "sender") diagnostics.Add(Diagnostic.Create(Rule, method.Locations[0], method.Name, parameter1.Name, "sender"));
                    if (parameter2.Name != "e") diagnostics.Add(Diagnostic.Create(Rule, method.Locations[0], method.Name, parameter2.Name, "e"));
                    return diagnostics;
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static LocalizableResourceString LocalizableResource(string suffix) => new LocalizableResourceString(nameof(EventHandlingMethodParametersAnalyzer) + "_" + suffix, Resources.ResourceManager, typeof(Resources));

        private static bool InheritsFrom<T>(ITypeSymbol symbol)
        {
            var baseClass = typeof(T).FullName;

            while (true)
            {
                if (symbol.ToString() == baseClass) return true;
                if (symbol.BaseType == null) return false;

                symbol = symbol.BaseType;
            }
        }
    }
}