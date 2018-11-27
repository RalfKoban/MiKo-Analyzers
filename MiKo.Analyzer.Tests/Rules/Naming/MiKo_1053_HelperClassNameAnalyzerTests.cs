using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1053_HelperClassNameAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_name([ValueSource(nameof(WrongNames))]  string name) => An_issue_is_reported_for(@"
public class " + name + @"
{
}
");

        protected override string GetDiagnosticId() => MiKo_1053_HelperClassNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1053_HelperClassNameAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> WrongNames()
        {
            var names = new[]
                            {
                                "Helper",
                                "Helpers",
                                "Util",
                                "Utils",
                                "Utility",
                                "Utilities",
                            };

            var allNames = new HashSet<string>(names);
            foreach (var _ in names)
            {
                var lower = _.ToLowerInvariant();
                var upper = _.ToUpperInvariant();

                foreach (var name in new[] { _, lower, upper })
                {
                    allNames.Add(name);
                    allNames.Add("Some" + name);
                    allNames.Add(name + "Stuff");
                    allNames.Add("Some" + name + "Stuff");
                }
            }

            return allNames;
        }
    }
}