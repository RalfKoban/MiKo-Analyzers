using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1019_ClearRemoveMethodsAnalyzerTests : CodeFixVerifier
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

        [Test]
        public void No_issue_is_reported_for_test_methods()
        {
            Assert.Multiple(() =>
                                {
                                    foreach (var test in Tests)
                                    {
                                        No_issue_is_reported_for(@"
using System;
using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
    public void Clear(int i)
    {
    }

    [" + test + @"]
    public void Remove()
    {
    }
}
");
                                    }
                                });
        }

        [Test]
        public void No_issue_is_reported_for_Clears_method_with_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void ClearsSomething(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Removes_method_with_no_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void RemovesSomething()
    {
    }
}
");

        [TestCase("Clear(int i)", "Remove(int i)")]
        [TestCase("ClearAll(int i)", "RemoveAll(int i)")]
        [TestCase("Remove()", "Clear()")]
        [TestCase("RemoveAll()", "ClearAll()")]
        public void Code_gets_fixed_(string method, string wanted) => VerifyCSharpFix(
                                                                                      @"using System; class TestMe { void " + method + " { } }",
                                                                                      @"using System; class TestMe { void " + wanted + " { } }");

        protected override string GetDiagnosticId() => MiKo_1019_ClearRemoveMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1019_ClearRemoveMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1019_CodeFixProvider();
    }
}