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
    public sealed class MiKo_1051_DelegateParameterNameSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] DelegateTypes =
                                                         {
                                                             "Action",
                                                             "Action<int>",
                                                             "Action<int, string>",
                                                             "Func<bool>",
                                                             "Func<bool, bool>",
                                                             "Delegate",
                                                         };

        private static readonly string[] CorrectDelegateNames =
                                                                {
                                                                    "callback",
                                                                    "map",
                                                                    "filter",
                                                                    "predicate",
                                                                };

        private static readonly string[] WrongDelegateNames = CreateWrongDelegateNames();

        [TestCase("string s")]
        [TestCase("int i")]
        public void No_issue_is_reported_for_non_delegate_parameter_(string parameter) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(" + parameter + @")
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correct_parameter_name_(
                                                                 [ValueSource(nameof(DelegateTypes))] string type,
                                                                 [ValueSource(nameof(CorrectDelegateNames))] string name)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(" + type + " " + name + @")
    {
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_parameter_with_non_fitting_name_(
                                                                          [ValueSource(nameof(DelegateTypes))] string type,
                                                                          [ValueSource(nameof(WrongDelegateNames))] string name)
            => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(" + type + " " + name + @")
    {
    }
}
");

        [Test]
        public void Code_gets_fixed() => VerifyCSharpFix(
                                                         "using System; public class TestMe { public void DoSomething(Action action) { } }",
                                                         "using System; public class TestMe { public void DoSomething(Action callback) { } }");

        protected override string GetDiagnosticId() => MiKo_1051_DelegateParameterNameSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1051_DelegateParameterNameSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1051_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static string[] CreateWrongDelegateNames()
        {
            var names = new[] { "@delegate", "action", "func" };

            var allNames = new HashSet<string>(names);

            foreach (var name in names)
            {
                allNames.Add(name.ToLowerInvariant());
                allNames.Add(name.ToUpperInvariant());
            }

            return allNames.OrderBy(_ => _).ToArray();
        }
    }
}