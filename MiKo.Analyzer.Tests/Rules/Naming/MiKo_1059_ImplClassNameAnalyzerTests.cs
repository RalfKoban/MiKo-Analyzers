using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1059_ImplClassNameAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongNames = CreateWrongNames();

        [Test]
        public void No_issue_is_reported_for_correctly_named_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
public class " + name + @"
{
}
");

        protected override string GetDiagnosticId() => MiKo_1059_ImplClassNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1059_ImplClassNameAnalyzer();

        [ExcludeFromCodeCoverage]
        private static string[] CreateWrongNames()
        {
            var names = new[]
                            {
                                "Impl",
                                "Implementation",
                            };

            var allNames = new HashSet<string>(names);

            foreach (var n in names)
            {
                var lower = n.ToLowerInvariant();
                var upper = n.ToUpperInvariant();

                foreach (var name in new[] { n, lower, upper })
                {
                    allNames.Add(name);
                    allNames.Add("Some" + name);
                }
            }

            return allNames.OrderBy(_ => _).ToArray();
        }
    }
}