using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2200_DocumentationStartsCapitalizedAnalyzerTests : CodeFixVerifier
    {
        [Test, Combinatorial]
        public void Documentation_starting_with_upper_case_is_not_reported_for([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(ValidStartingCharacter))] char startingChar) => No_issue_is_reported_for(@"
/// <"+ xmlTag + @">
/// " + startingChar + @" something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void Documentation_starting_with_lower_case_is_reported_for([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(InvalidStartingCharacter))] char startingChar) => An_issue_is_reported_for(@"
/// <"+ xmlTag + @">
/// " + startingChar + @" something.
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
        public void Special_Or_phrase_in_Para_tag_of_XML_documentation_is_not_reported() => No_issue_is_reported_for(@"
/// <para>-or-</para>
public sealed class TestMe { }
");

        protected override string GetDiagnosticId() => MiKo_2200_DocumentationStartsCapitalizedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2200_DocumentationStartsCapitalizedAnalyzer();

        private static IEnumerable<string> XmlTags() => new[]
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

        private static IEnumerable<char> ValidStartingCharacter() => "abcdefghijklmnopqrstuvwxyz".ToUpperInvariant().ToCharArray();

        private static IEnumerable<char> InvalidStartingCharacter() => "abcdefghijklmnopqrstuvwxyz1234567890-#+*.,;".ToCharArray();
    }
}