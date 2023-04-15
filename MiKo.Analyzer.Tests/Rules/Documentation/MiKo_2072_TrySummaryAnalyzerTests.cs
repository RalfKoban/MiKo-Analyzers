using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2072_TrySummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public bool TryDoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public bool TryDoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_starts_documentation_with_see_XML() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// <see cref=""TestMe""/>.
    /// </summary>
    public bool TryDoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_Try_method_([Values("Try", "Tries")] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @" something.
    /// </summary>
    public bool TryDoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_async_Try_method_([Values("try", "tries")] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Asynchronously " + phrase + @" something.
    /// </summary>
    public bool TryDoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_non_Try_method_([Values("Try", "Tries")] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @" something.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_([Values("Try to", "Tries to")] string startPhrase)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// " + startPhrase + @" do something.
    /// </summary>
    public void DoSomething() { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Attempts to do something.
    /// </summary>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Async()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Asynchronously tries to do something.
    /// </summary>
    public void DoSomething() { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Asynchronously attempts to do something.
    /// </summary>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_in_middle()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Tries to do something and also tries to do anything but nothing.
    /// </summary>
    public void DoSomething() { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Attempts to do something and also attempts to do anything but nothing.
    /// </summary>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2072_TrySummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2072_TrySummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2072_CodeFixProvider();
    }
}