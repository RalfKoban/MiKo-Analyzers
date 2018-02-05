using System.Collections.Concurrent;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules
{
    public abstract class Analyzer : Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer
    {
        private static readonly ConcurrentDictionary<string, DiagnosticDescriptor> KnownRules = new ConcurrentDictionary<string, DiagnosticDescriptor>();

        protected Analyzer(string category, string diagnosticId)
        {
            Rule = KnownRules.GetOrAdd(
                                       GetType().Name,
                                       name => new DiagnosticDescriptor(
                                                                        diagnosticId,
                                                                        LocalizableResource(name, "Title"),
                                                                        LocalizableResource(name, "MessageFormat"),
                                                                        category,
                                                                        DiagnosticSeverity.Warning,
                                                                        isEnabledByDefault: true,
                                                                        description: LocalizableResource(name, "Description")));
        }

        protected DiagnosticDescriptor Rule { get; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);


        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static LocalizableResourceString LocalizableResource(string prefix, string suffix) => new LocalizableResourceString(prefix + "_" + suffix, Resources.ResourceManager, typeof(Resources));
    }
}