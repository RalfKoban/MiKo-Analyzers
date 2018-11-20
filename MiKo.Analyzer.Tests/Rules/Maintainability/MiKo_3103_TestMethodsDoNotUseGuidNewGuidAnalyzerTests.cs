using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzerTests : CodeFixVerifier
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

        [Test]
        public void No_issue_is_reported_for_non_test_method() => No_issue_is_reported_for(@"
public class TestMe
{
   public void DoSomething()
   {
       var x = Guid.NewGuid();
   }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_a_test_method(
                                                        [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                        [ValueSource(nameof(Tests))] string testAttribute)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
   public void DoSomething()
   {
       var x = Guid.NewGuid();
   }
}
");

        [Test]
        public void An_issue_is_reported_for_a_non_test_method_inside_a_test([ValueSource(nameof(TestFixtures))] string testClassAttribute) => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
   public void DoSomething()
   {
       var x = Guid.NewGuid();
   }
}
");

        protected override string GetDiagnosticId() => MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer();
    }
}