using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1010_CommandMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_with_completely_different_name() => No_issue_is_reported(@"
public class TestMe
{
    private int DoSomething() => 42;
}
");

        [Test]
        public void No_Issue_is_reported_for_ICommand_method() => No_issue_is_reported(@"
public class TestMe : System.Windows.Input.ICommand
{
    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter)
    {
    }
}
");

        [Test]
        public void Issue_is_reported_for_method_with_Execute_in_name() => Issue_is_reported(@"
public class TestMe
{
    private int DoExecute() => 42;
}
");

        [Test]
        public void Issue_is_reported_for_method_with_CanExecute_in_name() => Issue_is_reported(@"
public class TestMe
{
    private int DoCanExecute() => 42;
}
");

        protected override string GetDiagnosticId() => MiKo_1010_CommandMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1010_CommandMethodsAnalyzer();
    }
}