using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2204_DocumentationShallUseListAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags =
            {
                "example",
                "exception",
                "note",
                "overloads",
                "para",
                "param",
                "permission",
                "remarks",
                "returns",
                "summary",
                "typeparam",
                "value",
            };

        private static readonly string[] EnumerationMarkers =
            {
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
            };

        [Test, Combinatorial]
        public void An_issue_is_reported_for_Enumeration_in_Xml_tag([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(EnumerationMarkers))] string marker) => An_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The " + marker + @" something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void An_issue_is_reported_for_dot_enumeration_in_comment([Values("", ":", " ")] string markerBegin) => An_issue_is_reported_for(@"
/// <summary>
/// The reason" + markerBegin + @"
/// - It is something.
/// - It is something else.
/// </summary>
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_uncommented_class() => No_issue_is_reported_for(@"
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_normal_comment_in_XML_tag([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The identifier for something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_comment_with_slash_in_XML_tag([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The identifier - if available - for something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        protected override string GetDiagnosticId() => MiKo_2204_DocumentationShallUseListAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2204_DocumentationShallUseListAnalyzer();
    }
}