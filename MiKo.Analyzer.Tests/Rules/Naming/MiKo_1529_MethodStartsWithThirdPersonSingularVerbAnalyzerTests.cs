using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1529_MethodStartsWithThirdPersonSingularVerbAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_starting_with_infinite_verb() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [TestCase("Ends")]
        [TestCase("Starts")]
        [TestCase("Contains")]
        [TestCase("Extends")]
        [TestCase("Implements")]
        [TestCase("Is")]
        [TestCase("Matches")]
        [TestCase("Throws")]
        public void No_issue_is_reported_for_boolean_method_starting_with_(string verb) => No_issue_is_reported_for(@"
public class TestMe
{
    public bool " + verb + @"Something() { }
}
");

        [TestCase("As")]
        public void No_issue_is_reported_for_method_starting_with_(string verb) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + verb + @"Something() { }
}
");

        [TestCase("Creates")]
        [TestCase("Ends")]
        [TestCase("Starts")]
        [TestCase("Contains")]
        [TestCase("Extends")]
        [TestCase("Implements")]
        [TestCase("Is")]
        [TestCase("Matches")]
        [TestCase("Throws")]
        public void An_issue_is_reported_for_method_starting_with_3rd_person_singular_verb_(string verb) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + verb + @"Something() { }
}
");

        [TestCase("Creates", "Create")]
        public void Code_gets_fixed_for_method_with_3rd_person_singular_verb_(string originalName, string fixedName)
        {
            const string Template = """

                                    public class TestMe
                                    {
                                        public void ###Something() { }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1529_MethodStartsWithThirdPersonSingularVerbAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1529_MethodStartsWithThirdPersonSingularVerbAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1529_CodeFixProvider();
    }
}