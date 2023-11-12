using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1059_ImplClassNameAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongSuffixes =
                                                         {
                                                             "Impl",
                                                             "Implementation",
                                                         };

        private static readonly string[] WrongNames = CreateWrongNames(WrongSuffixes);

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

        [Test]
        public void Code_gets_fixed_for_wrong_name_([ValueSource(nameof(WrongSuffixes))] string wrongSuffix)
        {
            var originalCode = @"
public class TestMe" + wrongSuffix + @"
{
}
";

            const string FixedCode = @"
public class TestMe
{
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1059_ImplClassNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1059_ImplClassNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1059_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static string[] CreateWrongNames(string[] names)
        {
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