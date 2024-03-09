using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2205_DocumentationShallUseNoteAnalyzerTests : CodeFixVerifier
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

        private static readonly string[] Markers =
                                                   {
                                                       "Attention:",
                                                       " ATTENTION: ",
                                                       " !!! ATTENTION !!! ",
                                                       " !!!ATTENTION!!! ",
                                                       "Caution:",
                                                       " CAUTION: ",
                                                       " !!! CAUTION !!! ",
                                                       " !!!CAUTION!!! ",
                                                       " Note: ",
                                                       " Please note: ",
                                                       " Important: ",
                                                   };

        [Test, Combinatorial]
        public void An_issue_is_reported_for_information_marker_in_Xml_tag_([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(Markers))] string marker) => An_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// " + marker + @" something.
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
/// The something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correct_comment_in_XML_tag_([ValueSource(nameof(XmlTags))] string xmlTag, [Values("caution", "important")] string type) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// <note type=""" + type + @""" >
/// The something.
/// </note>
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        protected override string GetDiagnosticId() => MiKo_2205_DocumentationShallUseNoteAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2205_DocumentationShallUseNoteAnalyzer();
    }
}