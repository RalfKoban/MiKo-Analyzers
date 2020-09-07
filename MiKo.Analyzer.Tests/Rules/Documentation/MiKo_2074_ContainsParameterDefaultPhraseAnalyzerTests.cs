using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2074_ContainsParameterDefaultPhraseAnalyzerTests : CodeFixVerifier
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
    /// <param name=""i"">
    /// Something to find.
    /// </param>
    public bool " + methodName + @"(int i)
    {
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_empty_parameter_on_method_(
                                                                [Values("Contains", "ContainsKey")] string methodName,
                                                                [Values(@"<param name=""i"" />", @"<param name=""i""></param>", @"<param name=""i"">     </param>")] string parameter)
            => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// " + parameter + @"
    public bool " + methodName + @"(int i)
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
    /// <param name=""i"">
    /// Something to find.
    /// </param>
    public void DoSomething(int i)
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_documented_method_(
                                                                    [Values("Contains", "ContainsKey")] string methodName,
                                                                    [Values("Something to seek.", "Something to locate.")] string comment)
            => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// " + comment + @"
    /// </param>
    public bool " + methodName + @"(int i)
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_simple_text()
        {
            const string OriginalText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// The item.
    /// </param>
    public bool Contains(int i)
    {
    }
}
";

            const string FixedText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// The item to seek.
    /// </param>
    public bool Contains(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_text_with_seeCref_and_ending_dot()
        {
            const string OriginalText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// The <see cref=""int""/>.
    /// </param>
    public bool Contains(int i)
    {
    }
}
";

            const string FixedText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// The <see cref=""int""/> to seek.
    /// </param>
    public bool Contains(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_text_with_seeCref_without_ending_dot()
        {
            const string OriginalText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// The <see cref=""int""/>
    /// </param>
    public bool Contains(int i)
    {
    }
}
";

            const string FixedText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// The <see cref=""int""/> to seek.
    /// </param>
    public bool Contains(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_text_with_seeCref_on_same_line_without_ending_dot()
        {
            const string OriginalText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">The <see cref=""int""/></param>
    public bool Contains(int i)
    {
    }
}
";

            const string FixedText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">The <see cref=""int""/> to seek.</param>
    public bool Contains(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        protected override string GetDiagnosticId() => MiKo_2074_ContainsParameterDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2074_ContainsParameterDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2074_CodeFixProvider();
    }
}