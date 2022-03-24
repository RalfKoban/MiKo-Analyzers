using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2019_3rdPersonSingularVerbSummaryAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ThirdPersonVerbs =
            {
                "Accesses",
                "Allows",
                "Breaks",
                "Contains",
                "Converts",
                "Describes",
                "Gets",
                "Occurs",
                "Performs",
                "Stops",
                "Tells",
            };

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Age { get; set; }

    public void DoSomething() { }

    public event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_documented_exception_class() => No_issue_is_reported_for(@"
/// <summary>
/// The exception to be thrown.
/// </summary>
public class TestMeException : System.Exception
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_class() => No_issue_is_reported_for(@"
using System;

using Bla
{
    /// <summary>
    /// Contains some test data.
    /// </summary>
    public class TestMe
    {
        /// <summary>
        /// Gets or sets some test data.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Performs some test data.
        /// </summary>
        public virtual void DoSomething() { }

        /// <summary>
        /// Allows to do some test data.
        /// </summary>
        public virtual void DoSomething2() { }

        /// <summary>
        /// Breaks to do some test data.
        /// </summary>
        public virtual void DoSomething2() { }

        /// <summary>
        /// Asynchronously provides some test data.
        /// </summary>
        public Task DoSomethingAsync() { }

        /// <summary>
        /// Asynchronously stops some test data.
        /// </summary>
        public Task DoSomethingAsync() { }

        /// <summary>
        /// Occurs after some test data.
        /// </summary>
        public event EventHandler MyEvent;
    }

    /// <summary>
    /// Contains some test data.
    /// </summary>
    public class TestMe2
    {
        /// <summary>
        /// Gets or sets some test data.
        /// </summary>
        public int Age { get; set; }

        /// <inheritdoc />
        public override void DoSomething() { }

        /// <summary>
        /// Asynchronously provides some test data.
        /// </summary>
        public Task DoSomethingAsync() { }

        /// <summary>
        /// Occurs after some test data.
        /// </summary>
        public event EventHandler MyEvent;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_starting_verb_in_passive_form_([ValueSource(nameof(ThirdPersonVerbs))] string verb) => No_issue_is_reported_for(@"
using System;

using Bla
{
    /// <summary>
    /// " + verb + @" some test data.
    /// </summary>
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_starting_verb_in_para_tag_in_passive_form_([ValueSource(nameof(ThirdPersonVerbs))] string verb) => No_issue_is_reported_for(@"
using System;

using Bla
{
    /// <summary>
    /// <para>
    /// " + verb + @" some test data.
    /// </para>
    /// </summary>
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_starting_verb_in_passive_form_followed_by_comma() => No_issue_is_reported_for(@"
using System;

using Bla
{
    /// <summary>
    /// Represents, provides or includes some test data.
    /// </summary>
    public class TestMe
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_class([Values("Provide", "This are")] string start) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// " + start + @" some test data.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_class_in_para_tag_([Values("Provide", "This are")] string start) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// <para>
/// " + start + @" some test data.
/// </para>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_([Values("Perform", "Miss", "Mixs", "Buzzs", "Enrichs", "This are")] string verb) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + verb + @" some test data.
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_async_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Asynchronously perform some test data.
    /// </summary>
    public Task DoSomethingAsync() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Get or sets some test data.
    /// </summary>
    public int Age { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_starting_with_term_([Values("Recursively")] string term) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + term + @" loops over some test data.
    /// </summary>
    public void Loop() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_private_class_with_see_cref_as_second_word() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Converts <see cref=""string""/> into something else.
    /// </summary>
    private class Converter
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_private_class_with_see_cref_as_second_word() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Convert <see cref=""string""/> into something else.
    /// </summary>
    private class Converter
    { }
}
");

        protected override string GetDiagnosticId() => MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer();
    }
}