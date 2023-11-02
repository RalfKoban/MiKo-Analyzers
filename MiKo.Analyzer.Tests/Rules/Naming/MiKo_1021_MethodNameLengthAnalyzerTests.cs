using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1021_MethodNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        private static readonly string[] Fitting = GetAllWithMaxLengthOf(Constants.MaxNamingLengths.Methods);
        private static readonly string[] NonFitting = GetAllAboveLengthOf(Constants.MaxNamingLengths.Methods);

        [Test]
        public void No_issue_is_reported_for_method_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + name + @"()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + name + @"()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_exceeding_length_([ValueSource(nameof(Tests))] string name) => No_issue_is_reported_for(@"
using NUnit;

public class TestMe
{
    [" + name + @"]
    public void Abcdefghijklmnopqrstuvwxyz()
    {
    }
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

        protected override DiagnosticAnalyzer GetObjectUnderTest()
        {
            NamingLengthAnalyzer.EnabledPerDefault = true;

            Analyzer.Reset();

            return new MiKo_1021_MethodNameLengthAnalyzer();
        }
    }
}