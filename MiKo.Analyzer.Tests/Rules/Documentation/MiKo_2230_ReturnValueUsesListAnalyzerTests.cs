using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2230_ReturnValueUsesListAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_with_no_comment() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_returns() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary></summary>
    public int DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_problematic_text_in_summary() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// A value that indicates something being compared. The return value has these meanings: Value Meaning Less than zero Some text here. Zero Some other text here. Greater than zero Some even other text here.
    /// </summary>
    public int DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_problematic_text_in_remarks() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <remarks>
    /// A value that indicates something being compared. The return value has these meanings: Value Meaning Less than zero Some text here. Zero Some other text here. Greater than zero Some even other text here.
    /// </remarks>
    public int DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_text_list_in_returns() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <returns>
    /// A value that indicates something being compared.
    /// The return value has these meanings:
    /// <list type=""table"">
    /// <listheader><term>Value</term><description>Meaning</description></listheader>
    /// <item><term>Less than zero</term><description>Some text here.</description></item>
    /// <item><term>Zero</term><description>Some other text here.</description></item>
    /// <item><term>Greater than zero</term><description>Some even other text here.</description></item>
    /// </list>
    /// </returns>
    public int DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_text_list_in_value() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <value>
    /// A value that indicates something being compared.
    /// The return value has these meanings:
    /// <list type=""table"">
    /// <listheader><term>Value</term><description>Meaning</description></listheader>
    /// <item><term>Less than zero</term><description>Some text here.</description></item>
    /// <item><term>Zero</term><description>Some other text here.</description></item>
    /// <item><term>Greater than zero</term><description>Some even other text here.</description></item>
    /// </list>
    /// </value>
    public int Something { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_problematic_text_in_returns() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <returns>
    /// A value that indicates something being compared. The return value has these meanings: Value Meaning Less than zero Some text here. Zero Some other text here. Greater than zero Some even other text here.
    /// </returns>
    public int DoSomething()
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_problematic_text_in_value() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <value>
    /// A value that indicates something being compared. The return value has these meanings: Value Meaning Less than zero Some text here. Zero Some other text here. Greater than zero Some even other text here.
    /// </value>
    public int Something { get; set; }
}
");

        [Test]
        public void Code_gets_fixed_for_problematic_text_in_returns()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <returns>
    /// A value that indicates something being compared. The return value has these meanings: Value Meaning Less than zero Some text here. Zero Some other text here. Greater than zero Some even other text here.
    /// </returns>
    public int DoSomething()
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <returns>
    /// A value that indicates something being compared. The return value has these meanings:
    /// <list type=""table"">
    /// <listheader><term>Value</term><description>Meaning</description></listheader>
    /// <item><term>Less than zero</term><description>Some text here.</description></item>
    /// <item><term>Zero</term><description>Some other text here.</description></item>
    /// <item><term>Greater than zero</term><description>Some even other text here.</description></item>
    /// </list>
    /// </returns>
    public int DoSomething()
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_problematic_text_in_value()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <value>
    /// A value that indicates something being compared. The return value has these meanings: Value Meaning Less than zero Some text here. Zero Some other text here. Greater than zero Some even other text here.
    /// </value>
    public int Something { get; set; }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <value>
    /// A value that indicates something being compared. The return value has these meanings:
    /// <list type=""table"">
    /// <listheader><term>Value</term><description>Meaning</description></listheader>
    /// <item><term>Less than zero</term><description>Some text here.</description></item>
    /// <item><term>Zero</term><description>Some other text here.</description></item>
    /// <item><term>Greater than zero</term><description>Some even other text here.</description></item>
    /// </list>
    /// </value>
    public int Something { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2230_ReturnValueUsesListAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2230_ReturnValueUsesListAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2230_CodeFixProvider();
    }
}