using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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

        private static readonly TestCaseData[] CodeFixData = [.. CreateCodeFixData()];

        [Test]
        public void No_issue_is_reported_for_correctly_named_field() => No_issue_is_reported_for(@"

public class TestMe
{
    private object _item;
}
");

        [TestCase("IGrouping<int, string> group")]
        [TestCase("IQueryable query")]
        [TestCase("IQueryable<int> query")]
        [TestCase("IOrderedQueryable query")]
        [TestCase("IOrderedQueryable<int> query")]
        [TestCase("int[] replacementMap")]
        [TestCase("string[] WrongNamesForConcreteLookup")]
        [TestCase("string[] WrongNamesForLookup")]
        public void No_issue_is_reported_for_field_(string field) => No_issue_is_reported_for(@"
using System.Linq;

public class TestMe
{
    private " + field + @";
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_field_([ValueSource(nameof(FieldPrefixes))] string prefix, [Values("dictionary", "map", "array", "value", "myValue", "replacementMap")] string field)
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

        [Test]
        public void No_issue_is_reported_for_method_with_field_of_specific_type_with_number_at_the_end() => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public interface IXyzObject42 : IEnumerable<string>, IEnumerable
{
}

public class TestMe
{
    private IXyzObject42 xyzObject;
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

        [Test]
        public void Code_gets_fixed_for_field_([ValueSource(nameof(CodeFixData))] TestCaseData data)
        {
            const string Template = @"
using System;

public class TestMe
{
    private int[] ###;
}
";

            VerifyCSharpFix(Template.Replace("###", data.Wrong), Template.Replace("###", data.Fixed));
        }

        protected override string GetDiagnosticId() => MiKo_1041_FieldCollectionSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1041_FieldCollectionSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1041_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<TestCaseData> CreateCodeFixData()
        {
            Pair[] pairs =
                           [
                               new("number", "numbers"),
                               new("resultOfSomething", "resultsOfSomething"),
                               new("resultToShow", "resultsToShow"),
                               new("resultWithData", "resultsWithData"),
                               new("resultInSomething", "resultsInSomething"),
                               new("resultFromSomething", "resultsFromSomething"),
                               new("itemList", "items"),
                               new("triviaList", "trivia"),
                               new("allElementNodeList", "allElements"),
                               new("allElementReferenceNodeList", "allElements"),
                               new("elementNodeList", "elements"),
                               new("elementReferenceNodeList", "elements"),
                           ];

            foreach (var prefix in FieldPrefixes)
            {
                foreach (var pair in pairs)
                {
                    yield return new TestCaseData
                                     {
                                         Wrong = prefix + pair.Key,
                                         Fixed = prefix + pair.Value,
                                     };
                }
            }
        }
    }
}