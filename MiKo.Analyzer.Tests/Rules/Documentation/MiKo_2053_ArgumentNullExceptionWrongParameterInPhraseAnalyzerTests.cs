using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2053_ArgumentNullExceptionWrongParameterInPhraseAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_ArgumentNullException_documenting_class_parameter() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_ArgumentNullException_documenting_class_parameter_inside_para_tags() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <para>
    /// <paramref name=""o""/> is <see langword=""null""/>.
    /// </para>
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentNullException_documenting_Nullable_struct_parameter() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_ArgumentNullException_documenting_multiple_nullable_parameters() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_ArgumentNullException_documenting_non_nullable_struct_parameter() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(int o) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentNullException_documenting_non_nullable_struct_parameter_inside_para_tags() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <para>
    /// <paramref name=""o""/> is <see langword=""null""/>.
    /// </para>
    /// </exception>
    public void DoSomething(int o) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentNullException_documenting_non_nullable_struct_as_first_of_multiple_parameters() => An_issue_is_reported_for(@"
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
    public void DoSomething(int o1, object o2) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentNullException_documenting_non_nullable_struct_as_second_of_multiple_parameters() => An_issue_is_reported_for(@"
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
    public void DoSomething(object o1, int o2) { }
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
        public void No_issue_is_reported_for_property_with_ArgumentNullException_for_class_type() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_property_with_ArgumentNullException_for_non_nullable_struct_type() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""value""/> is <see langword=""null""/>.
    /// </exception>
    public int DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_generic_class_constrained_parameter() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_generic_struct_constrained_parameter() => An_issue_is_reported_for(@"
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
    internal static void NotNull<T>(T argument, string argumentName) where T : struct
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2053_ArgumentNullExceptionWrongParameterInPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2053_ArgumentNullExceptionWrongParameterInPhraseAnalyzer();
    }
}