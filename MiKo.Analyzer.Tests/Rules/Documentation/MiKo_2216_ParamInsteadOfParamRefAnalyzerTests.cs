using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2216_ParamInsteadOfParamRefAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
/// <summary>
/// Some description.
/// </summary>
public class TestMe
{
    public void DoSomething(int i)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_used_param_tag() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Some method description.
    /// </summary>
    /// <param name=""i"">The parameter</param>
    public void DoSomething(int i)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_used_paramref_tag() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Some method description for <paramref name=""i""/>.
    /// </summary>
    /// <param name=""i"">The parameter</param>
    /// <param name=""j"">The parameter, not same as <paramref name=""i""/>.</param>
    public void DoSomething(int i, int j)
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_used_param_tag_in_summary() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Some method description for <param name=""i""/>.
    /// </summary>
    /// <param name=""i"">The parameter</param>
    /// <param name=""j"">The parameter, not same as <paramref name=""i""/>.</param>
    public void DoSomething(int i, int j)
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_used_param_tag_in_param() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Some method description for <paramref name=""i""/>.
    /// </summary>
    /// <param name=""i"">The parameter</param>
    /// <param name=""j"">The parameter, not same as <param name=""i""/>.</param>
    public void DoSomething(int i, int j)
    { }
}
");

        [Test]
        public void Code_gets_fixed_for_param_tags_in_summary()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Some method description for <param name=""i""/> and <param name=""j""></param>.
    /// </summary>
    /// <param name=""i"">The parameter</param>
    /// <param name=""j"">The parameter, not same as <paramref name=""i""/>.</param>
    public void DoSomething(int i, int j)
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Some method description for <paramref name=""i""/> and <paramref name=""j""/>.
    /// </summary>
    /// <param name=""i"">The parameter</param>
    /// <param name=""j"">The parameter, not same as <paramref name=""i""/>.</param>
    public void DoSomething(int i, int j)
    { }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_param_tag_in_param()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Some method description.
    /// </summary>
    /// <param name=""i"">The parameter, not same as <param name=""j""/>.</param>
    /// <param name=""j"">The parameter, not same as <param name=""i""></param>.</param>
    public void DoSomething(int i, int j)
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Some method description.
    /// </summary>
    /// <param name=""i"">The parameter, not same as <paramref name=""j""/>.</param>
    /// <param name=""j"">The parameter, not same as <paramref name=""i""/>.</param>
    public void DoSomething(int i, int j)
    { }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2216_ParamInsteadOfParamRefAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2216_ParamInsteadOfParamRefAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2216_CodeFixProvider();
    }
}