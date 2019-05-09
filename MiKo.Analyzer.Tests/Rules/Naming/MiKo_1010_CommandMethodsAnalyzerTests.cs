﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1010_CommandMethodsAnalyzerTests : CodeFixVerifier
    {
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
        public void No_issue_is_reported_for_CanExecuteChanged_trigger_method([Values("OnCanExecuteChanged", "RaiseCanExecuteChanged")] string methodName) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_event_handling_method_([Values("OnCommandExecuting", "OnCommandExecuted", "OnMyOwnCommandExecuted")] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    private int " + methodName + @"() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_test_method([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
public class TestMe
{
    [" + test + @"]
    public int Do_execute_something() => 42;
}
");

        protected override string GetDiagnosticId() => MiKo_1010_CommandMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1010_CommandMethodsAnalyzer();
    }
}