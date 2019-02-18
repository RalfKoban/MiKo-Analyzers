using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2046_InvalidTypeParameterReferenceInSummaryAnalyzerTests : CodeFixVerifier
    {
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
        public void No_issue_is_reported_for_correctly_documented_method_without_parameters() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_generic_method() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_non_generic_method() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_type() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_generic_method([Values("see", "seealso")] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something with <" + tag + @" cref=""T"" />.
    /// </summary>
    public void DoSomething<T>(T t) where T: class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_non_generic_method([Values("see", "seealso")] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe<T> where T : class
{
    /// <summary>
    /// Does something with <" + tag + @" cref=""T"" />.
    /// </summary>
    public void DoSomething(T t)
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_generic_type([Values("see", "seealso")] string tag) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Does something with <" + tag + @" cref=""T"" />.
/// </summary>
public class TestMe<T> where T: class
{
    public void DoSomething(T t)
    { }
}
");

        protected override string GetDiagnosticId() => MiKo_2046_InvalidTypeParameterReferenceInSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2046_InvalidTypeParameterReferenceInSummaryAnalyzer();
    }
}