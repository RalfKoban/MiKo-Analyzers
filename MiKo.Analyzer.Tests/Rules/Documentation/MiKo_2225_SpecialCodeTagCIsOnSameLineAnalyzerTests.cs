using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2225_SpecialCodeTagCIsOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_on_documentation_with_no_code_tag() => No_issue_is_reported_for(@"
/// <summary>
/// Does something.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_code_tag_on_single_line() => No_issue_is_reported_for(@"
/// <summary>
/// Does something with <c>42</c>.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_code_tag_with_start_tag_on_different_line() => An_issue_is_reported_for(@"
/// <summary>
/// Does something with <c>
/// 42</c>.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_code_tag_with_end_tag_on_different_line() => An_issue_is_reported_for(@"
/// <summary>
/// Does something with <c>42
/// </c>.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Code_gets_fixed_for_documentation_with_code_tag_with_start_tag_on_different_line()
        {
            const string OriginalCode = @"
/// <summary>
/// Does something with <c>
/// 42</c>.
/// </summary>
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Does something with <c>42</c>.
/// </summary>
public sealed class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_code_tag_with_end_tag_on_different_line()
        {
            const string OriginalCode = @"
/// <summary>
/// Does something with <c>42
/// </c>.
/// </summary>
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Does something with <c>42</c>.
/// </summary>
public sealed class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2225_SpecialCodeTagCIsOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2225_SpecialCodeTagCIsOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2225_CodeFixProvider();
    }
}