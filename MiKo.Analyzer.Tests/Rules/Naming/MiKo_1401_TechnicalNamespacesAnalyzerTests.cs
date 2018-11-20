using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1401_TechnicalNamespacesAnalyzerTests : CodeFixVerifier
    {
        [TestCase("MiKoSolutions")]
        public void No_issue_is_reported_for_non_technical_namespace(string ns) => No_issue_is_reported_for(ns + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_technical_namespace([ValueSource(nameof(ForbiddenNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_namespace_that_starts_with_technical_sub_namespace([ValueSource(nameof(ForbiddenNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace " + ns + @".ABCD.EFG
{
}
");

        [Test]
        public void An_issue_is_reported_for_namespace_that_ends_with_technical_sub_namespace([ValueSource(nameof(ForbiddenNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace ABCD.EFG." + ns + @"
{
}
");


        [Test]
        public void An_issue_is_reported_for_namespace_that_contains_technical_sub_namespace([ValueSource(nameof(ForbiddenNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace ABCD.EFG." + ns + @".HIJK
{
}
");

        protected override string GetDiagnosticId() => MiKo_1401_TechnicalNamespacesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1401_TechnicalNamespacesAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> ForbiddenNamespaceNames() => new HashSet<string>
                                                                            {
                                                                                "Base",
                                                                                "Class",
                                                                                "Classes",
                                                                                "Command",
                                                                                "Commands",
                                                                                "Enum",
                                                                                "Enums",
                                                                                "Exception",
                                                                                "Exceptions",
                                                                                "Helper",
                                                                                "Implementation",
                                                                                "Implementations",
                                                                                "Interface",
                                                                                "Interfaces",
                                                                                "Model",
                                                                                "Models",
                                                                                "Proxies",
                                                                                "Proxy",
                                                                                "ServiceProxies",
                                                                                "ServiceProxy",
                                                                                "Struct",
                                                                                "Structs",
                                                                                "View",
                                                                                "Views",
                                                                                "ViewModel",
                                                                                "ViewModels",
                                                                            };
    }
}