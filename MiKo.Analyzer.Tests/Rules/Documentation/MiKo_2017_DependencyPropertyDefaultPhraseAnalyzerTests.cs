using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2017_DependencyPropertyDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_DependencyProperty_field() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    private static readonly DependencyProperty MyDependencyProperty;
}
");

        [Test]
        public void No_issue_is_reported_for_DependencyProperty_field_with_standard_summary_and_value() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public int MyDependency { get; set; }

    /// <summary>
    /// Identifies the <see cref=""MyDependency""/> dependency property.
    /// </summary>
    /// <value>
    /// The identifier for the <see cref=""MyDependency""/> dependency property.
    /// </value>
    private static readonly DependencyProperty MyDependencyProperty;
}
");

        [Test]
        public void No_issue_is_reported_for_DependencyProperty_field_with_standard_summary() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public int MyDependency { get; set; }

    /// <summary>
    /// Identifies the <see cref=""MyDependency""/> dependency property.
    /// </summary>
    private static readonly DependencyProperty MyDependencyProperty;
}
");

        [Test]
        public void No_issue_is_reported_for_DependencyProperty_field_with_standard_summary_including_readonly_statement() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public int MyDependency { get; set; }

    /// <summary>
    /// Identifies the <see cref=""MyDependency""/> dependency property. This field is read-only.
    /// </summary>
    private static readonly DependencyProperty MyDependencyProperty;
}
");

        [Test]
        public void No_issue_is_reported_for_DependencyProperty_field_with_standard_value() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public int MyDependency { get; set; }

    /// <value>
    /// The identifier for the <see cref=""MyDependency""/> dependency property.
    /// </value>
    private static readonly DependencyProperty MyDependencyProperty;
}
");

        [Test]
        public void An_issue_is_reported_for_DependencyProperty_field_with_non_standard_summary() => An_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public int MyDependency { get; set; }

    /// <summary>
    /// My summary.
    /// </summary>
    private static readonly DependencyProperty MyDependencyProperty;
}
");

        [Test]
        public void An_issue_is_reported_for_DependencyProperty_field_with_non_standard_value() => An_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public int MyDependency { get; set; }

    /// <value>
    /// My value.
    /// </value>
    private static readonly DependencyProperty MyDependencyProperty;
}
");

        [Test]
        public void Code_gets_fixed_by_replacing_with_standard_documentation()
        {
            const string OriginalCode = @"
using System.Windows;

public class TestMe
{
    public int MyDependency { get; set; }

    /// <summary>
    /// Some summary.
    /// </summary>
    /// <value>
    /// Some value.
    /// </value>
    private static readonly DependencyProperty MyDependencyProperty;
}";

            const string FixedCode = @"
using System.Windows;

public class TestMe
{
    public int MyDependency { get; set; }

    /// <summary>
    /// Identifies the <see cref=""MyDependency""/> dependency property. This field is read-only.
    /// </summary>
    /// <value>
    /// The identifier for the <see cref=""MyDependency""/> dependency property.
    /// </value>
    private static readonly DependencyProperty MyDependencyProperty;
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2017_DependencyPropertyDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2017_DependencyPropertyDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2017_CodeFixProvider();
    }
}