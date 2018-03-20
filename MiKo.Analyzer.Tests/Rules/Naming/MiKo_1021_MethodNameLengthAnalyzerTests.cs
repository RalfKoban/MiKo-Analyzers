using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture(Explicit = !NamingLengthAnalyzer.EnabledPerDefault)]
    public sealed class MiKo_1021_MethodNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_method_with_fitting_length([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"

public void " + name + @"()
{
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_exceeding_length([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"

public void " + name + @"()
{
}
");

        [TestCase("Fact")]
        [TestCase("Fact()")]
        [TestCase("Test")]
        [TestCase("Test()")]
        [TestCase("TestCase")]
        [TestCase("TestCase()")]
        [TestCase("Theory")]
        [TestCase("Theory()")]
        [TestCase("TestMethod")]
        [TestCase("TestMethod()")]
        public void No_issue_is_reported_for_test_method_with_exceeding_length(string name) => No_issue_is_reported_for(@"

[" + name + @"]
public void Abcdefghijklmnopqrstuvwxyz()
{
}
");

        [Test]
        public void No_issue_is_reported_for_property_accessor_with_exceeding_length() => No_issue_is_reported_for(@"
public class TestMe
{
    public int Abcdefghijklmnopqrstuvwxyz { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_event_accessor_with_exceeding_length() => No_issue_is_reported_for(@"
public class TestMe
{
    public event EventHandler<EventArgs> Abcdefghijklmnopqrstuvwxyz { add; remove; }
}");

        [Test]
        public void No_issue_is_reported_for_explicit_interface_method_with_exceeding_length() => No_issue_is_reported_for(@"
public class TestMe
{
    int IAbcdefghijklmnopqrstuvwxyz.Abcdefghijklmnopqrstuvwxyz() => 42;
}");

        protected override string GetDiagnosticId() => MiKo_1021_MethodNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1021_MethodNameLengthAnalyzer();

        private static IEnumerable<string> Fitting() => GetAllWithMaxLengthOf(Constants.MaxNamingLengths.Methods);

        private static IEnumerable<string> NonFitting() => GetAllAboveLengthOf(Constants.MaxNamingLengths.Methods);
    }
}