﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2000_MalformedDocumentationAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Malformed_documentation_is_reported_on_class() => An_issue_is_reported_for(@"
/// <summary>
/// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Malformed_documentation_is_reported_on_method() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
    /// </summary>
    public void Malform() { }
}
");

        [Test]
        public void Malformed_documentation_is_reported_on_property() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
    /// </summary>
    public int Malform { get; set; }
}
");

        [Test]
        public void Malformed_documentation_is_reported_on_event() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
    /// </summary>
    public event EventHandler Malform;
}
");

        [Test]
        public void Malformed_documentation_is_reported_on_field() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
    /// </summary>
    private string Malform;
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_class() => No_issue_is_reported_for(@"
/// <summary>
/// Something valid.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_method() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Something valid.
    /// </summary>
    public void Malform() { }
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_property() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Something valid.
    /// </summary>
    public int Malform { get; set; }
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_event() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Something valid.
    /// </summary>
    public event EventHandler Malform;
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_field() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Something valid.
    /// </summary>
    private string Malform;
}
");

        protected override string GetDiagnosticId() => MiKo_2000_MalformedDocumentationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2000_MalformedDocumentationAnalyzer();
    }
}