using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2037_CommandPropertySummaryAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ValidPhrasesForReadWrite =
                                                                    {
                                                                        "Gets or sets the <see cref=\"ICommand\" /> that can ",
                                                                        "Gets or sets the <see cref=\"ICommand\"/> that can ",
                                                                    };

        private static readonly string[] ValidPhrasesForReadOnly =
                                                                   {
                                                                       "Gets the <see cref=\"ICommand\" /> that can ",
                                                                       "Gets the <see cref=\"ICommand\"/> that can ",
                                                                   };

        private static readonly string[] ValidPhrasesForWriteOnly =
                                                                    {
                                                                        "Sets the <see cref=\"ICommand\" /> that can ",
                                                                        "Sets the <see cref=\"ICommand\"/> that can ",
                                                                    };

        [Test]
        public void No_issue_is_reported_for_undocumented_property() => No_issue_is_reported_for(@"
using System.Windows.Input;

public class TestMe
{
    public ICommand DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_readwrite_property_([ValueSource(nameof(ValidPhrasesForReadWrite))] string phrase) => No_issue_is_reported_for(@"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// " + phrase + @" something.
    /// </summary>
    public ICommand DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_readonly_property_([ValueSource(nameof(ValidPhrasesForReadOnly))] string phrase) => No_issue_is_reported_for(@"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// " + phrase + @" something.
    /// </summary>
    public ICommand DoSomething { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_writeonly_property_([ValueSource(nameof(ValidPhrasesForWriteOnly))] string phrase) => No_issue_is_reported_for(@"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// " + phrase + @" something.
    /// </summary>
    public ICommand DoSomething { set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_readwrite_property() => An_issue_is_reported_for(@"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// Do something.
    /// </summary>
    public ICommand DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_readonly_property() => An_issue_is_reported_for(@"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// Do something.
    /// </summary>
    public ICommand DoSomething { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_writeonly_property() => An_issue_is_reported_for(@"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// Do something.
    /// </summary>
    public ICommand DoSomething { set; }
}
");

        [Test]
        public void Code_gets_fixed_for_readonly_arrow_Command_property()
        {
            const string OriginalCode = @"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// Something.
    /// </summary>
    public ICommand DoSomething => null;
}
";

            const string FixedCode = @"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// Gets the <see cref=""ICommand""/> that can something.
    /// </summary>
    public ICommand DoSomething => null;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_readonly_Command_property()
        {
            const string OriginalCode = @"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// Something.
    /// </summary>
    public ICommand DoSomething { get; }
}
";

            const string FixedCode = @"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// Gets the <see cref=""ICommand""/> that can something.
    /// </summary>
    public ICommand DoSomething { get; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_writeonly_Command_property()
        {
            const string OriginalCode = @"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// Something.
    /// </summary>
    public ICommand DoSomething { set; }
}
";

            const string FixedCode = @"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// Sets the <see cref=""ICommand""/> that can something.
    /// </summary>
    public ICommand DoSomething { set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_readwrite_Command_property()
        {
            const string OriginalCode = @"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// Something.
    /// </summary>
    public ICommand DoSomething { get; set; }
}
";

            const string FixedCode = @"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// Gets or sets the <see cref=""ICommand""/> that can something.
    /// </summary>
    public ICommand DoSomething { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2037_CommandPropertySummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2037_CommandPropertySummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2037_CodeFixProvider();
    }
}