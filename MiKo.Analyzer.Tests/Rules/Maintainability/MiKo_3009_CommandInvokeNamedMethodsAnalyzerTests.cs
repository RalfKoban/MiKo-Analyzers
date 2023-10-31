using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3009_CommandInvokeNamedMethodsAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_command_with_named_method() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMeCommand : ICommand
{
    public TestMeCommand(Action execute) => _execute = execute;

    public TestMeCommand(Func<bool> canExecute, Action execute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => _canExecute();

    public void Execute(object parameter) _execute();

    private Action _execute;
    private Func<bool> _canExecute;
}

public class TestMe
{
    public void DoSomething()
    {
        var testMeCommand = new TestMeCommand(DoSomething);
    }
}

public class TestMe2
{
    public void DoSomethingElse()
    {
        var testMeCommand = new TestMeCommand(CanDoSomethingElse, DoSomethingElse);
    }

    public bool CanDoSomethingElse() => true;
}
");

        [Test]
        public void An_issue_is_reported_for_command_with_lambda_as_Execute_method() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMeCommand : ICommand
{
    public TestMeCommand(Action execute) => _execute = execute;

    public TestMeCommand(Func<bool> canExecute, Action execute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => _canExecute();

    public void Execute(object parameter) _execute();

    private Action _execute;
    private Func<bool> _canExecute;
}

public class TestMe
{
    public void DoSomething()
    {
        var testMeCommand = new TestMeCommand(() => { });
    }
}
");

        [Test]
        public void An_issue_is_reported_for_command_with_lambda_as_CanExecute_method() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMeCommand : ICommand
{
    public TestMeCommand(Action execute) => _execute = execute;

    public TestMeCommand(Func<bool> canExecute, Action execute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => _canExecute();

    public void Execute(object parameter) _execute();

    private Action _execute;
    private Func<bool> _canExecute;
}

public class TestMe
{
    public void DoSomething()
    {
        var testMeCommand = new TestMeCommand(() => true, DoSomething);
    }
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

        protected override string GetDiagnosticId() => MiKo_3009_CommandInvokeNamedMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3009_CommandInvokeNamedMethodsAnalyzer();
    }
}