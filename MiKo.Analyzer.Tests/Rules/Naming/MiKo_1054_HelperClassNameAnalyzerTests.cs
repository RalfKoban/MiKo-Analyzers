using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1054_HelperClassNameAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongSuffixes =
                                                         {
                                                             "Helper",
                                                             "Helpers",
                                                             "Util",
                                                             "Utils",
                                                             "Utility",
                                                             "Utilities",
                                                         };

        private static readonly string[] WrongNames = CreateWrongNames(WrongSuffixes);
        private static readonly string[] CorrectNames = { "TestMe", "OnlineHelp", "SoftwareUtilization" };

        [Test]
        public void No_issue_is_reported_for_correctly_named_class_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
public class " + name + @"
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

        protected override string GetDiagnosticId() => MiKo_1054_HelperClassNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1054_HelperClassNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1054_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static string[] CreateWrongNames(string[] names)
        {
            var allNames = new HashSet<string>(names);

            foreach (var s in names)
            {
                var lower = s.ToLowerInvariant();
                var upper = s.ToUpperInvariant();

                foreach (var name in new[] { s, lower, upper })
                {
                    allNames.Add(name);
                    allNames.Add("Some" + name);
                    allNames.Add(name + "Stuff");
                    allNames.Add("Some" + name + "Stuff");
                }
            }

            return allNames.OrderBy(_ => _).ToArray();
        }
    }
}