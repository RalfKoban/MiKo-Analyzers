using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2054_ArgumentExceptionPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method_without_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentException_starting_with_paramref_and_verb_([Values("is", "does", "has", "contains")] string phrase) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// <paramref name=""o""/> " + phrase + @" something.
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentException_starting_with_paramref_and_verb_for_multiple_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// <paramref name=""o1""/> is something.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is something.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentException_not_starting_with_paramref() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// The <paramref name=""o""/> is not set.
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentException_without_paramref_tags() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// Thrown if something is not set.
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentException_not_starting_with_paramref_for_first_of_multiple_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// The <paramref name=""o1""/> is not set.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is something.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentException_not_starting_with_paramref_for_second_of_multiple_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// <paramref name=""o1""/> is something.
    /// <para>-or-</para>
    /// The <paramref name=""o2""/> is not set.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_property_without_setter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public object DoSomething { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_property_with_private_setter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <value>
    /// Something to return.
    /// </value>
    public object DoSomething { get; private set; }
}
");

        [Test]
        public void No_issue_is_reported_for_property_with_ArgumentException_starting_with_paramref_and_verb() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// <paramref name=""value""/> is something.
    /// </exception>
    public object DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_ArgumentException_not_starting_with_paramref() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// The <paramref name=""value""/> is not set.
    /// </exception>
    public object DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_ArgumentException_without_paramref_tags() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// Thrown if something is not set.
    /// </exception>
    public object DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentException_comparing_multiple_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// <paramref name=""o1""/> is not <paramref name=""o2""/>.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
");

        [Test]
        public void Code_gets_fixed_by_replacing_with_paramref_starting_phrase_for_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// In case i is -1.
    /// </exception>
    public void DoSomething(int i) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// <paramref name=""i""/> is -1.
    /// </exception>
    public void DoSomething(int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_replacing_with_paramref_starting_phrase_for_indexer()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    ///     If key is -1.
    /// </exception>
    public object this[int key] { get; set; }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    ///     <paramref name=""key""/> is -1.
    /// </exception>
    public object this[int key] { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2054_ArgumentExceptionPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2054_ArgumentExceptionPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2054_CodeFixProvider();
    }
}