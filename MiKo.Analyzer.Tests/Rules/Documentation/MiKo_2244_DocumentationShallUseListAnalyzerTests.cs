using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2244_DocumentationShallUseListAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags = ["example", "exception", "note", "overloads", "para", "param", "permission", "remarks", "returns", "summary", "typeparam", "value"];

        [Test]
        public void An_issue_is_reported_for_Enumeration_in_Xml_tag_([ValueSource(nameof(XmlTags))] string xmlTag, [Values("ul", "ol", "UL", "OL")] string wrongTag)
            => An_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// <" + wrongTag + @">
/// <li>Something.</li>
/// </" + wrongTag + @">
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_uncommented_class() => No_issue_is_reported_for(@"
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_normal_comment_in_XML_tag_([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The identifier for something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void Code_gets_fixed_for_unordered_list_([Values("ul", "UL")] string wrongTag)
        {
            var originalCode = @"
/// <summary>
/// Some text here.
/// <" + wrongTag + @">
/// <li>It is something.</li>
/// <li>It is something else.</li>
/// </" + wrongTag + @">
/// </summary>
public sealed class TestMe { }
";

            const string FixedCode = @"
/// <summary>
/// Some text here.
/// <list type=""bullet"">
/// <item><description>It is something.</description></item>
/// <item><description>It is something else.</description></item>
/// </list>
/// </summary>
public sealed class TestMe { }
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ordered_list_([Values("ol", "OL")] string wrongTag)
        {
            var originalCode = @"
/// <summary>
/// Some text here.
/// <" + wrongTag + @">
/// <li>It is something.</li>
/// <li>It is something else.</li>
/// </" + wrongTag + @">
/// </summary>
public sealed class TestMe { }
";

            const string FixedCode = @"
/// <summary>
/// Some text here.
/// <list type=""number"">
/// <item><description>It is something.</description></item>
/// <item><description>It is something else.</description></item>
/// </list>
/// </summary>
public sealed class TestMe { }
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2244_DocumentationShallUseListAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2244_DocumentationShallUseListAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2244_CodeFixProvider();
    }
}