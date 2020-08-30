using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public bool Contains()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_([Values("Contains", "ContainsKey")] string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public bool " + methodName + @"()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_async_method_([Values("Contains", "ContainsKey")] string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Asynchronously does something.
    /// </summary>
    public bool " + methodName + @"()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_documented_non_Contains_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_([Values("Contains", "ContainsKey")] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Determines whether something is there.
    /// </summary>
    public bool " + methodName + @"()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_documented_async_method_([Values("Contains", "ContainsKey")] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Asynchronously determines whether something is there.
    /// </summary>
    public bool " + methodName + @"()
    {
    }
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public bool Contains() => true;
}
";

            const string FixedText = @"
public class TestMe
{
    /// <summary>
    /// Determines whether something.
    /// </summary>
    public bool Contains() => true;
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_async_phrase()
        {
            const string OriginalText = @"
public class TestMe
{
    /// <summary>
    /// Asynchronously does something.
    /// </summary>
    public bool Contains() => true;
}
";

            const string FixedText = @"
public class TestMe
{
    /// <summary>
    /// Asynchronously determines whether something.
    /// </summary>
    public bool Contains() => true;
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_Determines_if_phrase()
        {
            const string OriginalText = @"
public class TestMe
{
    /// <summary>
    /// Determines if it is there.
    /// </summary>
    public bool Contains() => true;
}
";

            const string FixedText = @"
public class TestMe
{
    /// <summary>
    /// Determines whether it is there.
    /// </summary>
    public bool Contains() => true;
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        protected override string GetDiagnosticId() => MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2073_CodeFixProvider();
    }
}