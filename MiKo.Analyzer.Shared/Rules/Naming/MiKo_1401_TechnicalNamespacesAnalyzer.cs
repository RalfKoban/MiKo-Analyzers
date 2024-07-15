using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1401_TechnicalNamespacesAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1401";

        private static readonly HashSet<string> TechnicalNamespaces = CreateTechnicalNamespaces();

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

//// ncrunch: rdi off

        private static HashSet<string> CreateTechnicalNamespaces()
        {
            var names = new[]
                            {
                                // TODO RKN: Shall this be acceptable to support ASP .NET Core projects based on Clean Architecture using API and Domain entities?
                                // "API", "Api", "APIs", "Apis",
                                // "Domain", "Domains",
                                // "Interceptors",
                                // "Controller", "Controllers",
                                // "Entities",
                                "Action", "Actions",
                                "Applications",
                                "Attributes",
                                "Base", "Class", "Classes",
                                "Client", "Clients", "HttpClient", "HttpClients",
                                "Compare", "Comparer", "Comparers",
                                "Constants",
                                "Data", "Database", "Databases",
                                "Dependency", "Dependencies", "DependencyInjection",
                                "Dto", "DTO", "DTOs", "Dtos",
                                "Enum", "Enums", "Enumeration", "Enumerations",
                                "Error", "Errors",
                                "Events", "EventArgument", "EventArguments",
                                "Exception", "Exceptions",
                                "Execution", "Executions",
                                "Factories",
                                "Filters",
                                "Func", "Funcs", "Function", "Functions",
                                "Handler", "Handlers",
                                "Imp", "Impl", "Impls", "Implementation", "Implementations",
                                "Indexer", "Indexers",
                                "Interactions",
                                "Interface", "Interfaces", "Itf", "Itfs",
                                "Middleware",
                                "Mocks",
                                "Module", "Modules",
                                "MVC", "MVVM",
                                "Observer", "Observers",
                                "Pipelines",
                                "Platform",
                                "Provider", "Providers",
                                "Proxies", "Proxy",
                                "Queries",
                                "Record",
                                "Repositories",
                                "Resources",
                                "Service", "Services", "ServiceProxies", "ServiceProxy", "ApplicationService", "ApplicationServices", "Microservice", "MicroService", "Microservices", "MicroServices",
                                "State", "States", "Statements",
                                "Struct", "Structs",
                                "Type", "Types",
                                "Transaction", "Transactions",
                                "ValueObject", "ValueObjects",
                                "Wrapper", "Wrappers",
                            };

            var result = new HashSet<string>
                             {
                                 "Core",
                                 "Shared",
                             };

            foreach (var name in names)
            {
                result.Add("Core" + name);
                result.Add("Shared" + name);
                result.Add(name);
            }

            return result;
        }

//// ncrunch: rdi default
    }
}