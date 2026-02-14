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
                                                            "actual",
                                                            "array",
                                                            "batch",
                                                            "buffer",
                                                            "cache",
                                                            "collection",
                                                            "dictionary",
                                                            "expected",
                                                            "gateways",
                                                            "items",
                                                            "list",
                                                            "map",
                                                            "mapping",
                                                            "queue",
                                                            "result",
                                                            "results",
                                                            "set",
                                                            "someBatch",
                                                            "someCache",
                                                            "someMap",
                                                            "someTrivia",
                                                            "source",
                                                            "stack",
                                                            "textTokens",
                                                            "trivia",
                                                            "typesUnderTest",
                                                            "variablesRead",
                                                            "variablesWritten",
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
        public void No_issue_is_reported_for_variable_(string variable) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_non_collection_variable() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_collection_variable_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_collection_variable_with_suffix_([ValueSource(nameof(CorrectNamesWithSuffixes))] string name) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_var_collection_variable_with_plural_name() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_byte_array_named_hash() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_Linq_variable_(string variable) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_variable_of_enumerable_type_with_number_suffix_matching_type_name() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_collection_variable_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_collection_variable_with_suffix_([ValueSource(nameof(WrongNamesWithSuffixes))] string name) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_var_collection_variable_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_variable_declaration_pattern_with_plural_name() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_variable_declaration_pattern_with_singular_name() => An_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_ref_var_with_conditional_reference() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_XmlNode_variable() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_XNode_variable() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_variable_of_enumerable_type_named_after_type() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_foreach_variable_of_enumerable_type_named_after_type() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_IGrouping_variable() => No_issue_is_reported_for(@"
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
        public void Code_gets_fixed_by_pluralizing_variable_name_(string originalName, string fixedName)
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
        public void Code_gets_fixed_by_pluralizing_variable_declaration_pattern_name()
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