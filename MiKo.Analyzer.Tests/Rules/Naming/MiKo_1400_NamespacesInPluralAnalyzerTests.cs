using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1400_NamespacesInPluralAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_proper_namespace([ValueSource(nameof(AllowedNamespaceNames))]string ns) => No_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_singular_namespace([ValueSource(nameof(SingularNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        protected override string GetDiagnosticId() => MiKo_1400_NamespacesInPluralAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1400_NamespacesInPluralAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> SingularNamespaceNames() => new HashSet<string>
                                                                            {
                                                                                "Converter",
                                                                                "Test",
                                                                            };

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> AllowedNamespaceNames() => new HashSet<string>
                                                                            {
                                                                                "Activities",
                                                                                "ComponentModel",
                                                                                "Converters",
                                                                                "Documentation",
                                                                                "Extensions",
                                                                                "Framework",
                                                                                "Generic",
                                                                                "IO",
                                                                                "Infrastructure",
                                                                                "Lifetime",
                                                                                "Linq",
                                                                                "Maintainability",
                                                                                "MiKoSolutions",
                                                                                "Naming",
                                                                                "Resources",
                                                                                "Runtime",
                                                                                "Security",
                                                                                "ServiceModel",
                                                                                "System",
                                                                                "Threading",
                                                                                "UserExperience",
                                                                            };
    }
}