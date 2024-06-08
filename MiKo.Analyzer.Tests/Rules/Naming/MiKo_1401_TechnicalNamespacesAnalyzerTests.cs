using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1401_TechnicalNamespacesAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ForbiddenNamespaceNames =
                                                                   [
                                                                       "API",
                                                                       "APIs",
                                                                       "Action",
                                                                       "Actions",
                                                                       "Api",
                                                                       "Apis",
                                                                       "Attributes",
                                                                       "Base",
                                                                       "Class",
                                                                       "Classes",
                                                                       "Compare",
                                                                       "Comparer",
                                                                       "Comparers",
                                                                       "Controller",
                                                                       "Controllers",
                                                                       "DTO",
                                                                       "DTOs",
                                                                       "Dto",
                                                                       "Dtos",
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
                                                                       "Func",
                                                                       "Funcs",
                                                                       "Function",
                                                                       "Functions",
                                                                       "Imp",
                                                                       "Impl",
                                                                       "Impls",
                                                                       "Implementation",
                                                                       "Implementations",
                                                                       "Indexer",
                                                                       "Indexers",
                                                                       "Interface",
                                                                       "Interfaces",
                                                                       "Itf",
                                                                       "Itfs",
                                                                       "Module",
                                                                       "Modules",
                                                                       "Observer",
                                                                       "Observers",
                                                                       "Platform",
                                                                       "Provider",
                                                                       "Providers",
                                                                       "Proxies",
                                                                       "Proxy",
                                                                       "Record",
                                                                       "ServiceProxies",
                                                                       "ServiceProxy",
                                                                       "State",
                                                                       "States",
                                                                       "Struct",
                                                                       "Structs",
                                                                       "Type",
                                                                       "Types"
                                                                   ];

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
    }
}