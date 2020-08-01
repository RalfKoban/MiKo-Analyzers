﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_lambda() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [TestCase("_")]
        [TestCase("__")]
        [TestCase("___")]
        public void No_issue_is_reported_correctly_named_lambda_identifier_(string identifier) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        var items = new List<string>();
        if (items.Where(" + identifier + @" => " + identifier + @" == null)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_incorrectly_named_lambda_identifier() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        var items = new List<string>();
        if (items.Where(s => s == null)
        {
        }
    }
}
");

        [TestCase("using System; using System.Collections.Generic; using System.Linq; class T { void D(List<string> items) { if (items.Where(### => ### == null) { } } }")]
        [TestCase("using System; using System.Collections.Generic; using System.Linq; class T { void D(List<string> items) { if (items.Where(### => ###.Length == 0 || ###.Length == 1) { } } }")]
        public void Code_gets_fixed(string template)
        {
            var originalCode = template.Replace("###", "item");
            var fixedCode = template.Replace("###", "_");

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1300_CodeFixProvider();
    }
}