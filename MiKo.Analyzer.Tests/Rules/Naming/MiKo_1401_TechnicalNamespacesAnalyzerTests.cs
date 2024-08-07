using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1401_TechnicalNamespacesAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ForbiddenNamespaceNames = CreateNames(
                                                                           "API",
                                                                           "APIs",
                                                                           "Action",
                                                                           "Actions",
                                                                           "Api",
                                                                           "Apis",
                                                                           "Applications",
                                                                           "ApplicationService",
                                                                           "ApplicationServices",
                                                                           "Attributes",
                                                                           "Base",
                                                                           "Class",
                                                                           "Classes",
                                                                           "Client",
                                                                           "Clients",
                                                                           "Compare",
                                                                           "Comparer",
                                                                           "Comparers",
                                                                           "Constants",
                                                                           "Controller",
                                                                           "Controllers",
                                                                           "DTO",
                                                                           "DTOs",
                                                                           "Data",
                                                                           "Database",
                                                                           "Databases",
                                                                           "Dependencies",
                                                                           "Dependency",
                                                                           "DependencyInjection",
                                                                           "Domain",
                                                                           "Domains",
                                                                           "Dto",
                                                                           "Dtos",
                                                                           "Entities",
                                                                           "Enum",
                                                                           "Enumeration",
                                                                           "Enumerations",
                                                                           "Enums",
                                                                           "Error",
                                                                           "Errors",
                                                                           "EventArgument",
                                                                           "EventArguments",
                                                                           "Events",
                                                                           "Exception",
                                                                           "Exceptions",
                                                                           "Execution",
                                                                           "Executions",
                                                                           "Filters",
                                                                           "Factories",
                                                                           "Func",
                                                                           "Funcs",
                                                                           "Function",
                                                                           "Functions",
                                                                           "Handler",
                                                                           "Handlers",
                                                                           "HttpClient",
                                                                           "HttpClients",
                                                                           "Imp",
                                                                           "Impl",
                                                                           "Implementation",
                                                                           "Implementations",
                                                                           "Impls",
                                                                           "Indexer",
                                                                           "Indexers",
                                                                           "Interactions",
                                                                           "Interceptors",
                                                                           "Interface",
                                                                           "Interfaces",
                                                                           "Itf",
                                                                           "Itfs",
                                                                           "MVC",
                                                                           "MVVM",
                                                                           "Microservice",
                                                                           "MicroService",
                                                                           "Microservices",
                                                                           "MicroServices",
                                                                           "Middleware",
                                                                           "Mocks",
                                                                           "Module",
                                                                           "Modules",
                                                                           "Observer",
                                                                           "Observers",
                                                                           "Queries",
                                                                           "Pipelines",
                                                                           "Platform",
                                                                           "Provider",
                                                                           "Providers",
                                                                           "Proxies",
                                                                           "Proxy",
                                                                           "Record",
                                                                           "Repositories",
                                                                           "Resources",
                                                                           "ServiceProxies",
                                                                           "ServiceProxy",
                                                                           "Service",
                                                                           "Services",
                                                                           "State",
                                                                           "States",
                                                                           "Statements",
                                                                           "Struct",
                                                                           "Structs",
                                                                           "Type",
                                                                           "Types",
                                                                           "Transaction",
                                                                           "Transactions",
                                                                           "Wrapper",
                                                                           "Wrappers",
                                                                           "ValueObject",
                                                                           "ValueObjects");

        [TestCase("MiKoSolutions")]
        [TestCase("MiKoSolutions.Infrastructure")]
        public void No_issue_is_reported_for_proper_namespace_(string ns) => No_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_namespace_([ValueSource(nameof(ForbiddenNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_namespace_that_starts_with_wrong_sub_namespace_([ValueSource(nameof(ForbiddenNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace " + ns + @".ABCD.EFG
{
}
");

        [Test]
        public void An_issue_is_reported_for_namespace_that_ends_with_wrong_sub_namespace_([ValueSource(nameof(ForbiddenNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace ABCD.EFG." + ns + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_namespace_that_contains_wrong_sub_namespace_([ValueSource(nameof(ForbiddenNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace ABCD.EFG." + ns + @".HIJK
{
}
");

        protected override string GetDiagnosticId() => MiKo_1401_TechnicalNamespacesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1401_TechnicalNamespacesAnalyzer();

        private static string[] CreateNames(params string[] names)
        {
            var results = new HashSet<string>((3 * names.Length) + 2)
                              {
                                  "Core",
                                  "Shared",
                              };

            foreach (var name in names)
            {
                results.Add(name);
                results.Add("Core" + name);
                results.Add("Shared" + name);
            }

            return [.. results];
        }
    }
}