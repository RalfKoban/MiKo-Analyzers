using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        private static readonly string[] Fitting = GetAllWithMaxLengthOf(Constants.MaxNamingLengths.LocalVariablesInLoops);
        private static readonly string[] NonFitting = GetAllAboveLengthOf(Constants.MaxNamingLengths.LocalVariablesInLoops);

        [Test]
        public void No_issue_is_reported_for_variable_in_foreach_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(string[] data)
    {
        foreach (var " + name + @" in data)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_in_for_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        for(var " + name + " = 0; " + name + " < 1; " + name + @"++)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_in_foreach_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(string[] data)
    {
        foreach (var " + name + @" in data)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_in_for_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        for(var " + name + " = 0; " + name + " < 1; " + name + @"++)
        {
        }

    }
}
");

        protected override string GetDiagnosticId() => MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzer();
    }
}