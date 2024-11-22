using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2233_EmptyXmlTagIsOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_start_and_end_tags_on_single_lines() => No_issue_is_reported_for(@"
/// <summary>
/// Does something.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_empty_XML_tag_on_single_line() => No_issue_is_reported_for(@"
/// <inheritdoc />
public sealed class TestMe
{
}
");

        [Test]
        public void Code_gets_fixed_for_documentation_with_LessThan_token_of_empty_XML_tag_on_different_line()
        {
            const string OriginalCode = @"
/// <
/// inheritdoc />
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <inheritdoc/>
public sealed class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_SlashGreaterThan_token_of_empty_XML_tag_on_different_line()
        {
            const string OriginalCode = @"
/// <inheritdoc
/// />
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <inheritdoc/>
public sealed class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_attribute_of_empty_XML_tag_on_different_line()
        {
            const string OriginalCode = @"
/// <inheritdoc
/// cref=""TestMe"" />
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <inheritdoc cref=""TestMe""/>
public sealed class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_parameter_token_of_empty_XML_tag_on_different_line()
        {
            const string OriginalCode = @"
public sealed class TestMe
{
    /// <summary>
    /// Does something with <paramref
    /// name=""o""/>.
    /// </summary>
    public void DoSomething(object o)
    { }
}
";

            const string FixedCode = @"
public sealed class TestMe
{
    /// <summary>
    /// Does something with <paramref name=""o""/>.
    /// </summary>
    public void DoSomething(object o)
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_attribute_of_empty_XML_tag_spanning_multiple_lines()
        {
            const string OriginalCode = @"
public sealed class TestMe
{
    public void DoSomething(object o, bool b) { }

    /// <see
    /// cref=""DoSomething(object,
    /// bool)"" />
    public void DoSomething(object o) { }
}
";

            const string FixedCode = @"
public sealed class TestMe
{
    public void DoSomething(object o, bool b) { }

    /// <see cref=""DoSomething(object,bool)""/>
    public void DoSomething(object o) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_contents_of_attribute_of_empty_XML_tag_spanning_multiple_lines()
        {
            const string OriginalCode = @"
public sealed class TestMe
{
    public void DoSomething(object o, bool b) { }

    /// <see cref=""
    ///    DoSomething(
    ///    object,
    ///    bool)"" />
    public void DoSomething(object o) { }
}
";

            const string FixedCode = @"
public sealed class TestMe
{
    public void DoSomething(object o, bool b) { }

    /// <see cref=""DoSomething(object,bool)""/>
    public void DoSomething(object o) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_LessThan_token_of_start_XML_tag_on_different_line()
        {
            const string OriginalCode = @"
/// <
/// summary>
/// Does something.
/// </summary>
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Does something.
/// </summary>
public sealed class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_GreaterThan_token_of_start_XML_tag_on_different_line()
        {
            const string OriginalCode = @"
/// <summary
/// >
/// Does something.
/// </summary>
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Does something.
/// </summary>
public sealed class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_LessThanSlash_token_of_end_XML_tag_on_different_line()
        {
            const string OriginalCode = @"
/// <summary>
/// Does something.
/// </
/// summary>
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Does something.
/// </summary>
public sealed class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_GreaterThan_token_of_end_XML_tag_on_different_line()
        {
            const string OriginalCode = @"
/// <summary>
/// Does something.
/// </summary
/// >
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Does something.
/// </summary>
public sealed class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2233_EmptyXmlTagIsOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2233_EmptyXmlTagIsOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2233_CodeFixProvider();
    }
}