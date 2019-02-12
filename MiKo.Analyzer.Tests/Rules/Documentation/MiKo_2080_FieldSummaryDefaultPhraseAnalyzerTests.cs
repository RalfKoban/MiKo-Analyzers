using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2080_FieldSummaryDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private string m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// The field.
    /// </summary>
    private string m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_const_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// The field.
    /// </summary>
    public const string Field;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_field() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Bla bla bla.
    /// </summary>
    private string m_field;
}
");

        protected override string GetDiagnosticId() => MiKo_2080_FieldSummaryDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2080_FieldSummaryDefaultPhraseAnalyzer();
    }
}