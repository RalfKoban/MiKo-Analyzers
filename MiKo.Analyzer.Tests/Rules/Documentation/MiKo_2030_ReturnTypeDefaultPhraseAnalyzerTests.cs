using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2030_ReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public object DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property() => No_issue_is_reported_for(@"
public class TestMe
{
    public object DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething(object o) { }
}
");

        [TestCase("string")]
        [TestCase("bool")]
        [TestCase("Task")]
        [TestCase("Task<string>")]
        [TestCase("Task<bool>")]
        [TestCase(nameof(System.String))]
        [TestCase(nameof(System.Boolean))]
        [TestCase(nameof(System.Threading.Tasks.Task))]
        public void No_issue_is_reported_for_method_that_returns_a(string returnType) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public " + returnType + @" DoSomething(object o) => throw new NotSupportedException();
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_method(
                                                                    [Values("returns", "value")] string xmlTag,
                                                                    [Values("A whatever", "An whatever", "The whatever")] string comment)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + comment + @"
    /// </" + xmlTag + @">
    public object DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_wrong_commented_method(
                                                                [Values("returns", "value")] string xmlTag,
                                                                [Values("Whatever")] string comment)
            => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + comment + @"
    /// </" + xmlTag + @">
    public object DoSomething(object o) => null;
}
");

        protected override string GetDiagnosticId() => MiKo_2030_ReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2030_ReturnTypeDefaultPhraseAnalyzer();
    }
}