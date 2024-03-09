using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2046_InvalidTypeParameterReferenceInXmlAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongTags =
                                                     {
                                                         "see cref=",
                                                         "see name=",
                                                         "seealso cref=",
                                                         "seealso name=",
                                                     };

        [Test]
        public void No_issue_is_reported_for_undocumented_generic_type() => No_issue_is_reported_for(@"
using System;

public class TestMe<T> where T : class
{
    public void DoSomething(T t)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_generic_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething<T>(T t) where T : class
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_summary_on_method_without_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_summary_on_generic_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething<T>(T t) where T : class
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_summary_on_generic_method_with_see_tag() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// <see langword=""true""/> if something; otherwise, <see langword=""false""/>.
    /// </returns>
    public bool DoSomething<T>(T t) where T : class
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_summary_on_non_generic_method() => No_issue_is_reported_for(@"
using System;

public class TestMe<T> where T : class
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething(T t)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_summary_on_type() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Does something with <typeparamref name=""T"" />.
/// </summary>
public class TestMe<T> where T: class
{
    public void DoSomething(T t)
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_summary_on_generic_method_([ValueSource(nameof(WrongTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something with <" + tag + @"""T"" />.
    /// </summary>
    public void DoSomething<T>(T t) where T: class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_summary_on_non_generic_method_([ValueSource(nameof(WrongTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe<T> where T : class
{
    /// <summary>
    /// Does something with <" + tag + @"""T"" />.
    /// </summary>
    public void DoSomething(T t)
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_returnValue_on_non_generic_method_([ValueSource(nameof(WrongTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe<T> where T : class
{
    /// <returns>
    /// Returns something with <" + tag + @"""T"" />.
    /// </returns>
    public int DoSomething(T t) => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_exception_on_non_generic_method_([ValueSource(nameof(WrongTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe<T> where T : class
{
    /// <exception cref=""ArgumentException"">
    /// Throws something with <" + tag + @"""T"" />.
    /// </exception>
    public int DoSomething(T t) => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_summary_on_generic_type_([ValueSource(nameof(WrongTags))] string tag) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Does something with <" + tag + @"""T"" />.
/// </summary>
public class TestMe<T> where T: class
{
    public void DoSomething(T t)
    { }
}
");

        [Test]
        public void Code_gets_fixed_for_([ValueSource(nameof(WrongTags))] string tag)
        {
            const string Template = @"
using System;

/// <summary>
/// Does something with <###""T""/>.
/// </summary>
public class TestMe<T> where T: class
{
    public void DoSomething(T t)
    { }
}
";

            VerifyCSharpFix(Template.Replace("###", tag), Template.Replace("###", "typeparamref name="));
        }

        protected override string GetDiagnosticId() => MiKo_2046_InvalidTypeParameterReferenceInXmlAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2046_InvalidTypeParameterReferenceInXmlAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2046_CodeFixProvider();
    }
}