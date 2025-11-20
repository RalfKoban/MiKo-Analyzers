using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1070_CollectionLocalVariableAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectNames =
                                                        [
                                                            "map",
                                                            "mapping",
                                                            "array",
                                                            "buffer",
                                                            "collection",
                                                            "dictionary",
                                                            "items",
                                                            "list",
                                                            "queue",
                                                            "result",
                                                            "results",
                                                            "set",
                                                            "source",
                                                            "stack",
                                                            "typesUnderTest",
                                                            "variablesRead",
                                                            "variablesWritten",
                                                            "gateways",
                                                            "someTrivia",
                                                            "textTokens",
                                                            "trivia",
                                                            "someMap",
                                                            "actual",
                                                            "expected",
                                                        ];

        private static readonly string[] WrongNames =
                                                      [
                                                          "item",
                                                          "enumerable",
                                                          "target",
                                                          "myDictionary",
                                                          "myList42",
                                                          "myDict",
                                                          "myDic",
                                                          "myEnumList",
                                                      ];

        private static readonly string[] CorrectNamesWithSuffixes =
                                                                    [
                                                                        "resultsOfSomething",
                                                                        "resultsToShow",
                                                                        "resultsWithData",
                                                                        "resultsInSomething",
                                                                        "resultsFromSomething",
                                                                        "fieldInitializers",
                                                                    ];

        private static readonly string[] WrongNamesWithSuffixes =
                                                                  [
                                                                      "resultOfSomething",
                                                                      "resultToShow",
                                                                      "resultWithData",
                                                                      "resultInSomething",
                                                                      "resultFromSomething",
                                                                  ];

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [TestCase("object item")]
        [TestCase("string value")]
        [TestCase("string myValue")]
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
        public void No_issue_is_reported_for_method_with_variable_(string variable) => No_issue_is_reported_for(@"
using System;
using System.Xml;
using System.Xml.Linq;

public class TestMe
{
    public void DoSomething()
    {
        string " + variable + @" = null;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_non_Collection_variable() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int i = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_Collection_variable_with_correct_name_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        int[] " + name + @" = new int[0];
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_Collection_variable_with_correct_name_and_additional_suffix_([ValueSource(nameof(CorrectNamesWithSuffixes))] string name) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        int[] " + name + @" = new int[0];
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_var_Collection_variable_with_correct_name() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        var items = new int[0];
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_byte_array_for_hash() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        byte[] hash = new byte[0];
    }
}
");

        [TestCase("IGrouping<int, string> group")]
        [TestCase("IQueryable query")]
        [TestCase("IQueryable<int> query")]
        [TestCase("IOrderedQueryable query")]
        [TestCase("IOrderedQueryable<int> query")]
        public void No_issue_is_reported_for_method_with_(string variable) => No_issue_is_reported_for(@"
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        " + variable + @" = null;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_variable_of_specific_type_with_number_at_the_end() => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public interface IXyzObject42 : IEnumerable<string>, IEnumerable
{
}

public class TestMe
{
    public void DoSomething()
    {
        IXyzObject42 xyzObject = null;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Collection_variable_with_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        int[] " + name + @" = new int[0];
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Collection_variable_with_incorrect_name_with_suffix_([ValueSource(nameof(WrongNamesWithSuffixes))] string name) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        int[] " + name + @" = new int[0];
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_var_Collection_variable_with_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        var " + name + @" = new int[0];
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_variable_declaration_pattern_for_Collection_variable_with_correct_name() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case int[] items: return;
            default: return;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_variable_declaration_pattern_for_Collection_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case int[] item: return;
            default: return;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_for_statement_without_identifier() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i, string methodName)
    {
        for (; i < methodName.Length; i++)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_strange_ref_usage() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(Guid guid1, Guid guid2)
    {
        ref var x = (guid1 == Guid.Empty) ? ref guid1 : ref guid2;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_XML_node() => No_issue_is_reported_for(@"
using System;
using System.Xml;

public class TestMe
{
    public void DoSomething()
    {
        XmlDocument document = null;
        XmlNode xmlNode = document.SelectSingleNode("""");
        XmlNode node = xmlNode.SelectSingleNode("""");
    }
}
");

        [Test]
        public void No_issue_is_reported_for_XML_Linq_node() => No_issue_is_reported_for(@"
using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

public class TestMe
{
    public void DoSomething()
    {
        XDocument document = null;
        XElement element = document.XPathSelectElement("""");
        XNode node = element.FirstNode;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_document_node() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class Document : IEnumerable<int>
{
}

public class TestMe
{
    public void DoSomething()
    {
        Document document = new Document();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_document_variable_in_foreach() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class Document : IEnumerable<int>
{
}

public class TestMe
{
    public void DoSomething(IEnumerable<Document> documents)
    {
        foreach (var document in documents)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_IGrouping_node() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public string Name { get; set; }

    public void DoSomething(IEnumerable<TestMe> items)
    {
        var itemsGroupedByName = items.GroupBy(_ => _.Name);

        foreach (var itemGroup in itemsGroupedByName)
        {
        }
    }
}
");

        [TestCase("number", "numbers")]
        [TestCase("resultOfSomething", "resultsOfSomething")]
        [TestCase("resultToShow", "resultsToShow")]
        [TestCase("resultWithData", "resultsWithData")]
        [TestCase("resultInSomething", "resultsInSomething")]
        [TestCase("resultFromSomething", "resultsFromSomething")]
        [TestCase("triviaList", "trivia")]
        [TestCase("allElementNodeList", "allElements")]
        [TestCase("allElementReferenceNodeList", "allElements")]
        [TestCase("elementNodeList", "elements")]
        [TestCase("elementReferenceNodeList", "elements")]
        public void Code_gets_fixed_for_variable_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] ### = new int[0];
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        [Test]
        public void Code_gets_fixed_for_variable_declaration_pattern()
        {
            const string OriginalCode = @"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case int[] item: return;
            default: return;
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case int[] items: return;
            default: return;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1070_CollectionLocalVariableAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1070_CollectionLocalVariableAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1070_CodeFixProvider();
    }
}