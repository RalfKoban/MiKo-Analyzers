﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2302_CommentedOutCodeAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Comments =
            {
                "var x = 42;",
                "string s = x.ToString();",
                "if (i == 42) ",
                "switch (expression)",
                "case 0815:",
            };

        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_method() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // some comment that is long enough
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_commented_out_code_in_method([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2302_CommentedOutCodeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2302_CommentedOutCodeAnalyzer();
    }
}