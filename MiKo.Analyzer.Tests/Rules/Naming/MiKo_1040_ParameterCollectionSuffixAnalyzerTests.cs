using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1040_ParameterCollectionSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("string bla")]
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

        [TestCase("string blaList")]
        [TestCase("string blaCollection")]
        [TestCase("string blaObservableCollection")]
        [TestCase("string blaArray")]
        [TestCase("string blaHashSet")]
        [TestCase("string blaDictionary")]
        public void An_issue_is_reported_for_incorrectly_named_parameter_(string parameter) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(" + parameter + @")
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_parameter_of_method_that_implements_interface() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public interface ITestMe
{
    void DoSomething(List<string> items);
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

        protected override string GetDiagnosticId() => MiKo_1040_ParameterCollectionSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1040_ParameterCollectionSuffixAnalyzer();
    }
}