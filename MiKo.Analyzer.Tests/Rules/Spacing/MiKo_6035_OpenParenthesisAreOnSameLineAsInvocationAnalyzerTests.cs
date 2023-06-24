﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6035_OpenParenthesisAreOnSameLineAsInvocationAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_complete_call_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestME
{
    public void DoSomething()
    {
        GC.Collect();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_invocation_and_parenthesis_are_on_different_lines() => An_issue_is_reported_for(@"
using System;

public class TestME
{
    public void DoSomething()
    {
        GC.Collect
                  ();
    }
}
");

        [Test]
        public void Code_gets_fixed_if_invocation_and_parenthesis_are_on_different_lines()
        {
            const string OriginalCode = @"
using System;

public class TestME
{
    public void DoSomething()
    {
        GC.Collect
                  ();
    }
}
";

            const string FixedCode = @"
using System;

public class TestME
{
    public void DoSomething()
    {
        GC.Collect();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6035_OpenParenthesisAreOnSameLineAsInvocationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6035_OpenParenthesisAreOnSameLineAsInvocationAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6035_CodeFixProvider();
    }
}