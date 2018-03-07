using System;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2017_DependencyDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_field() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    private static readonly DependencyProperty MyDependencyProperty;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_field() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_field_summary() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_field_value() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_commented_field_summary() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_correctly_commented_field_value() => An_issue_is_reported_for(@"
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

        protected override string GetDiagnosticId() => MiKo_2017_DependencyDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2017_DependencyDefaultPhraseAnalyzer();
    }
}