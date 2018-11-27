using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1051_DelegateParameterNameSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("string s")]
        [TestCase("int i")]
        public void No_issue_is_reported_for_non_delegate_parameter(string parameter) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(" + parameter + @")
    {
    }
}
");
        [Test, Combinatorial]
        public void No_issue_is_reported_for_correct_parameter_name(
                                                                [ValueSource(nameof(DelegateTypes))] string type,
                                                                [ValueSource(nameof(CorrectDelegateNames))] string name)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(" + type + @" " + name + @")
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_with_non_fitting_name(
                                                                        [ValueSource(nameof(DelegateTypes))] string type,
                                                                        [ValueSource(nameof(WrongDelegateNames))] string name)
            => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(" + type + @" " + name + @")
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1051_DelegateParameterNameSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1051_DelegateParameterNameSuffixAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> DelegateTypes() => new[]
                                                                  {
                                                                      "Action",
                                                                      "Action<int>",
                                                                      "Action<int, string>",
                                                                      "Func<bool>",
                                                                      "Func<bool, bool>",
                                                                      "Delegate",
                                                                  };

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> WrongDelegateNames()
        {
            var names = new[] { "@delegate", "action", "func" };

            var allNames = new HashSet<string>(names);
            foreach (var _ in names)
            {
                allNames.Add(_.ToLowerInvariant());
                allNames.Add(_.ToUpperInvariant());
            }

            return allNames;
        }

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> CorrectDelegateNames() => new[]
                                                                         {
                                                                             "callback",
                                                                             "map",
                                                                             "filter",
                                                                             "predicate",
                                                                         };
    }
}