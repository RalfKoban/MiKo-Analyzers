using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2052_ArgumentNullExceptionPhraseAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_ArgumentNullException_with_standard_phrase_for_single_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentNullException_with_standard_phrase_for_Nullable_struct() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(int? o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentNullException_with_standard_phrase_for_multiple_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o1""/> is <see langword=""null""/>.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentNullException_with_non_standard_phrase() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// The <paramref name=""o""/> is not set.
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentNullException_with_non_standard_phrase_for_Nullable_struct() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// The <paramref name=""o""/> is not set.
    /// </exception>
    public void DoSomething(int? o) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentNullException_without_paramref_tags() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// Thrown if something is not set.
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentNullException_with_non_standard_phrase_for_first_of_multiple_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// The <paramref name=""o1""/> is not set.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentNullException_with_non_standard_phrase_for_second_of_multiple_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o1""/> is <see langword=""null""/>.
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
        public void No_issue_is_reported_for_property_with_ArgumentNullException_with_standard_phrase() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""value""/> is <see langword=""null""/>.
    /// </exception>
    public object DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_ArgumentNullException_with_non_standard_phrase() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// The <paramref name=""value""/> is not set.
    /// </exception>
    public object DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_ArgumentNullException_without_paramref_tags() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// Thrown if something is not set.
    /// </exception>
    public object DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_generic_class_constrained_method_with_ArgumentNullException_with_standard_phrase() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Verifies that a specific value is not <see langword=""null""/>.
    /// </summary>
    /// <typeparam name=""T"">
    /// The type of <paramref name=""argument""/>.
    /// </typeparam>
    /// <param name=""argument"">
    /// The value to verify.
    /// </param>
    /// <param name=""argumentName"">
    /// The name of the value.
    /// </param>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""argument""/> is <see langword=""null""/>.
    /// </exception>
    [Pure]
    internal static void NotNull<T>(T argument, string argumentName) where T : class
    {
        if (argument is null)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_generic_class_constrained_method_with_ArgumentNullException_with_non_standard_phrase() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Verifies that a specific value is not <see langword=""null""/>.
    /// </summary>
    /// <typeparam name=""T"">
    /// The type of <paramref name=""argument""/>.
    /// </typeparam>
    /// <param name=""argument"">
    /// The value to verify.
    /// </param>
    /// <param name=""argumentName"">
    /// The name of the value.
    /// </param>
    /// <exception cref=""ArgumentNullException"">
    /// The <paramref name=""argument""/> is <see langword=""null""/>.
    /// </exception>
    [Pure]
    internal static void NotNull<T>(T argument, string argumentName) where T : class
    {
        if (argument is null)
        {
        }
    }
}
");

        [TestCase(nameof(ArgumentNullException), "If null")]
        [TestCase(nameof(ArgumentNullException), @"If <paramref name=""o""/> is null")]
        [TestCase(nameof(ArgumentNullException), @"If <paramref name=""o""/> is <see langword=""null""/>")]
        [TestCase(nameof(ArgumentNullException), @"If the <paramref name=""o""/> is <see langword=""null""/>.")]
        [TestCase(nameof(ArgumentNullException), @"The <paramref name=""o""/> parameter is <see langword=""null""/>.")]
        [TestCase("System." + nameof(ArgumentNullException), "If null")]
        [TestCase("System." + nameof(ArgumentNullException), @"If <paramref name=""o""/> is null")]
        [TestCase("System." + nameof(ArgumentNullException), @"If <paramref name=""o""/> is <see langword=""null""/>")]
        [TestCase("System." + nameof(ArgumentNullException), @"If the <paramref name=""o""/> is <see langword=""null""/>.")]
        [TestCase("System." + nameof(ArgumentNullException), @"The <paramref name=""o""/> is <see langword=""null""/>.")]
        public void Code_gets_fixed_by_replacing_with_standard_phrase_for_single_parameter_(string exceptionType, string text)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""" + exceptionType + @""">
    /// " + text + @"
    /// </exception>
    public void DoSomething(object o) { }
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""" + exceptionType + @""">
    /// <paramref name=""o""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o) { }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_replacing_with_standard_phrase_for_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// If <paramref name=""o1""/> or <paramref name=""o2""/> are null.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o1""/> is <see langword=""null""/>.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_replacing_with_standard_phrase_for_only_referenced_parameter_when_multiple_exceptions()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// If <paramref name=""o1""/> is no item
    /// </exception>
    /// <exception cref=""ArgumentNullException"">
    /// If <paramref name=""o2""/> is null
    /// </exception>
    public void DoSomething(object o1, object o2) { }
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
    /// If <paramref name=""o1""/> is no item
    /// </exception>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o2""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_replacing_with_standard_phrase_for_only_referenced_parameters_variant_1()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// If <paramref name=""o1""/> or <paramref name=""o2""/> are null.
    /// </exception>
    public void DoSomething(object o1, object o2, object o3) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o1""/> is <see langword=""null""/>.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o1, object o2, object o3) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_replacing_with_standard_phrase_for_only_referenced_parameters_variant_2()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    ///     If o1 or o2 are null.
    /// </exception>
    public void DoSomething(object o1, object o2, object o3) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o1""/> is <see langword=""null""/>.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o1, object o2, object o3) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_replacing_with_standard_phrase_for_property_setter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    ///     If value is null.
    /// </exception>
    public object Something { get; set; }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""value""/> is <see langword=""null""/>.
    /// </exception>
    public object Something { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_replacing_with_standard_phrase_for_indexer()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    ///     If key is null.
    /// </exception>
    public object this[object key] { get; set; }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""key""/> is <see langword=""null""/>.
    /// </exception>
    public object this[object key] { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_adding_standard_phrase_for_empty_ArgumentNullException()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <exception cref=""ArgumentNullException""></exception>
    public void DoSomething(object o) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_adding_standard_phrase_for_empty_ArgumentNullException_ignoring_struct_parameters()
        {
            const string OriginalCode = @"
using System;
using System.Threading;
using System.Threading.Tasks;

public class TestMe
{
    /// <exception cref=""ArgumentNullException""></exception>
    public Task<int> DoSomething(object o1, object o2, CancellationToken token = default) { }
}
";

            const string FixedCode = @"
using System;
using System.Threading;
using System.Threading.Tasks;

public class TestMe
{
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o1""/> is <see langword=""null""/>.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is <see langword=""null""/>.
    /// </exception>
    public Task<int> DoSomething(object o1, object o2, CancellationToken token = default) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2052_ArgumentNullExceptionPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2052_ArgumentNullExceptionPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2052_CodeFixProvider();
    }
}