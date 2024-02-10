using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3042_EventArgsInterfaceAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3042";

        public MiKo_3042_EventArgsInterfaceAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsEventArgs();

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            var interfaces = symbol.Interfaces;

            if (interfaces.Any())
            {
                var names = interfaces.ToHashSet(_ => _.Name);

                if (symbol.GetSyntax() is ClassDeclarationSyntax declaration)
                {
                    var baseList = declaration.BaseList;

                    if (baseList != null)
                    {
                        var baseTypes = baseList.Types;

                        // keep in local variable to avoid multiple requests (see Roslyn implementation)
                        var baseTypesCount = baseTypes.Count;

                        for (var index = 0; index < baseTypesCount; index++)
                        {
                            var type = baseTypes[index];
                            var typeName = type.Type.GetName();

                            if (names.Contains(typeName))
                            {
                                yield return Issue(symbol.Name, type);
                            }
                        }
                    }
                }
            }
        }
    }
}