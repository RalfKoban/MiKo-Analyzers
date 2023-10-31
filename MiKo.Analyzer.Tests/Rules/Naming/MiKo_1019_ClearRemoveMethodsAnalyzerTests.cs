using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
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

        [Test]
        public void No_issue_is_reported_for_method_with_non_Clear_Remove_local_function() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        void Something() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Clear_local_function_with_no_parameters_and_Remove_local_function_with_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        void Clear()
        {
        }

        void Remove(int i)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Clear_local_function_with_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        void Clear(int i)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Remove_local_function_with_no_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        void Remove()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_functions_in_test_methods()
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
    public void DoSomething1()
    {
        void Clear(int i)
        {
        }
    }

    [" + test + @"]
    public void DoSomething2()
     {
        void Remove()
        {
        }
    }
}
");
                                     }
                                 });
        }

        [Test]
        public void No_issue_is_reported_for_Clears_local_function_with_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        void ClearsSomething(int i)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Removes_local_function_with_no_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        void RemovesSomething()
        {
        }
    }
}
");

        [TestCase("Clear(int i)", "Remove(int i)")]
        [TestCase("ClearAll(int i)", "RemoveAll(int i)")]
        [TestCase("Remove()", "Clear()")]
        [TestCase("RemoveAll()", "ClearAll()")]
        public void Code_gets_fixed_for_method_(string method, string wanted) => VerifyCSharpFix(
                                                                                      "using System; class TestMe { void " + method + " { } }",
                                                                                      "using System; class TestMe { void " + wanted + " { } }");

        [TestCase("Clear(int i)", "Remove(int i)")]
        [TestCase("ClearAll(int i)", "RemoveAll(int i)")]
        [TestCase("Remove()", "Clear()")]
        [TestCase("RemoveAll()", "ClearAll()")]
        public void Code_gets_fixed_for_local_function_(string method, string wanted) => VerifyCSharpFix(
                                                                                      "using System; class TestMe { public void DoSomething() { void " + method + " { } } }",
                                                                                      "using System; class TestMe { public void DoSomething() { void " + wanted + " { } } }");

        protected override string GetDiagnosticId() => MiKo_1019_ClearRemoveMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1019_ClearRemoveMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1019_CodeFixProvider();
    }
}