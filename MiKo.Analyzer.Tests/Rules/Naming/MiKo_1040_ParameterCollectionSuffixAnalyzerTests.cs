using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1040_ParameterCollectionSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("object item")]
        [TestCase("string value")]
        [TestCase("string MyValue")]
        [TestCase("int[] numbers")]
        [TestCase("int[] array")]
        [TestCase("IList<string> list")]
        [TestCase("ICollection<int> collection")]
        [TestCase("ICollection<string> playlist")]
        [TestCase("ICollection<string> blacklist")]
        [TestCase("ICollection<string> whitelist")]
        [TestCase("IDictionary<string,string> dictionary")]
        public void No_issue_is_reported_for_correctly_named_parameter_(string parameter) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(" + parameter + @")
    { }
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
        public void No_issue_is_reported_for_correctly_named_XML_parameter_(string parameter) => No_issue_is_reported_for(@"
using System;
using System.Xml;
using System.Xml.Linq;

public class TestMe
{
    public void DoSomething(" + parameter + @")
    { }
}
");

        [Test] // this situation is covered by CA 1725 so we do not report that as well
        public void No_issue_is_reported_for_incorrectly_named_parameter_of_method_that_implements_interface() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public interface ITestMe
{
    void DoSomething(List<string> myItems);
}

public class TestMe : ITestMe
{
    public void DoSomething(List<string> itemList)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_boolean_parameter() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe : ITestMe
{
    public void DoSomething(bool refreshList)
    { }
}
");

        [TestCase("value")]
        [TestCase("source")]
        public void No_issue_is_reported_for_extension_method_parameter_(string parameter) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public static class TestMeExtensions
{
    public static void DoSomething(this List<int> " + parameter + @")
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_better_parameter_name_that_would_match_an_existing_parameter() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(List<int> value, int[] values)
    { }
}
");

        [TestCase("IGrouping<int, string> group")]
        [TestCase("IQueryable query")]
        [TestCase("IQueryable<int> query")]
        [TestCase("IOrderedQueryable query")]
        [TestCase("IOrderedQueryable<int> query")]
        public void No_issue_is_reported_for_parameter_(string parameter) => No_issue_is_reported_for(@"
using System.Linq;

public class TestMe
{
    public void DoSomething("" + parameter + @"")
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameter_of_specific_type_with_number_at_the_end() => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public interface IXyzObject42 : IEnumerable<string>, IEnumerable
{
}

public class TestMe
{
    public void DoSomething(IXyzObject42 xyzObject)
    {
    }
}
");

        [TestCase("string blaEnumList")]
        [TestCase("string blaList")]
        [TestCase("string blaCollection")]
        [TestCase("string blaObservableCollection")]
        [TestCase("string blaArray")]
        [TestCase("string blaHashSet")]
        [TestCase("string blaDictionary")]
        [TestCase("string blaDict")]
        [TestCase("string blaDic")]
        public void An_issue_is_reported_for_incorrectly_named_parameter_(string parameter) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(" + parameter + @")
    { }
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
        public void Code_gets_fixed_for_parameter_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething(int[] ###)
    {
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1040_ParameterCollectionSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1040_ParameterCollectionSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1040_CodeFixProvider();
    }
}