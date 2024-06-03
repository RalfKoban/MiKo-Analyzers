using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
                                                                                    "Build",
                                                                                    "Children",
                                                                                    "ComponentModel",
                                                                                    "Composition",
                                                                                    "Converters",
                                                                                    "Core",
                                                                                    "CPlusPlus",
                                                                                    "CSharp",
                                                                                    "Data",
                                                                                    "Design",
                                                                                    "Documentation",
                                                                                    "Extensions",
                                                                                    "Framework",
                                                                                    "Generic",
                                                                                    "Infrastructure",
                                                                                    "Interop",
                                                                                    "IO",
                                                                                    "JavaScript",
                                                                                    "Lifetime",
                                                                                    "Linq",
                                                                                    "Maintainability",
                                                                                    "MyOwnNumber0815",
                                                                                    "Naming",
                                                                                    "Office",
                                                                                    "Performance",
                                                                                    "Perl",
                                                                                    "Resources",
                                                                                    "Runtime",
                                                                                    "Security",
                                                                                    "Serialization",
                                                                                    "ServiceModel",
                                                                                    "Shared",
                                                                                    "SomeTrivia",
                                                                                    "Support",
                                                                                    "System",
                                                                                    "Threading",
                                                                                    "TypeScript",
                                                                                    "UI",
                                                                                    "UserExperience",
                                                                                    "VisualBasic",
                                                                                    "Web",
                                                                                };

        private static readonly IEnumerable<string> WellKnownCompanyAndFrameworkNames = new[]
                                                                                            {
                                                                                                "JetBrains",
                                                                                                "Microsoft",
                                                                                                "MiKoSolutions",
                                                                                                "NDepend",
                                                                                                "PostSharp",
                                                                                                "Azure",
                                                                                                "Docker",
                                                                                            };

        private static readonly IEnumerable<string> Acronyms = new[]
                                                                   {
                                                                       "WYSIWYG",
                                                                   };

        [Test]
        public void No_issue_is_reported_for_acronym_namespace_name_([ValueSource(nameof(Acronyms))]string ns) => No_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_known_namespace_name_([ValueSource(nameof(WellKnownCompanyAndFrameworkNames))]string ns) => No_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_combined_known_namespace_name_([ValueSource(nameof(WellKnownCompanyAndFrameworkNames))]string ns) => No_issue_is_reported_for(@"
namespace Abc." + ns + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_proper_namespace_([ValueSource(nameof(AllowedNamespaceNames))]string ns) => No_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_combined_proper_namespace_([ValueSource(nameof(AllowedNamespaceNames))]string ns) => No_issue_is_reported_for(@"
namespace Abc." + ns + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_top_level_singular_namespace_([ValueSource(nameof(SingularNamespaceNames))] string ns) => No_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_combined_singular_namespace_([ValueSource(nameof(SingularNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace Abc." + ns + @"
{
}
");

        [Test]
        public void Model_namespace_gets_reported_as_Entities()
        {
            var betterName = MiKo_1400_NamespacesInPluralAnalyzer.FindBetterName("Model");

            Assert.That(betterName, Is.EqualTo("Entities"));
        }

        protected override string GetDiagnosticId() => MiKo_1400_NamespacesInPluralAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1400_NamespacesInPluralAnalyzer();
    }
}