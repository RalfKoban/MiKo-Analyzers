using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1045_CommandInvokeMethodsSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_command_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object DoSomething()
    {
        return new object();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_command_methods() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMeCommand : ICommand
{
    public TestMeCommand(Action execute) => _execute = execute;

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) _execute();

    private Action _execute;
}

public class TestMe
{
    public void DoSomething()
    {
        var testMeCommand = new TestMeCommand(DoSomething);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_command_methods() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMeCommand : ICommand
{
    public TestMeCommand(Action execute) => _execute = execute;

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) _execute();

    private Action _execute;
}

public class TestMe
{
    public void DoSomething()
    {
        var testMeCommand = new TestMeCommand(TestCommand);
    }

    private void TestCommand() { }
}
");

        [Test]
        public void No_issue_is_reported_for_command_with_no_arguments() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMeCommand : ICommand
{
    public TestMeCommand() { }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) { };

    public string Text { get; set; }
}

public class TestMe
{
    public void DoSomething()
    {
        var testMeCommand = new TestMeCommand { Text=""something"" };
    }
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string Template = @"
using System;
using System.Windows.Input;

public class TestMeCommand : ICommand
{
    public TestMeCommand(Action execute) => _execute = execute;

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) _execute();

    private Action _execute;
}

public class TestMe
{
    public void Initialize()
    {
        var testMeCommand = new TestMeCommand(###);
    }

    private void ###() { }
}
";

            VerifyCSharpFix(Template.Replace("###", "DoSomethingCommand"), Template.Replace("###", "DoSomething"));
        }

        protected override string GetDiagnosticId() => MiKo_1045_CommandInvokeMethodsSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1045_CommandInvokeMethodsSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1045_CodeFixProvider();
    }
}