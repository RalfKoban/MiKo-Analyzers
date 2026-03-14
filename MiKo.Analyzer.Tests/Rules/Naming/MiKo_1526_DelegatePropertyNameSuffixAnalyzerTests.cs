using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1526_DelegatePropertyNameSuffixAnalyzerTests : CodeFixVerifier
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

        [TestCase("string SomeName")]
        [TestCase("int SomeNumber")]
        public void No_issue_is_reported_for_non_delegate_property_(string property) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public " + property + @" { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_property_name_([ValueSource(nameof(DelegateTypes))] string type) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public " + type + @" SomeCallback { get; set; }
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_non_fitting_name_([ValueSource(nameof(DelegateTypes))] string type)
            => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public " + type + @" Something { get; set; }
}
");

        [TestCase("Action", "Callback")]
        [TestCase("Delegate", "Callback")]
        [TestCase("Func", "Callback")]
        [TestCase("SomeAction", "SomeCallback")]
        [TestCase("SomeDelegate", "SomeCallback")]
        [TestCase("SomeFunc", "SomeCallback")]
        [TestCase("Something", "SomethingCallback")]
        public void Code_gets_fixed(string originalName, string fixedName)
        {
            const string Template = """
                                    using System;

                                    public class TestMe
                                    {
                                        public Action ### { get; set; }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1526_DelegatePropertyNameSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1526_DelegatePropertyNameSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1526_CodeFixProvider();
    }
}