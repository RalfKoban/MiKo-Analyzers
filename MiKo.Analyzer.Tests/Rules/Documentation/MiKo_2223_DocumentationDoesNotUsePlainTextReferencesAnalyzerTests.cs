using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public class MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_documented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_with_para() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// <para>
    /// Does something that is very important.
    /// </para>
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_with_see_cref() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something inside <see cref=""DoSomething""/> that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something inside SomeMethodName that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_with_para() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// <para>
    /// Does something inside SomeMethodName that is very important.
    /// </para>
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer();
    }
}