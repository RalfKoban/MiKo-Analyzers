using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1053_DelegateFieldNameSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] DelegateTypes =
                                                         [
                                                             "Action",
                                                             "Action<int>",
                                                             "Action<int, string>",
                                                             "Func<bool>",
                                                             "Func<bool, bool>",
                                                             "Delegate",
                                                         ];

        private static readonly string[] CorrectDelegateNames =
                                                                [
                                                                    "callback",
                                                                    "map",
                                                                    "filter",
                                                                    "predicate",
                                                                ];

        private static readonly string[] WrongDelegateNames = ["@delegate", "action", "func"];

        [TestCase("string s")]
        [TestCase("int i")]
        [TestCase("IDisposable disposable")]
        public void No_issue_is_reported_for_non_delegate_field_(string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private " + name + @";
}
");

        [Test]
        public void No_issue_is_reported_for_record_field() => No_issue_is_reported_for(@"
using System;

public record DTO
{
}

public class TestMe
{
    private DTO dto;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correct_field_name_(
                                                             [ValueSource(nameof(DelegateTypes))] string type,
                                                             [ValueSource(nameof(CorrectDelegateNames))] string name)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private " + type + " " + name + @";
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_field_with_non_fitting_name_(
                                                                      [ValueSource(nameof(DelegateTypes))] string type,
                                                                      [ValueSource(nameof(WrongDelegateNames))] string name)
            => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private " + type + " " + name + @";
}
");

        [Test]
        public void Code_gets_fixed_on_field_([ValueSource(nameof(WrongDelegateNames))] string name) => VerifyCSharpFix(
                                                                                                                    "using System; class TestMe { Action " + name + "; }",
                                                                                                                    "using System; class TestMe { Action callback; }");

        protected override string GetDiagnosticId() => MiKo_1053_DelegateFieldNameSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1053_DelegateFieldNameSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1053_CodeFixProvider();
    }
}