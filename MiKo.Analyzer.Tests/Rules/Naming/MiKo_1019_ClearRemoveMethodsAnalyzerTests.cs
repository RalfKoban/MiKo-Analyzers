using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public class MiKo_1019_ClearRemoveMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_Clear_Remove_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
");

        [Test]
        public void No_issue_is_reported_for_Clear_method_with_no_parameters_and_Remove_method_with_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Clear()
    {
    }

    public void Remove(int i)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Clear_method_with_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Clear(int i)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Remove_method_with_no_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Remove()
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1019_ClearRemoveMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1019_ClearRemoveMethodsAnalyzer();
    }
}