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
    public sealed class MiKo_1052_DelegateLocalVariableNameSuffixAnalyzerTests : CodeFixVerifier
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
        [TestCase("object transaction")]
        public void No_issue_is_reported_for_non_delegate_parameter_(string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(" + name + @")
    {
    }
}
");

        [TestCase("string s")]
        [TestCase("int i")]
        [TestCase("object transaction")]
        public void No_issue_is_reported_for_non_delegate_variable_(string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        " + name + @";
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correct_variable_name_(
                                                                [ValueSource(nameof(DelegateTypes))] string type,
                                                                [ValueSource(nameof(CorrectDelegateNames))] string name)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public " + type + @" DoSomething()
    {
        " + type + " " + name + @";
        return " + name + @"; 
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_variable_with_non_fitting_name_(
                                                                         [ValueSource(nameof(DelegateTypes))] string type,
                                                                         [ValueSource(nameof(WrongDelegateNames))] string name)
            => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public " + type + @" DoSomething()
    {
        " + type + " " + name + @";
        return " + name + @"; 
    }
}
");

        [Test]
        public void Code_gets_fixed() => VerifyCSharpFix(
                                                     "using System; public class TestMe { public void DoSomething() { Action action = null; } }",
                                                     "using System; public class TestMe { public void DoSomething() { Action callback = null; } }");

        protected override string GetDiagnosticId() => MiKo_1052_DelegateLocalVariableNameSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1052_DelegateLocalVariableNameSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1052_CodeFixProvider();

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