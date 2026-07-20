using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3111_testAssertsUseSpecificConstraintInsteadOfEqualToAnalyzerTests : CodeFixVerifier
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
        [TestCase("Assert.That(42, Is.Not.Negative.And.Not.EqualTo(0815)")]
        [TestCase("Assert.That(42, Has.Count.EqualTo(0815))")]
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

        [TestCase("Assert.That(42, Is.EqualTo(0))")]
        [TestCase("Assert.That(42, Is.Not.EqualTo(0))")]
        [TestCase("Assert.That(42, Is.Not.Negative.And.Not.EqualTo(0))")]
        [TestCase("Assert.That(42, Has.Count.EqualTo(0))")]
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

        // Zero
        [TestCase("Is.EqualTo(0)", "Is.Zero")]
        [TestCase("Is.Not.EqualTo(0)", "Is.Not.Zero")]
        [TestCase("Is.Not.Negative.And.Not.EqualTo(0)", "Is.Not.Negative.And.Not.Zero")]
        [TestCase("Has.Count.EqualTo(0)", "Is.Empty")]
        [TestCase("Is.LessThan(0)", "Is.Negative")]
        [TestCase("Is.Not.LessThan(0)", "Is.Not.Negative")]
        [TestCase("Is.GreaterThan(0)", "Is.Positive")]
        [TestCase("Is.Not.GreaterThan(0)", "Is.Not.Positive")]

        // NaN
        [TestCase("Is.EqualTo(double.NaN)", "Is.NaN")]
        [TestCase("Is.Not.EqualTo(double.NaN)", "Is.Not.NaN")]
        [TestCase("Is.EqualTo(Double.NaN)", "Is.NaN")]
        [TestCase("Is.Not.EqualTo(Double.NaN)", "Is.Not.NaN")]
        [TestCase("Is.EqualTo(float.NaN)", "Is.NaN")]
        [TestCase("Is.Not.EqualTo(float.NaN)", "Is.Not.NaN")]
        [TestCase("Is.EqualTo(Single.NaN)", "Is.NaN")]
        [TestCase("Is.Not.EqualTo(Single.NaN)", "Is.Not.NaN")]

        // booleans
        [TestCase("Is.EqualTo(true)", "Is.True")]
        [TestCase("Is.Not.EqualTo(true)", "Is.Not.True")]
        [TestCase("Is.EqualTo(false)", "Is.False")]
        [TestCase("Is.Not.EqualTo(false)", "Is.Not.False")]

        // null
        [TestCase("Is.EqualTo(null)", "Is.Null")]
        [TestCase("Is.Not.EqualTo(null)", "Is.Not.Null")]

        // strings
        [TestCase("""Is.EqualTo("")""", "Is.Empty")]
        [TestCase("""Is.Not.EqualTo("")""", "Is.Not.Empty")]
        [TestCase("Is.EqualTo(string.Empty)", "Is.Empty")]
        [TestCase("Is.Not.EqualTo(string.Empty)", "Is.Not.Empty")]
        [TestCase("Is.EqualTo(String.Empty)", "Is.Empty")]
        [TestCase("Is.Not.EqualTo(String.Empty)", "Is.Not.Empty")]
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
            Assert.That(42, ###);
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode), allowNewCompilerDiagnostics: true); // CS8019 unused 'using' directive
        }

        protected override string GetDiagnosticId() => MiKo_3111_TestAssertsUseSpecificConstraintInsteadOfEqualToAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3111_TestAssertsUseSpecificConstraintInsteadOfEqualToAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3111_CodeFixProvider();
    }
}