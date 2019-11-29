using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3028_AssignNullToListItemParameterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_null_assignment_in_block_in_lambda_statement() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        var list = new List<object>();
        list.ForEach(item => { item = new object(); });
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_null_assignment_in_simple_lambda_statement() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        var list = new List<object>();
        list.ForEach(item => item = new object());
    }
}
");

        [Test]
        public void An_issue_is_reported_for_null_assignment_in_block_in_lambda_statement() => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        var list = new List<object>();
        list.ForEach(item => { item = null; });
    }
}
");

        [Test]
        public void An_issue_is_reported_for_null_assignment_in_simple_lambda_statement() => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        var list = new List<object>();
        list.ForEach(item => item = null);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_null_assignment_to_different_variable_in_block_lambda_statement() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        var list = new List<object>();
        list.ForEach(item =>
                            {
                                var copy = item;
                                copy = null;
                            });
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3028_AssignNullToListItemParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3028_AssignNullToListItemParameterAnalyzer();
    }
}