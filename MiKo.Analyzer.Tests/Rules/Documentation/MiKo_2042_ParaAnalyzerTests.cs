using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2042_ParaAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectItems = ["<para />", "<para/>", "<br/>", "<br />"];

        private static readonly string[] WrongItems = CreateWrongItems();

        [Test]
        public void No_issue_is_reported_for_valid_documentation_on_class_([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
/// <summary>
/// Does something. " + finding + @"
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_valid_documentation_on_method_([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public void Correct() { }
}
");

        [Test]
        public void No_issue_is_reported_for_valid_documentation_on_property_([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public int Correct { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_valid_documentation_on_event_([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public event EventHandler Correct;
}
");

        [Test]
        public void No_issue_is_reported_for_valid_documentation_on_field_([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    private string Correct;
}
");

        [Test]
        public void No_issue_is_reported_for_valid_example_on_class_([ValueSource(nameof(WrongItems))] string finding) => No_issue_is_reported_for(@"
/// <summary>
/// Does something.
/// </summary>
/// <example>
/// <code>" + finding + @"</code>
/// </example>
public sealed class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation_on_class_([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
/// <summary>
/// Does something. " + finding + @"
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation_on_method_([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public void Malform() { }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation_on_property_([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public int Malform { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation_on_event_([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public event EventHandler Malform;
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation_on_field_([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    private string Malform;
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_example_on_class_([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
/// <summary>
/// Does something.
/// </summary>
/// <example>
/// " + finding + @"
/// </example>
public sealed class TestMe
{
}
");

        [Test]
        public void Code_gets_fixed_for_empty_P_tag_on_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// <p/>
    /// Something more.
    /// </summary>
    void DoSomething() { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// <para/>
    /// Something more.
    /// </summary>
    void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_P_tag_on_type()
        {
            const string OriginalCode = @"
/// <summary>
/// Does something.
/// <p>
/// Something more.
/// </p>
/// </summary>
public class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Does something.
/// <para>
/// Something more.
/// </para>
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_P_tag_on_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// <p>
    /// Something more.
    /// </p>
    /// </summary>
    void DoSomething() { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// <para>
    /// Something more.
    /// </para>
    /// </summary>
    void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2042_ParaAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2042_ParaAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2042_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static string[] CreateWrongItems()
        {
            string[] tokens = ["<p>Whatever.</p>", "<p/>", "<p />"];

            var results = new HashSet<string>(2 * tokens.Length);

            foreach (var token in tokens)
            {
                results.Add(token);
                results.Add(token.ToUpperInvariant());
            }

            return [.. results];
        }
    }
}