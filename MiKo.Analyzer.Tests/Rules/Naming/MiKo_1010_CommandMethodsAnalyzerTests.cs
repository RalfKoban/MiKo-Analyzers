using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1010_CommandMethodsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AcceptableCommands = [
                                                                  "CommandBindingOnCanExecute",
                                                                  "CommandBindingOnExecuted",
                                                                  "OnCanExecuteCommandBinding",
                                                                  "OnCommandBindingCanExecute",
                                                                  "OnCommandBindingCanExecute",
                                                                  "OnCommandBindingExecuted",
                                                                  "OnCommandBindingExecuted",
                                                                  "OnCommandExecuted",
                                                                  "OnCommandExecuting",
                                                                  "OnExecutedCommandBinding",
                                                                  "OnMyOwnCommandExecuted",
                                                              ];

        [Test]
        public void No_issue_is_reported_for_method_with_completely_different_name() => No_issue_is_reported_for(@"
public class TestMe
{
    private int DoSomething() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_ICommand_method() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

namespace Bla
{
    public class TestMe : System.Windows.Input.ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
        }

        public event EventHandler CanExecuteChanged;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_CanExecuteChanged_trigger_method_([Values("OnCanExecuteChanged", "RaiseCanExecuteChanged")] string methodName) => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

namespace Bla
{
    public class TestMe : System.Windows.Input.ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
        }

        public event EventHandler CanExecuteChanged;

        public void " + methodName + @"() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Execute_in_name() => An_issue_is_reported_for(@"
public class TestMe
{
    private int DoExecute() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_CanExecute_in_name() => An_issue_is_reported_for(@"
public class TestMe
{
    private int DoCanExecute() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_Execute_in_name() => An_issue_is_reported_for(@"
public class TestMe
{
    private void Something()
    {
        void DoExecute() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_CanExecute_in_name() => An_issue_is_reported_for(@"
public class TestMe
{
    private void Something()
    {
        void DoCanExecute() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_in_ctor_with_Execute_in_name() => An_issue_is_reported_for(@"
public class TestMe
{
    private TestMe()
    {
        void DoExecute() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_in_ctor_with_CanExecute_in_name() => An_issue_is_reported_for(@"
public class TestMe
{
    private TestMe()
    {
        void DoCanExecute() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_handling_method_([ValueSource(nameof(AcceptableCommands))] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    private int " + methodName + @"() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_event_handling_local_function_([ValueSource(nameof(AcceptableCommands))] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    private void Something()
    {
        void " + methodName + @"() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_handling_local_function_in_ctor_([ValueSource(nameof(AcceptableCommands))] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    private TestMe()
    {
        void " + methodName + @"() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
public class TestMe
{
    [" + test + @"]
    public int Do_execute_something() => 42;
}
");

        [TestCase("DoExecute", "Do")]
        [TestCase("CanDoExecute", "CanDo")]
        [TestCase("ExecuteUpdate", "Update")]
        [TestCase("Execute", "Execute")]
        public void Code_gets_fixed_for_method_(string method, string wanted) => VerifyCSharpFix(
                                                                                             "using System; class TestMe { void " + method + "() { } }",
                                                                                             "using System; class TestMe { void " + wanted + "() { } }");

        [TestCase("DoExecute", "Do")]
        [TestCase("CanDoExecute", "CanDo")]
        [TestCase("ExecuteUpdate", "Update")]
        [TestCase("Execute", "Execute")]
        public void Code_gets_fixed_for_local_function_(string method, string wanted) => VerifyCSharpFix(
                                                                                                     "using System; class TestMe { void Something() { void " + method + "() { } } }",
                                                                                                     "using System; class TestMe { void Something() { void " + wanted + "() { } } }");

        protected override string GetDiagnosticId() => MiKo_1010_CommandMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1010_CommandMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1010_CodeFixProvider();
    }
}