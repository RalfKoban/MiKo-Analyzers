using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

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
        public void No_issue_is_reported_for_non_command_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() {}
}
");

        [Test]
        public void No_issue_is_reported_for_non_command_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Bla { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_command_indexer() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMe
{
    public ICommand this[string key] => Create(key);

    private ICommand Create(string key) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_factory_method() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMe
{
    public ICommand Create(string key) => null;
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

        [TestCase("_command")]
        [TestCase("m_command")]
        [TestCase("_myCommand")]
        [TestCase("m_myCommand")]
        public void No_issue_is_reported_for_correctly_named_command_field_(string fieldName) => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMe
{
    private ICommand " + fieldName + @";
}
");

        [Test]
        public void No_issue_is_reported_for_command_method_in_test() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    public ICommand CreateObjectUnderTest() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_command_property_in_test() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    public ICommand ObjectUnderTest { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_command_field_in_test() => No_issue_is_reported_for(@"
using System;
using System.Windows.Input;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    private ICommand ObjectUnderTest;
}
");

        [Test]
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

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_command_interface() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public interface ITestMe : ICommand
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_command_method() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMe
{
    public ICommand DoSomething() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_command_property() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMe
{
    public ICommand Bla { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_command_field() => An_issue_is_reported_for(@"
using System;
using System.Windows.Input;

public class TestMe
{
    private ICommand m_bla;
}
");

        [Test]
        public void Code_gets_fixed_for_field()
        {
            const string OriginalCode = @"
using System;
using System.Windows.Input;

public class TestMe
{
    private ICommand m_field;
}
";

            const string FixedCode = @"
using System;
using System.Windows.Input;

public class TestMe
{
    private ICommand m_fieldCommand;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property()
        {
            const string OriginalCode = @"
using System;
using System.Windows.Input;

public class TestMe
{
    public ICommand Bla { get; set; }
}
";

            const string FixedCode = @"
using System;
using System.Windows.Input;

public class TestMe
{
    public ICommand BlaCommand { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method()
        {
            const string OriginalCode = @"
using System;
using System.Windows.Input;

public class TestMe
{
    public ICommand DoSomething() => null;
}
";

            const string FixedCode = @"
using System;
using System.Windows.Input;

public class TestMe
{
    public ICommand DoSomethingCommand() => null;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface()
        {
            const string OriginalCode = @"
using System;
using System.Windows.Input;

public interface ITestMe : ICommand
{
}
";

            const string FixedCode = @"
using System;
using System.Windows.Input;

public interface ITestMeCommand : ICommand
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class()
        {
            const string OriginalCode = @"
using System;
using System.Windows.Input;

public class TestMe : ICommand
{
}
";

            const string FixedCode = @"
using System;
using System.Windows.Input;

public class TestMeCommand : ICommand
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1044_CommandSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1044_CommandSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1044_CodeFixProvider();
    }
}