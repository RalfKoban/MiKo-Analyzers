using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1044_CommandSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_command_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");
        [Test]
        public void No_issue_is_reported_for_non_command_interface() => No_issue_is_reported_for(@"
using System;

public interface ITestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_non_command_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() {}
}
");

        [Test]
        public void An_issue_is_reported_for_non_command_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Bla { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_non_command_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int m_bla;
}
");

        [Test, Ignore("Currently, ICommand is not detected properly by Roslyn when run within unit test")]
        public void An_issue_is_reported_for_incorrectly_named_command_class() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMe : ICommand
{
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) { }
}
");

        [Test, Ignore("Currently, ICommand is not detected properly by Roslyn when run within unit test")]
        public void An_issue_is_reported_for_incorrectly_named_command_interface() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public interface ITestMe : ICommand
{
}
");

        [Test, Ignore("Currently, ICommand is not detected properly by Roslyn when run within unit test")]
        public void An_issue_is_reported_for_incorrectly_named_command_method() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMe
{
    public ICommand DoSomething() => null;
}
");

        [Test, Ignore("Currently, ICommand is not detected properly by Roslyn when run within unit test")]
        public void An_issue_is_reported_for_incorrectly_named_command_property() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMe
{
    public ICommand Bla { get; set; }
}
");

        [Test, Ignore("Currently, ICommand is not detected properly by Roslyn when run within unit test")]
        public void An_issue_is_reported_for_incorrectly_named_command_field() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMe
{
    private ICommand m_bla;
}
");

        protected override string GetDiagnosticId() => MiKo_1044_CommandSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1044_CommandSuffixAnalyzer();
    }
}