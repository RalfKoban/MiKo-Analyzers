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
        [TestCase("MiKoSolutions")]
        public void No_issue_is_reported_for_proper_namespace([ValueSource(nameof(PluralNamespaceNames))]string ns) => No_issue_is_reported_for(ns + @"
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
                                                                                "Test",
                                                                                "Converter",
                                                                            };

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> PluralNamespaceNames() => new HashSet<string>
                                                                            {
                                                                                "MiKoSolutions",
                                                                                "Naming",
                                                                                "Activities",
                                                                                "Security",
                                                                                "Maintainablity",
                                                                                "Documentation",
                                                                                "Converters",
                                                                                "Extensions",
                                                                                "System",
                                                                            };
    }
}