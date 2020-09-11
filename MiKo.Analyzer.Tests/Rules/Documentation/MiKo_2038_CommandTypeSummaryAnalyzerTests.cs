using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2038_CommandTypeSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMe : ICommand
{
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) { }
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_interface() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public interface ITestMe : ICommand
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_class() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

/// <summary>
/// Represents a command that can do something.
/// </summary>
public class TestMe : ICommand
{
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_interface() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

/// <summary>
/// Represents a command that can do something.
/// </summary>
public interface ITestMe : ICommand
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_class() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

/// <summary>
/// Can do something.
/// </summary>
public class TestMe : ICommand
{
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_interface() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

/// <summary>
/// Can do something.
/// </summary>
public interface ITestMe : ICommand
{
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
using System;
using System.Windows.Input;

/// <summary>
/// Do something.
/// </summary>
public class TestMe : ICommand
{
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) { }
}
";

            const string FixedCode = @"
using System;
using System.Windows.Input;

/// <summary>
/// Represents a command that can do something.
/// </summary>
public class TestMe : ICommand
{
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2038_CommandTypeSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2038_CommandTypeSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2038_CodeFixProvider();
    }
}