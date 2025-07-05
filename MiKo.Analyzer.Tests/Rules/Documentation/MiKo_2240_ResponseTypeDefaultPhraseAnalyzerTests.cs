using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2240_ResponseTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public object DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property() => No_issue_is_reported_for(@"
public class TestMe
{
    public object DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_describes_response_as_([Values("A whatever", "An whatever", "The whatever")] string comment) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <response code=""1234"">
    /// " + comment + @"
    /// </response>
    public Task DoSomething(object o) => throw new NotSupportedException();
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_describes_response_as_([Values("Return whatever", "Returns whatever", "Is returned when the whatever", "Is returned if whatever")] string comment)
            => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <response code=""1234"">
    /// " + comment + @"
    /// </response>
    public object DoSomething(object o) => null;
}
");

        [TestCase("Return whatever", "Whatever")]
        [TestCase("Returns whatever", "Whatever")]
        [TestCase("Returned whatever", "Whatever")]
        [TestCase("Returned when whatever", "Whatever")]
        [TestCase("Returned when the whatever", "The whatever")]
        [TestCase("Returned if whatever", "Whatever")]
        [TestCase("Returned if the whatever", "The whatever")]
        [TestCase("Is returned when whatever", "Whatever")]
        [TestCase("Is returned when the whatever", "The whatever")]
        [TestCase("Is returned if whatever", "Whatever")]
        [TestCase("Is returned if the whatever", "The whatever")]
        public void Code_gets_fixed_for_method_that_describes_response_as_(string originalComment, string fixedComment)
        {
            const string Template = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <response code=""1234"">
    /// ###.
    /// </response>
    public object DoSomething(object o) => null;
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        protected override string GetDiagnosticId() => MiKo_2240_ResponseTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2240_ResponseTypeDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2240_CodeFixProvider();
    }
}