using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3112_TestAssertsUseIsEmptyInsteadOfHasCountZeroAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
public class TestMe
{
   public void DoSomething()
   {
   }
}
");

        [TestCase("Assert.That(42, Is.EqualTo(0815))")]
        [TestCase("Assert.That(42, Is.Not.EqualTo(0815))")]
        [TestCase("Assert.That(42, Is.Zero)")]
        [TestCase("Assert.That(42, Is.Not.Zero)")]
        [TestCase("Assert.That(42, Has.Count.EqualTo(0815))")]
        [TestCase("Assert.That(42, Has.Exactly(0815).Items)")]
        [TestCase("Assert.That(42, Is.Not.Negative.And.Not.EqualTo(0815)")]
        [TestCase("Assert.That(42, Is.Not.Negative.And.Not.Zero")]
        public void No_issue_is_reported_for_correct_usage_in_a_test_method_(string assertion) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            " + assertion + @";
        }
    }
}");

        [TestCase("Assert.That(Array.Empty<int>(), Has.Count.Zero)")]
        [TestCase("Assert.That(Array.Empty<int>(), Has.Not.Count.Zero)")]
        [TestCase("Assert.That(Array.Empty<int>(), Has.Exactly(0).Items")]
        public void An_issue_is_reported_for_incorrect_usage_in_a_test_method_(string assertion) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            " + assertion + @";
        }
    }
}");

        [TestCase("Has.Count.Zero", "Is.Empty")]
        [TestCase("Has.Not.Count.Zero", "Is.Not.Empty")]
        [TestCase("Has.Exactly(0).Items", "Is.Empty")]
        public void Code_gets_fixed_(string originalCode, string fixedCode)
        {
            const string Template = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMeTests
    {
        [Test]
        public void DoSomething()
        {
            Assert.That(Array.Empty<int>(), ###);
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        protected override string GetDiagnosticId() => MiKo_3112_TestAssertsUseIsEmptyInsteadOfHasCountZeroAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3112_TestAssertsUseIsEmptyInsteadOfHasCountZeroAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3112_CodeFixProvider();
    }
}