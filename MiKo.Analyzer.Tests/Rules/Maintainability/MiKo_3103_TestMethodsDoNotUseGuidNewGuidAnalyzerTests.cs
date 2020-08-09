using Microsoft.CodeAnalysis.CodeFixes;
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
        public void No_issue_is_reported_for_test_method_using_(
                                                        [ValueSource(nameof(Tests))] string test,
                                                        [Values("new Guid()", @"Guid.Parse(""62AD86A4-3F05-403E-B53F-B2B21A62D6C7"")")] string call)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
   public void DoSomething()
   {
       var x = " + call + @";
   }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_method_([ValueSource(nameof(Tests))] string test) => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
   public void DoSomething()
   {
       var x = Guid.NewGuid();
   }
}
");

        [Test]
        public void An_issue_is_reported_for_a_strangely_formatted_test_method_([ValueSource(nameof(Tests))] string test) => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
   public void DoSomething()
   {
       var x = Guid.
                NewGuid
                    ();
   }
}
");

        [Test]
        public void An_issue_is_reported_for_a_non_test_method_inside_a_test_([ValueSource(nameof(TestFixtures))] string testFixture) => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
   public void DoSomething()
   {
       var x = Guid.NewGuid();
   }
}
");

        [Test]
        public void An_issue_is_reported_for_a_strangely_formatted_non_test_method_inside_a_test_([ValueSource(nameof(TestFixtures))] string testFixture) => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
   public void DoSomething()
   {
       var x = Guid
                .NewGuid
                        (
                        );
   }
}
");

        [Test, Ignore("Cannot be executed for the moment as a new GUID is generated each time.")]
        public void Code_gets_fixed()
        {
            const string Template = @"using NUnit.Framework; public class TestMe { [Test] public void Test() { var x = ###; } }";

            VerifyCSharpFix(Template.Replace("###", "Guid.NewGuid()"), Template.Replace("###", @"Guid.Parse("""")"));
        }

        protected override string GetDiagnosticId() => MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3103_CodeFixProvider();
    }
}