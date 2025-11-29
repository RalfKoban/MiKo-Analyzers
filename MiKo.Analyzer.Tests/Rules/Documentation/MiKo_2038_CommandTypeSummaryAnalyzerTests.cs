using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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

        [TestCase("A command that can do")]
        [TestCase("Command that can do")]
        [TestCase("Command which can do")]
        [TestCase("Can do")]
        [TestCase("Interface for commands in")]
        [TestCase("Interface for toggle commands in")]
        [TestCase("A interface for commands in")]
        [TestCase("An interface for commands in")]
        [TestCase("The interface for commands in")]
        [TestCase("A interface for toggle commands in")]
        [TestCase("An interface for toggle commands in")]
        [TestCase("The interface for toggle commands in")]
        [TestCase("Interface for a command in")]
        [TestCase("Interface for a command that can do")]
        [TestCase("Interface for a command which can do")]
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

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_class_with_see_cref() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

/// <summary>
/// <see cref=""ICommand""/>
/// </summary>
public class TestMe : ICommand
{
}
");

        [TestCase("A class that offers to execute", "execute")]
        [TestCase("A class that tries to execute", "execute")]
        [TestCase("A command base class that tries to execute", "execute")]
        [TestCase("A command executes", "execute")]
        [TestCase("A command that can do", "do")]
        [TestCase("A helper command that is able to do something", "do something")]
        [TestCase("A helper command to be able to do something", "do something")]
        [TestCase("A helper command which is able to do something", "do something")]
        [TestCase("A interface for commands in", "be used in")]
        [TestCase("A interface for toggle commands in", "be used in")]
        [TestCase("A standard command to execute", "execute")]
        [TestCase("A sync command to execute", "execute")]
        [TestCase("A synchronous command to execute", "execute")]
        [TestCase("A toggle command to execute", "execute")]
        [TestCase("An async command to execute", "execute")]
        [TestCase("An asynchronous command to execute", "execute")]
        [TestCase("An command base class that tries to execute", "execute")]
        [TestCase("An helper command that is able to do something", "do something")]
        [TestCase("An helper command to be able to do something", "do something")]
        [TestCase("An helper command which is able to do something", "do something")]
        [TestCase("An interface for commands in", "be used in")]
        [TestCase("An interface for toggle commands in", "be used in")]
        [TestCase("Backstage command that can be used without", "be used without")]
        [TestCase("Base class for backstage commands that can open", "open")]
        [TestCase("Base class for backstage commands that open", "open")]
        [TestCase("Base class for commands used to show and change", "show and change")]
        [TestCase("Base class for commands with", "be used with")]
        [TestCase("Base class for the backstage commands that can open", "open")]
        [TestCase("Base class for the backstage commands that open", "open")]
        [TestCase("Base class for the commands used to show and change", "show and change")]
        [TestCase("Base class for the commands with", "be used with")]
        [TestCase("Base class for the upload and download command for device parameters", "upload and download device parameters")]
        [TestCase("Base class for the upload and download commands for device parameters", "upload and download device parameters")]
        [TestCase("Command base class that tries to execute", "execute")]
        [TestCase("Command that can do", "do")]
        [TestCase("Do", "do")]
        [TestCase("Exported and in context menu usable extension of", "be used within a context menu of")]
        [TestCase("Exported and in context menu useable extension of", "be used within a context menu of")]
        [TestCase("Exported and in context-menu usable extension of", "be used within a context menu of")]
        [TestCase("Exported and in context-menu useable extension of", "be used within a context menu of")]
        [TestCase("Extension of", "extend")]
        [TestCase("Helper command that is able to do something", "do something")]
        [TestCase("Helper command to be able to do something", "do something")]
        [TestCase("Helper command which is able to do something", "do something")]
        [TestCase("In context menu usable extension of", "be used within a context menu of")]
        [TestCase("In context menu useable extension of", "be used within a context menu of")]
        [TestCase("In context-menu usable extension of", "be used within a context menu of")]
        [TestCase("In context-menu useable extension of", "be used within a context menu of")]
        [TestCase("Interface for a command in", "be used in")]
        [TestCase("Interface for a command that can do", "do")]
        [TestCase("Interface for a command which can do", "do")]
        [TestCase("Interface for a sync command that can do", "do")]
        [TestCase("Interface for a toggle command in", "be used in")]
        [TestCase("Interface for a toggle command that can do", "do")]
        [TestCase("Interface for a toggle command which can do", "do")]
        [TestCase("Interface for an async command that can do", "do")]
        [TestCase("Interface for an command in", "be used in")]
        [TestCase("Interface for an command that can do", "do")]
        [TestCase("Interface for an command which can do", "do")]
        [TestCase("Interface for an toggle command in", "be used in")]
        [TestCase("Interface for an toggle command that can do", "do")]
        [TestCase("Interface for an toggle command which can do", "do")]
        [TestCase("Interface for command which is used for opening", "open")]
        [TestCase("Interface for commands in", "be used in")]
        [TestCase("Interface for the command in", "be used in")]
        [TestCase("Interface for the command that can do", "do")]
        [TestCase("Interface for the command which can do", "do")]
        [TestCase("Interface for the toggle command in", "be used in")]
        [TestCase("Interface for the toggle command that can do", "do")]
        [TestCase("Interface for the toggle command which can do", "do")]
        [TestCase("Interface for toggle commands in", "be used in")]
        [TestCase("Interface for wrapping", "wrap")]
        [TestCase("Interface which is used for ex-/importing", "ex-/import")]
        [TestCase("Offers to execute", "execute")]
        [TestCase("Provides a functionality to step", "step")]
        [TestCase("Provides functionality to step", "step")]
        [TestCase("Provides the functionality to step", "step")]
        [TestCase("Represents a command that is capable to open", "open")]
        [TestCase("The class which offers to execute", "execute")]
        [TestCase("The class which tries to execute", "execute")]
        [TestCase("The command base class that tries to execute", "execute")]
        [TestCase("The command that can do", "do")]
        [TestCase("The helper command is able to do something", "do something")]
        [TestCase("The helper command that is able to do something", "do something")]
        [TestCase("The helper command to be able to do something", "do something")]
        [TestCase("The helper command which is able to do something", "do something")]
        [TestCase("The interface for commands in", "be used in")]
        [TestCase("The interface for toggle commands in", "be used in")]
        [TestCase("The interface is needed for the", "support the")]
        [TestCase("The interface needed for the", "support the")]
        [TestCase("The standard command to execute", "execute")]
        [TestCase("The toggle command to execute", "execute")]
        [TestCase("This class offers to execute", "execute")]
        [TestCase("This class tries to execute", "execute")]
        [TestCase("This command can be used to execute", "execute")]
        [TestCase("This command is used to execute", "execute")]
        [TestCase("Tries to execute", "execute")]
        [TestCase("Wrapper for", "wrap")]
        public void Code_gets_fixed_(string originalComment, string fixedComment)
        {
            const string Template = @"
using System;
using System.Windows.Input;

/// <summary>
/// ### something.
/// </summary>
public class TestMe : ICommand
{
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) { }
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", "Represents a command that can " + fixedComment));
        }

        protected override string GetDiagnosticId() => MiKo_2038_CommandTypeSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2038_CommandTypeSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2038_CodeFixProvider();
    }
}