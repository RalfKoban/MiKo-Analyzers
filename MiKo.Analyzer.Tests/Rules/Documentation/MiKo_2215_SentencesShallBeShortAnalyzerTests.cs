using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2215_SentencesShallBeShortAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_summary_containing_short_sentences() => No_issue_is_reported_for(@"
/// <summary>
/// This is a short sentence. And yet another one. As well as this one. Maybe not the best but still sufficient.
/// </summary>
/// <remarks>
/// Believe it or not but it works.
/// </remarks>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_summary_containing_medium_to_almost_long_sentences() => No_issue_is_reported_for(@"
/// <summary>
/// Specifies that the test fixture(s) marked with this attribute are considered to be <i>atomic</i> by NCrunch, meaning that their child tests cannot
/// be run separately from each other.
/// </summary>
/// <remarks>
/// A test being queued for execution under an atomic fixture will result in the entire fixture being queued with its child tests all executed in the
/// same task/batch.
/// </remarks>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_summary_containing_medium_to_almost_long_sentences_but_not_ending_with_periods() => No_issue_is_reported_for(@"
/// <summary>
/// Specifies that the test fixture(s) marked with this attribute are considered to be <i>atomic</i> by NCrunch, meaning that their child tests cannot
/// be run separately from each other
/// </summary>
/// <remarks>
/// A test being queued for execution under an atomic fixture will result in the entire fixture being queued with its child tests all executed in the
/// same task/batch
/// </remarks>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_code_fragment_containing_long_sentences() => No_issue_is_reported_for(@"
/// <summary>
/// <code>
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </code>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_para_containing_short_sentences_inside_summary() => No_issue_is_reported_for(@"
/// <summary>
/// <para>
/// This is a short sentence.
/// </para>
/// <para>
/// And yet another short one.
/// </para>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_class_summary_containing_long_sentences() => An_issue_is_reported_for(@"
/// <summary>
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_method_summary_containing_long_sentences() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_remarks_containing_long_sentences() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <remarks>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </remarks>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_exception_containing_long_sentences() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <exception cref=""InvalidOperationException"">
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </exception>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_param_containing_long_sentences() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name=""i"">
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </param>
    public void DoSomething(int i)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_returns_containing_long_sentences() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <returns>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </returns>
    public bool DoSomething() => false;
}
");

        [Test]
        public void An_issue_is_reported_for_property_value_containing_long_sentences() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <value>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </value>
    public bool DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_method_example_containing_long_sentences() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <example>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </example>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_enum_member_summary_containing_long_sentences() => An_issue_is_reported_for(@"
public enum TestMe
{
    /// <summary>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </summary>
    None,
}
");

        [Test]
        public void An_issue_is_reported_for_event_field_summary_containing_long_sentences() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </summary>
    event EventHandler MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_event_declaration_summary_containing_long_sentences() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </summary>
    event EventHandler MyEvent
    {
        add { }
        remove { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_field_summary_containing_long_sentences() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </summary>
    private int m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_long_sentence_in_para_inside_summary() => An_issue_is_reported_for(@"
/// <summary>
/// <para>
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </para>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_long_sentence_in_para_inside_content() => An_issue_is_reported_for(@"
/// <content>
/// <para>
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </para>
/// </content>
public class TestMe
{
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_each_section_containing_long_sentences() => An_issue_is_reported_for(2, @"
/// <summary>
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </summary>
/// <remarks>
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </remarks>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_long_sentence_in_one_of_multiple_para_inside_summary() => An_issue_is_reported_for(@"
/// <summary>
/// <para>
/// This is a short sentence.
/// </para>
/// <para>
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </para>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_long_sentence_in_note_inside_summary() => An_issue_is_reported_for(@"
/// <summary>
/// <note type=""note"">
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </note>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_long_sentence_in_note_inside_remarks() => An_issue_is_reported_for(@"
/// <summary />
/// <remarks>
/// <note type=""note"">
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </note>
/// </remarks>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_note_containing_short_sentences_inside_summary() => No_issue_is_reported_for(@"
/// <summary>
/// This is a short sentence.
/// <note type=""note"">
/// And yet another short one.
/// </note>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_note_containing_short_sentences_inside_remarks() => No_issue_is_reported_for(@"
/// <summary />
/// <remarks>
/// This is a short sentence.
/// <note type=""note"">
/// And yet another short one.
/// </note>
/// </remarks>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_long_sentence_outside_of_short_note_inside_summary() => An_issue_is_reported_for(@"
/// <summary>
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// <note type=""note"">
/// This is a short sentence.
/// </note>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_long_sentence_outside_of_short_note_inside_remarks() => An_issue_is_reported_for(@"
/// <summary />
/// <remarks>
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// <note type=""note"">
/// This is a short sentence.
/// </note>
/// </remarks>
public class TestMe
{
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_long_sentences_inside_and_outside_of_note_inside_summary() => An_issue_is_reported_for(2, @"
/// <summary>
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// <note type=""note"">
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </note>
/// </summary>
public class TestMe
{
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_long_sentences_inside_and_outside_of_note_inside_remarks() => An_issue_is_reported_for(2, @"
/// <summary />
/// <remarks>
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// <note type=""note"">
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </note>
/// </remarks>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_long_note_inside_summary_containing_short_text() => An_issue_is_reported_for(@"
/// <summary>
/// This is a short sentence.
/// <note type=""note"">
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </note>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_long_note_inside_remarks_containing_short_text() => An_issue_is_reported_for(@"
/// <summary />
/// <remarks>
/// This is a short sentence.
/// <note type=""note"">
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </note>
/// </remarks>
public class TestMe
{
}
");

        protected override string GetDiagnosticId() => MiKo_2215_SentencesShallBeShortAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2215_SentencesShallBeShortAnalyzer();
    }
}