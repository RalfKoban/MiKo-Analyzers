using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1028_LocalFunctionNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        private static readonly string[] Fitting = GetAllWithMaxLengthOf(Constants.MaxNamingLengths.Methods);
        private static readonly string[] NonFitting = GetAllAboveLengthOf(Constants.MaxNamingLengths.Methods);

        [Test]
        public void No_issue_is_reported_for_local_function_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void " + name + @"() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void " + name + @"() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_with_fitting_length_inside_test_method_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
using NUnit;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        void " + name + @"() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_exceeding_length_inside_test_method_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
using NUnit;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        void " + name + @"() { }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1028_LocalFunctionNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1028_LocalFunctionNameLengthAnalyzer();
    }
}