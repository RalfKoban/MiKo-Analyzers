using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2229_XmlFragmentAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correct_XML() => No_issue_is_reported_for(@"
/// <summary>
/// Some text
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_correct_XML_with_CDataSection() => No_issue_is_reported_for(@"
/// <summary>
/// Some text
/// </summary>
/// <example>
/// <code>
/// <![CDATA[
///    <Button>
///    </Button>
/// ]]>
/// </code>
/// </example>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrect_XML_([Values("</", "/>", "/ >")] string fragment) => An_issue_is_reported_for(@"
/// <summary>
/// Some text
/// </summary>" + fragment + @"
public class TestMe
{
}
");

        [Test]
        public void Code_gets_fixed_for_incorrect_XML_([Values("</", "/>", "/ >")] string fragment)
        {
            const string Template = @"
/// <summary>
/// Some text
/// </summary>###
public class TestMe
{
}
";

            VerifyCSharpFix(Template.Replace("###", fragment), Template.Replace("###", string.Empty));
        }

        protected override string GetDiagnosticId() => MiKo_2229_XmlFragmentAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2229_XmlFragmentAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2229_CodeFixProvider();
    }
}