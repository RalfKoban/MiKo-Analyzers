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
                                                                           "Action",
                                                                           "Actions",
                                                                           "Api",
                                                                           "API",
                                                                           "Apis",
                                                                           "APIs",
                                                                           "Applications",
                                                                           "ApplicationService",
                                                                           "ApplicationServices",
                                                                           "Attributes",
                                                                           "Base",
                                                                           "Builder",
                                                                           "Builders",
                                                                           "Class",
                                                                           "Classes",
                                                                           "Client",
                                                                           "Clients",
                                                                           "Compare",
                                                                           "Comparer",
                                                                           "Comparers",
                                                                           "Constants",
                                                                           "Contract",
                                                                           "Contracts",
                                                                           "Controller",
                                                                           "Controllers",
                                                                           "Data",
                                                                           "Database",
                                                                           "Databases",
                                                                           "Dependencies",
                                                                           "Dependency",
                                                                           "DependencyInjection",
                                                                           "Domain",
                                                                           "Domains",
                                                                           "Dto",
                                                                           "DTO",
                                                                           "Dtos",
                                                                           "DTOs",
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
                                                                           "Factories",
                                                                           "Filters",
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
                                                                           "Intf",
                                                                           "Intfc",
                                                                           "Intfcs",
                                                                           "Intfs",
                                                                           "Intrfc",
                                                                           "Intrfcs",
                                                                           "Itf",
                                                                           "Itfs",
                                                                           "Microservice",
                                                                           "MicroService",
                                                                           "Microservices",
                                                                           "MicroServices",
                                                                           "Middleware",
                                                                           "Mocks",
                                                                           "Module",
                                                                           "Modules",
                                                                           "MVC",
                                                                           "MVVM",
                                                                           "Observer",
                                                                           "Observers",
                                                                           "Pipelines",
                                                                           "Platform",
                                                                           "Provider",
                                                                           "Providers",
                                                                           "Proxies",
                                                                           "Proxy",
                                                                           "Queries",
                                                                           "Record",
                                                                           "Repositories",
                                                                           "Resources",
                                                                           "Service",
                                                                           "ServiceProxies",
                                                                           "ServiceProxy",
                                                                           "Services",
                                                                           "State",
                                                                           "Statements",
                                                                           "States",
                                                                           "Struct",
                                                                           "Structs",
                                                                           "Transaction",
                                                                           "Transactions",
                                                                           "Type",
                                                                           "Types",
                                                                           "UiOperation",
                                                                           "UIOperation",
                                                                           "UiOperations",
                                                                           "UIOperations",
                                                                           "ValueObject",
                                                                           "ValueObjects",
                                                                           "Wrapper",
                                                                           "Wrappers");

        [TestCase("MiKoSolutions")]
        [TestCase("MiKoSolutions.Infrastructure")]
        [TestCase("MiKoSolutions.Core")]
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
            var results = new HashSet<string>((3 * names.Length) + 1)
                              {
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