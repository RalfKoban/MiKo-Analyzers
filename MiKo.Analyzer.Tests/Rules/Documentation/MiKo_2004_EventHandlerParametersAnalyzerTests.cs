using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2004_EventHandlerParametersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_event_handling_method() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_event_handling_method() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class MyEventArgs : System.EventArgs { }

    public class TestMe
    {
        public void DoSomething(object sender, MyEventArgs e) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_interitdoc_documented_event_handling_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class MyEventArgs : System.EventArgs { }

    public class TestMe
    {
        /// <inheritdoc />
        public void DoSomething(object sender, MyEventArgs e) { }
    }
}
");

        [TestCase("The source of the event.", "A <see cref='MyEventArgs' /> that contains the event data.")]
        [TestCase("The source of the event.", "A <see cref=\"MyEventArgs\" /> that contains the event data.")]
        [TestCase("The source of the event", "A <see cref='MyEventArgs' /> that contains the event data")]
        [TestCase("The source of the event.", "A <see cref='MyEventArgs'/> that contains the event data.")]
        [TestCase("The source of the event", "A <see cref='MyEventArgs'/> that contains the event data")]
        [TestCase("<para>The source of the event.</para>", "<para>A <see cref='MyEventArgs' /> that contains the event data.</para>")]
        [TestCase("<para>The source of the event</para>", "<para>A <see cref='MyEventArgs' /> that contains the event data</para>")]
        [TestCase("Unused.", "A <see cref='MyEventArgs' /> that contains the event data.")]
        [TestCase("Unused", "A <see cref='MyEventArgs' /> that contains the event data")]
        [TestCase("<para>Unused.</para>", "<para>A <see cref='MyEventArgs' /> that contains the event data.</para>")]
        [TestCase("<para>Unused</para>", "<para>A <see cref='MyEventArgs' /> that contains the event data</para>")]
        [TestCase("The source of the event.", "Unused.")]
        [TestCase("The source of the event", "Unused")]
        [TestCase("<para>The source of the event.</para>", "<para>Unused.</para>")]
        [TestCase("<para>The source of the event</para>", "<para>Unused</para>")]
        public void No_issue_is_reported_for_correctly_documented_event_handling_method_(string sender, string e) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class MyEventArgs : System.EventArgs { }

    public class TestMe
    {
        /// <summary>
        /// Does something.
        /// </summary>
        /// <param name='sender'>" + sender + @"</param>
        /// <param name='e'>" + e + @"</param>
        public void DoSomething(object sender, MyEventArgs e) { }
    }
}");

        [TestCase("The source of the event.", "An <see cref='EventArgs' /> that contains the event data.")]
        [TestCase("The source of the event.", "An <see cref=\"EventArgs\" /> that contains the event data.")]
        [TestCase("The source of the event.", "An <see cref='System.EventArgs' /> that contains the event data.")]
        [TestCase("The source of the event.", "An <see cref=\"System.EventArgs\" /> that contains the event data.")]
        public void No_issue_is_reported_for_correctly_documented_event_handling_method_with_vocal_at_begin_(string sender, string e) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Does something.
        /// </summary>
        /// <param name='sender'>" + sender + @"</param>
        /// <param name='e'>" + e + @"</param>
        public void DoSomething(object sender, EventArgs e) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_partly_documented_event_handling_method_with_missing_documentation_for_sender() => No_issue_is_reported_for(@"
public class MyEventArgs : System.EventArgs { }

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name='e'>A <see cref='MyEventArgs' /> that contains the event data.</param>
    public void DoSomething(object sender, MyEventArgs e) { }
}
");

        [Test]
        public void No_issue_is_reported_for_partly_documented_event_handling_method_with_missing_documentation_for_EventArgs() => No_issue_is_reported_for(@"
public class MyEventArgs : System.EventArgs { }

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name='sender'>The source of the event.</param>
    public void DoSomething(object sender, MyEventArgs e) { }
}
");

        [Test]
        public void No_issue_is_reported_for_partly_documented_event_handling_method_with_missing_documentation_for_sender_and_EventArgs() => No_issue_is_reported_for(@"
public class MyEventArgs : System.EventArgs { }

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething(object sender, MyEventArgs e) { }
}
");

        [Test]
        public void Code_gets_fixed_if_sender_and_EventArgs_are_commented()
        {
            const string OriginalCode = @"
using System;

class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""sender"">The sender.</param>
    /// <param name=""e"">Event arguments.</param>
    void DoSomething(object sender, EventArgs e) { }
}";

            const string FixedCode = @"
using System;

class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""sender"">
    /// The source of the event.
    /// </param>
    /// <param name=""e"">
    /// An <see cref=""EventArgs""/> that contains the event data.
    /// </param>
    void DoSomething(object sender, EventArgs e) { }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_sender_and_EventArgs_are_commented_on_different_lines()
        {
            const string OriginalCode = @"
using System;

class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""sender"">
    /// The sender.
    /// </param>
    /// <param name=""e"">
    /// Event arguments.
    /// </param>
    void DoSomething(object sender, EventArgs e) { }
}";

            const string FixedCode = @"
using System;

class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""sender"">
    /// The source of the event.
    /// </param>
    /// <param name=""e"">
    /// An <see cref=""EventArgs""/> that contains the event data.
    /// </param>
    void DoSomething(object sender, EventArgs e) { }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_sender_is_not_commented_and_EventArgs_is_commented_incorrectly()
        {
            const string OriginalCode = @"
using System;

class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""e"">
    /// Some event args.
    /// </param>
    void DoSomething(object sender, EventArgs e) { }
}";

            const string FixedCode = @"
using System;

class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""e"">
    /// An <see cref=""EventArgs""/> that contains the event data.
    /// </param>
    void DoSomething(object sender, EventArgs e) { }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_EventArgs_is_not_commented_and_sender_is_commented_incorrectly()
        {
            const string OriginalCode = @"
using System;

class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""sender"">
    /// The sender.
    /// </param>
    void DoSomething(object sender, EventArgs e) { }
}";

            const string FixedCode = @"
using System;

class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""sender"">
    /// The source of the event.
    /// </param>
    void DoSomething(object sender, EventArgs e) { }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2004_EventHandlerParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2004_EventHandlerParametersAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2004_CodeFixProvider();
    }
}