using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
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

        [TestCase("A command that can do something.")]
        [TestCase("Command that can do something.")]
        [TestCase("Command which can do something.")]
        [TestCase("Can do something.")]
        public void An_issue_is_reported_for_incorrectly_documented_interface_(string text) => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

/// <summary>
/// " + text + @"
/// </summary>
public interface ITestMe : ICommand
{
}
");

        [TestCase("A command that can do something.", "Represents a command that can do something.")]
        [TestCase("The command that can do something.", "Represents a command that can do something.")]
        [TestCase("Command that can do something.", "Represents a command that can do something.")]
        [TestCase("Do something.", "Represents a command that can do something.")]
        [TestCase("A command executes something.", "Represents a command that can execute something.")]
        [TestCase("A toggle command to execute something.", "Represents a command that can execute something.")]
        [TestCase("The toggle command to execute something.", "Represents a command that can execute something.")]
        [TestCase("A standard command to execute something.", "Represents a command that can execute something.")]
        [TestCase("The standard command to execute something.", "Represents a command that can execute something.")]
        public void Code_gets_fixed_(string originalComment, string fixedComment)
        {
            const string Template = @"
using System;
using System.Windows.Input;

/// <summary>
/// ###
/// </summary>
public class TestMe : ICommand
{
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) { }
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        protected override string GetDiagnosticId() => MiKo_2038_CommandTypeSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2038_CommandTypeSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2038_CodeFixProvider();
    }
}