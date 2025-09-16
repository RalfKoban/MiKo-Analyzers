using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1041_FieldCollectionSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] FieldPrefixes = Constants.Markers.FieldPrefixes;

        [Test]
        public void No_issue_is_reported_for_correctly_named_field_([ValueSource(nameof(FieldPrefixes))] string prefix, [Values("dictionary", "map", "array", "value", "myValue")] string field)
            => No_issue_is_reported_for(@"

public class TestMe
{
    private string " + prefix + field + @";
}
");

        [TestCase("blaList")]
        [TestCase("blaCollection")]
        [TestCase("blaObservableCollection")]
        [TestCase("blaArray")]
        [TestCase("blaHashSet")]
        public void No_issue_is_reported_for_incorrectly_named_field_in_enum_(string field) => No_issue_is_reported_for(@"

public enum TestMe
{
    " + field + @",
}
");

        [TestCase("XAttribute attribute")]
        [TestCase("XAttribute myAttribute")]
        [TestCase("XDocument document")]
        [TestCase("XDocument myDocument")]
        [TestCase("XElement element")]
        [TestCase("XElement myElement")]
        [TestCase("XmlAttribute attribute")]
        [TestCase("XmlAttribute myAttribute")]
        [TestCase("XmlDocument document")]
        [TestCase("XmlDocument myDocument")]
        [TestCase("XmlElement element")]
        [TestCase("XmlElement myElement")]
        [TestCase("XmlNode myNode")]
        [TestCase("XmlNode node")]
        [TestCase("XNode myNode")]
        [TestCase("XNode node")]
        public void No_issue_is_reported_for_correctly_named_XML_field_(string field) => No_issue_is_reported_for(@"
using System;
using System.Xml;
using System.Xml.Linq;

public class TestMe
{
    private " + field + @";
}
");

        [TestCase("string blaList")]
        [TestCase("string blaEnumList")]
        [TestCase("string blaCollection")]
        [TestCase("string blaObservableCollection")]
        [TestCase("string blaArray")]
        [TestCase("string blaHashSet")]
        [TestCase("string blaDictionary")]
        [TestCase("string blaDict")]
        [TestCase("string blaDic")]
        public void An_issue_is_reported_for_incorrectly_named_field_(string field) => An_issue_is_reported_for(@"

public class TestMe
{
    private " + field + @";
}
");

        [TestCase("number", "numbers")]
        [TestCase("resultOfSomething", "resultsOfSomething")]
        [TestCase("resultToShow", "resultsToShow")]
        [TestCase("resultWithData", "resultsWithData")]
        [TestCase("resultInSomething", "resultsInSomething")]
        [TestCase("resultFromSomething", "resultsFromSomething")]
        [TestCase("itemList", "items")]
        [TestCase("triviaList", "trivia")]
        [TestCase("allElementNodeList", "allElements")]
        [TestCase("allElementReferenceNodeList", "allElements")]
        [TestCase("elementNodeList", "elements")]
        [TestCase("elementReferenceNodeList", "elements")]
        public void Code_gets_fixed_for_field_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

public class TestMe
{
    private int[] ###;
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1041_FieldCollectionSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1041_FieldCollectionSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1041_CodeFixProvider();
    }
}