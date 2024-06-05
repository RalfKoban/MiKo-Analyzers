using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1401_TechnicalNamespacesAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1401";

        private static readonly HashSet<string> TechnicalNamespaces = new HashSet<string>
                                                                          {
                                                                              "Action", "Actions",
                                                                              "API", "Api", "APIs", "Apis",
                                                                              "Attributes",
                                                                              "Base", "Class", "Classes",
                                                                              "Compare", "Comparer", "Comparers",
                                                                              "Controller", "Controllers",
                                                                              "Dto", "DTO", "DTOs", "Dtos",
                                                                              "Enum", "Enums", "Enumeration", "Enumerations",
                                                                              "Error", "Errors",
                                                                              "Events", "EventArgument", "EventArguments",
                                                                              "Exception", "Exceptions",
                                                                              "Execution", "Executions",
                                                                              "Func", "Funcs", "Function", "Functions",
                                                                              "Imp", "Impl", "Impls", "Implementation", "Implementations",
                                                                              "Indexer", "Indexers",
                                                                              "Interface", "Interfaces", "Itf", "Itfs",
                                                                              "Module", "Modules",
                                                                              "Observer", "Observers",
                                                                              "Platform",
                                                                              "Provider", "Providers",
                                                                              "Proxies", "Proxy", "ServiceProxies", "ServiceProxy",
                                                                              "Record",
                                                                              "State", "States",
                                                                              "Struct", "Structs",
                                                                              "Type", "Types",
                                                                          };

        public MiKo_1401_TechnicalNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(IEnumerable<SyntaxToken> names)
        {
            foreach (var name in names)
            {
                if (TechnicalNamespaces.Contains(name.ValueText))
                {
                    yield return Issue(name);
                }
            }
        }
    }
}