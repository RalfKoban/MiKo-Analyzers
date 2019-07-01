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
        public void No_issue_is_reported_for_correctly_commented_enumerable_field([Values("Contains the", "The")] string comment) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// " + comment + @" data for the field.
    /// </summary>
    private List<string> m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_boolean_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Indicates whether something is the data for the field.
    /// </summary>
    private bool m_field;
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
        public void No_issue_is_reported_for_enum_member() => No_issue_is_reported_for(@"
using System;

public enum TestMe
{
    /// <summary>
    /// Bla bla.
    /// </summary>
    Field = 0,
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_field([Values("Bla bla.", "Contains something.", "Indicates whether something.")] string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    private string m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_enumerable_field([Values("Bla bla", "Indicates whether something.")] string comment) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    private List<string> m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_boolean_field([Values("Bla bla", "The field", "Contains something.")] string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    private bool m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_DependencyProperty_field_summary_with_readonly_comment() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_constant_boolean_field() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// The Bla bla
    /// </summary>
    private const bool m_field;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_commented_constant_boolean_field(
                                                                                [Values("Bla bla", "Indicates whether the field", "Contains something.")] string comment,
                                                                                [Values("bool", "string")] string fieldType)
            => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    private const " + fieldType + @" m_field;
}
");

        protected override string GetDiagnosticId() => MiKo_2080_FieldSummaryDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2080_FieldSummaryDefaultPhraseAnalyzer();
    }
}