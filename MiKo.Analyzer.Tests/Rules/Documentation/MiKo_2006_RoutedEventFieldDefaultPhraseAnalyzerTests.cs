using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2006_RoutedEventFieldDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_field() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public static readonly System.Windows.RoutedEvent TouchUpEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_field() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public event EventHandler<System.Windows.Input.TouchEventArgs> TouchUp;

    /// <summary>
    /// Identifies the <see cref=""TouchUp""/> routed event.
    /// </summary>
    /// <value>
    /// The identifier for the <see cref=""TouchUp""/> routed event.
    /// </value>
    public static readonly System.Windows.RoutedEvent TouchUpEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_field_summary() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public event EventHandler<System.Windows.Input.TouchEventArgs> TouchUp;

    /// <summary>
    /// Identifies the <see cref=""TouchUp""/> routed event.
    /// </summary>
    public static readonly System.Windows.RoutedEvent TouchUpEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_field_summary_with_readonly_comment() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public event EventHandler<System.Windows.Input.TouchEventArgs> TouchUp;

    /// <summary>
    /// Identifies the <see cref=""TouchUp""/> routed event. This field is read-only.
    /// </summary>
    public static readonly System.Windows.RoutedEvent TouchUpEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_field_value() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public event EventHandler<System.Windows.Input.TouchEventArgs> TouchUp;

    /// <value>
    /// The identifier for the <see cref=""TouchUp""/> routed event.
    /// </value>
    public static readonly System.Windows.RoutedEvent TouchUpEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_field_summary() => An_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public event EventHandler<System.Windows.Input.TouchEventArgs> TouchUp;

    /// <summary>
    /// My summary.
    /// </summary>
    public static readonly System.Windows.RoutedEvent TouchUpEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_field_value() => An_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public event EventHandler<System.Windows.Input.TouchEventArgs> TouchUp;

    /// <value>
    /// My value.
    /// </value>
    public static readonly System.Windows.RoutedEvent TouchUpEvent;
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
using System.Windows;

public class TestMe
{
    public event EventHandler<System.Windows.Input.TouchEventArgs> TouchUp;

    /// <summary>
    /// My summary.
    /// </summary>
    /// <value>
    /// My value.
    /// </value>
    public static readonly System.Windows.RoutedEvent TouchUpEvent;
}";

            const string FixedCode = @"
using System.Windows;

public class TestMe
{
    public event EventHandler<System.Windows.Input.TouchEventArgs> TouchUp;

    /// <summary>
    /// Identifies the <see cref=""TouchUp""/> routed event. This field is read-only.
    /// </summary>
    /// <value>
    /// The identifier for the <see cref=""TouchUp""/> routed event.
    /// </value>
    public static readonly System.Windows.RoutedEvent TouchUpEvent;
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2006_RoutedEventFieldDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2006_RoutedEventFieldDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2006_CodeFixProvider();
    }
}