using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1400_NamespacesInPluralAnalyzerTests : CodeFixVerifier
    {
        private static readonly IEnumerable<string> SingularNamespaceNames = new[]
                                                                                 {
                                                                                     "Converter",
                                                                                     "Test",
                                                                                 };

        private static readonly IEnumerable<string> AllowedNamespaceNames = new[]
                                                                                {
                                                                                    "Activities",
                                                                                    "ComponentModel",
                                                                                    "Composition",
                                                                                    "Converters",
                                                                                    "Data",
                                                                                    "Documentation",
                                                                                    "Extensions",
                                                                                    "Framework",
                                                                                    "Generic",
                                                                                    "IO",
                                                                                    "Infrastructure",
                                                                                    "Interop",
                                                                                    "Lifetime",
                                                                                    "Linq",
                                                                                    "Maintainability",
                                                                                    "Naming",
                                                                                    "Performance",
                                                                                    "Resources",
                                                                                    "Runtime",
                                                                                    "Security",
                                                                                    "ServiceModel",
                                                                                    "Serialization",
                                                                                    "System",
                                                                                    "Threading",
                                                                                    "UserExperience",
                                                                                    "Children",
                                                                                };

        private static readonly IEnumerable<string> WellKnownCompanyAndFrameworkNames = new[]
                                                                                            {
                                                                                                "JetBrains",
                                                                                                "MiKoSolutions",
                                                                                                "Microsoft",
                                                                                                "PostSharp",
                                                                                                "NDepend",
                                                                                            };
        [Test]
        public void No_issue_is_reported_for_known_namespace_name([ValueSource(nameof(WellKnownCompanyAndFrameworkNames))]string ns) => No_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_combined_known_namespace_name([ValueSource(nameof(WellKnownCompanyAndFrameworkNames))]string ns) => No_issue_is_reported_for(@"
namespace Abc." + ns + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_proper_namespace([ValueSource(nameof(AllowedNamespaceNames))]string ns) => No_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_combined_proper_namespace([ValueSource(nameof(AllowedNamespaceNames))]string ns) => No_issue_is_reported_for(@"
namespace Abc." + ns + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_singular_namespace([ValueSource(nameof(SingularNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_combined_singular_namespace([ValueSource(nameof(SingularNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace Abc." + ns + @"
{
}
");

        protected override string GetDiagnosticId() => MiKo_1400_NamespacesInPluralAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1400_NamespacesInPluralAnalyzer();
    }
}