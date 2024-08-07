﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3073_CtorContainsReturnAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_void_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { return; }
}
");

        [Test]
        public void No_issue_is_reported_for_expression_body_ctor_without_return() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int field;

    public TestMe() => field = 42;
}
");

        [Test]
        public void No_issue_is_reported_for_method_body_ctor_without_return() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int field;

    public TestMe()
    {
        field = 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_body_ctor_with_return_inside_local_function() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int field;

    public TestMe()
    {
        field = Calculate();

        int Calculate()
        {
            return 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_body_ctor_with_return_inside_parameterized_lambda_callback() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private Func<int> field;

    public TestMe(int value)
    {
        field = () =>
                        {
                            var result = new Random().Next(1, value);

                            return result;
                        };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_body_ctor_with_return_inside_non_parameterized_lambda_callback() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private Func<int, int> field;

    public TestMe(int value)
    {
        field = i =>
                    {
                        var result = new Random().Next(i, value);

                        return result;
                    };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_primary_ctor_and_method_with_return_statement_([Values("class", "record", "struct")] string type) => No_issue_is_reported_for(@"
using System;

public " + type + @" TestMe(int field)
{
    public int Calculate()
    {
        return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_body_ctor_with_return() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int field;

    public TestMe(int value)
    {
        if (value == 42)
            return;

        field = value;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3073_CtorContainsReturnAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3073_CtorContainsReturnAnalyzer();
    }
}