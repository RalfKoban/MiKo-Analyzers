using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2013_EnumSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_class_without_documentation() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_documentation() => No_issue_is_reported_for(@"
/// <summary>
/// Some documentation.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_enum_without_documentation() => No_issue_is_reported_for(@"
public enum TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_enum_with_correct_phrase() => No_issue_is_reported_for(@"
/// <summary>
/// Defines values that specify something.
/// </summary>
public enum TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_enum_with_correct_phrase_in_para_tag() => No_issue_is_reported_for(@"
/// <summary>
/// <para>
/// Defines values that specify something.
/// </para>
/// </summary>
public enum TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_enum_with_wrong_phrase() => An_issue_is_reported_for(@"
/// <summary>
/// Some documentation.
/// </summary>
public enum TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_enum_with_wrong_phrase_in_para_tag() => An_issue_is_reported_for(@"
/// <summary>
/// <para>
/// Defines something.
/// </para>
/// </summary>
public enum TestMe
{
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
/// <summary>
/// Something.
/// </summary>
public enum TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Defines values that specify Something.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_single_line()
        {
            const string OriginalCode = @"
/// <summary>Something to do.</summary>
public enum TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Defines values that specify Something to do.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_multiline()
        {
            const string OriginalCode = @"
/// <summary>
/// Something
/// to do.
/// </summary>
public enum TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Defines values that specify Something
/// to do.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_seecref_multiline()
        {
            const string OriginalCode = @"
/// <summary>
/// <see cref=""TestMe""/>
/// Something
/// to do.
/// </summary>
public enum TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Defines values that specify <see cref=""TestMe""/>
/// Something
/// to do.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_seecref_on_single_line()
        {
            const string OriginalCode = @"
/// <summary><see cref=""TestMe""/> Something to do.</summary>
public enum TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Defines values that specify <see cref=""TestMe""/> Something to do.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_almost_correct_documentation_([Values("Specifies", "Defines", "Indicates")] string firstWord)
        {
            var originalCode = @"
/// <summary>
/// " + firstWord + @" something to do.
/// </summary>
public enum TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Defines values that specify something to do.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2013_EnumSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2013_EnumSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2013_CodeFixProvider();
    }
}