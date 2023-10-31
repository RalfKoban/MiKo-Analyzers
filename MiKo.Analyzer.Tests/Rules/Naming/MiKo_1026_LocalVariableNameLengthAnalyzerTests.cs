using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1026_LocalVariableNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        private static readonly string[] Fitting = GetAllWithMaxLengthOf(Constants.MaxNamingLengths.LocalVariables);
        private static readonly string[] NonFitting = GetAllAboveLengthOf(Constants.MaxNamingLengths.LocalVariables);

        [Test]
        public void No_issue_is_reported_for_variable_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        var " + name + @" = 42;
        return " + name + @"; 
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        var " + name + @" = 42;
        return " + name + @"; 
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        const int " + name + @" = 42;
        return " + name + @"; 
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_declaration_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case int " + name + @": return 42;
            default: return -1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_declaration_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case int " + name + @": return 42;
            default: return -1;
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1026_LocalVariableNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest()
        {
            NamingLengthAnalyzer.EnabledPerDefault = true;

            return new MiKo_1026_LocalVariableNameLengthAnalyzer();
        }
    }
}