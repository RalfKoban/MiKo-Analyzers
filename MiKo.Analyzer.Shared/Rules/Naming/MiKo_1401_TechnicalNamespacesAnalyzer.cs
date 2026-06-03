using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1401_TechnicalNamespacesAnalyzer : NamespaceNamingAnalyzer
    {
        public const string Id = "MiKo_1401";

        private static readonly HashSet<string> TechnicalNamespaces = CreateTechnicalNamespaces();
        private static readonly HashSet<string> AlreadyAcceptedNamespaces = new HashSet<string>();

        public MiKo_1401_TechnicalNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeNamespaceName(in ReadOnlySpan<SyntaxToken> namespaceNames)
        {
            List<Diagnostic> issues = null;

            foreach (var name in namespaceNames)
            {
                var namespaceName = name.ValueText;

                // as namespaces are highly repetitive across the files of a solution, we first check if we already inspected such name (assumed, there are fewer namespace names than the technical ones)
                if (AlreadyAcceptedNamespaces.Contains(namespaceName))
                {
                    // seems we found the name ok, so let's ignore it
                    continue;
                }

                if (TechnicalNamespaces.Contains(namespaceName))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(name));
                }
                else
                {
                    AlreadyAcceptedNamespaces.Add(namespaceName);
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }

//// ncrunch: rdi off

        private static HashSet<string> CreateTechnicalNamespaces()
        {
            var names = new[]
                            {
                                "Action", "Actions",
                                "API", "Api", "APIs", "Apis",
                                "Applications",
                                "Attributes",
                                "Builder", "Builders",
                                "Base", "Class", "Classes",
                                "Client", "Clients", "HttpClient", "HttpClients",
                                "Command", "Commands", "CommandPattern", "CommandPatterns",
                                "Compare", "Comparer", "Comparers",
                                "Constants",
                                "Contract", "Contracts",
                                "Controller", "Controllers",
                                "Data", "Database", "Databases",
                                "Dependency", "Dependencies", "DependencyInjection",
                                "Domain", "Domains",
                                "Dto", "DTO", "DTOs", "Dtos",
                                "Entities",
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
                                "Interactions", "Interceptors",
                                "Interface", "Interfaces", "Itf", "Itfs", "Intf", "Intfs", "Intfc", "Intfcs", "Intrfc", "Intrfcs",
                                "Memento", "Mementos",
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
                                "Stack", "Stacks",
                                "State", "States", "Statements",
                                "Struct", "Structs",
                                "Type", "Types",
                                "Transaction", "Transactions",
                                "UiOperations", "UiOperation",
                                "UIOperations", "UIOperation",
                                "UndoRedo",
                                "ValueObject", "ValueObjects",
                                "Wrapper", "Wrappers",
                            };

            var results = new HashSet<string>
                              {
                                  "Shared",
                                  "Technical",
                                  "TechnicalDesign",
                              };

            foreach (var name in names)
            {
                results.Add(name);

                results.Add(name + "Core");
                results.Add(name + "Shared");
                results.Add(name + "Design");
                results.Add(name + "Technical");
                results.Add(name + "TechnicalDesign");

                results.Add("Core" + name);
                results.Add("Shared" + name);
                results.Add("Technical" + name);
                results.Add("TechnicalDesign" + name);
                results.Add("Technical" + name + "Design");
            }

            return results;
        }

//// ncrunch: rdi default
    }
}