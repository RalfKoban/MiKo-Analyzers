using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2204_DocumentationShallUseListAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] EnumerationMarkers =
                                                              [
                                                                  " 1. ",
                                                                  " 1: ",
                                                                  " a. ",
                                                                  " A. ",
                                                                  " 1) ",
                                                                  " a) ",
                                                                  " A) ",
                                                                  " 1.) ",
                                                                  " a.) ",
                                                                  " A.) ",
                                                              ];

        private static readonly string[] Markers = ["-", "*"];

        private static readonly string[] MarkerBeginnings = [string.Empty, ":", " "];

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_enumeration_markers_in_xml_tag_(
                                                                         [ValueSource(nameof(XmlTags))] string xmlTag,
                                                                         [ValueSource(nameof(EnumerationMarkers))] string marker)
            => An_issue_is_reported_for(2, @"
/// <" + xmlTag + @">
/// " + marker + @" something.
/// " + marker + @" something else.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_asterisk_markers_in_documentation_([ValueSource(nameof(MarkerBeginnings))] string markerBegin) => An_issue_is_reported_for(2, @"
/// <summary>
/// The reason" + markerBegin + @"
/// * It is something.
/// * It is something else.
/// </summary>
public sealed class TestMe { }
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_dash_markers_in_documentation_([ValueSource(nameof(MarkerBeginnings))] string markerBegin) => An_issue_is_reported_for(2, @"
/// <summary>
/// The reason" + markerBegin + @"
/// - It is something.
/// - It is something else.
/// </summary>
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_documentation_without_enumeration_markers_in_xml_tag_([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The identifier for something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_documentation_with_double_dash_in_xml_tag_([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The identifier -- if available.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_documentation_with_single_dash_in_xml_tag_([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The identifier - if available - for something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_documentation_with_single_dash_and_see_langword_in_xml_tag_([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The identifier - if not <see langword=""null""/> - for something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_documentation_with_double_dash_and_see_langword_in_xml_tag_([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The identifier -- if not <see langword=""null""/>.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_documentation_containing_similar_words_([Values("heuristic", "et cetera", "superb")] string word) => No_issue_is_reported_for(@"
/// <summary>
/// The identifier " + word + @". Seems to be no problem.
/// </summary>
public sealed class TestMe { }
");

        [Test]
        public void Code_gets_fixed_with_preceding_multi_line_text_([ValueSource(nameof(Markers))] string marker)
        {
            var originalCode = @"
/// <summary>
/// Some text here.
/// The reason:
/// " + marker + @" It is something.
/// " + marker + @" It is something else.
/// </summary>
public sealed class TestMe { }
";

            const string FixedCode = @"
/// <summary>
/// Some text here.
/// The reason:
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
        public void Code_gets_fixed_with_preceding_single_line_text_([ValueSource(nameof(Markers))] string marker)
        {
            var originalCode = @"
/// <summary>
/// The reason:
/// " + marker + @" It is something.
/// " + marker + @" It is something else.
/// </summary>
public sealed class TestMe { }
";

            const string FixedCode = @"
/// <summary>
/// The reason:
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
        public void Code_gets_fixed_with_following_multi_line_text_([ValueSource(nameof(Markers))] string marker)
        {
            var originalCode = @"
/// <summary>
/// " + marker + @" It is something.
/// " + marker + @" It is something else.
/// Those are the options.
/// Some more text here.
/// </summary>
public sealed class TestMe { }
";

            const string FixedCode = @"
/// <summary>
/// <list type=""bullet"">
/// <item><description>It is something.</description></item>
/// <item><description>It is something else.</description></item>
/// </list>
/// Those are the options.
/// Some more text here.
/// </summary>
public sealed class TestMe { }
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_following_single_line_text_([ValueSource(nameof(Markers))] string marker)
        {
            var originalCode = @"
/// <summary>
/// " + marker + @" It is something.
/// " + marker + @" It is something else.
/// Those are the options.
/// </summary>
public sealed class TestMe { }
";

            const string FixedCode = @"
/// <summary>
/// <list type=""bullet"">
/// <item><description>It is something.</description></item>
/// <item><description>It is something else.</description></item>
/// </list>
/// Those are the options.
/// </summary>
public sealed class TestMe { }
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2204_DocumentationShallUseListAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2204_DocumentationShallUseListAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2204_CodeFixProvider();
    }
}