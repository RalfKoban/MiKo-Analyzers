using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2045_InvalidParameterReferenceInSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i) { }
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
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">The parameter</param>
    public void DoSomething(int i) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method([Values("param", "paramref")] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something with <" + tag + @" name=""i"" />.
    /// </summary>
    public void DoSomething(int i) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_with_apostrophes([Values("'i'", "\"i\"")] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something with " + tag + @".
    /// </summary>
    public void DoSomething(int i) { }
}
");

        protected override string GetDiagnosticId() => MiKo_2045_InvalidParameterReferenceInSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2045_InvalidParameterReferenceInSummaryAnalyzer();
    }
}