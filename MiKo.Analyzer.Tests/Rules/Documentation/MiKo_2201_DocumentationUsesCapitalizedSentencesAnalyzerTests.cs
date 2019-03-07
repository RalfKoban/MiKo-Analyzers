
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2201_DocumentationUsesCapitalizedSentencesAnalyzerTests : CodeFixVerifier
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


        private static readonly char[] LowerCaseLetters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        private static readonly char[] UpperCaseLetters = "abcdefghijklmnopqrstuvwxyz".ToUpperInvariant().ToCharArray();

        [Test, Combinatorial]
        public void Documentation_starting_with_upper_case_after_dot_is_not_reported_for([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(UpperCaseLetters))] char startingChar) => No_issue_is_reported_for(@"
/// <"+ xmlTag + @">
/// Documentation. " + startingChar + @" something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void Documentation_starting_with_lower_case_after_dot_is_reported_for([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(LowerCaseLetters))] char startingChar) => An_issue_is_reported_for(@"
/// <"+ xmlTag + @">
/// Documentation. " + startingChar + @" something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void Empty_documentation_is_not_reported_for([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <"+ xmlTag + @" />
public sealed class TestMe { }
");

        [Test]
        public void Nested_XML_documentation_is_not_reported_for([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <"+ xmlTag + @">
/// <some />
/// </"+ xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void Undocumented_class_is_not_reported() => No_issue_is_reported_for(@"
public sealed class TestMe { }
");

        protected override string GetDiagnosticId() => MiKo_2201_DocumentationUsesCapitalizedSentencesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2201_DocumentationUsesCapitalizedSentencesAnalyzer();
    }
}