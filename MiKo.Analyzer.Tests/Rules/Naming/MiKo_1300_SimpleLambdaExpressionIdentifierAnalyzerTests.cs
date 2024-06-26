using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
        [TestCase("____")]
        [TestCase("_0")]
        [TestCase("_1")]
        [TestCase("_2")]
        [TestCase("_3")]
        [TestCase("_4")]
        [TestCase("_5")]
        [TestCase("failed")] // result to indicate an error in ASP .NET Core
        public void No_issue_is_reported_for_correctly_named_simple_lambda_identifier_(string identifier) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        var items = new List<string>();
        if (items.Where(" + identifier + " => " + identifier + @" == null)
        {
        }
    }
}
");

        [TestCase("_")]
        [TestCase("__")]
        [TestCase("___")]
        [TestCase("____")]
        [TestCase("_0")]
        [TestCase("_1")]
        [TestCase("_2")]
        [TestCase("_3")]
        [TestCase("_4")]
        [TestCase("_5")]
        [TestCase("failed")] // result to indicate an error in ASP .NET Core
        public void No_issue_is_reported_for_correctly_named_parenthesized_lambda_identifier_(string identifier) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        var items = new List<string>();
        if (items.Where((" + identifier + ") => " + identifier + @" == null)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_parenthesized_lambda_identifiers() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        var items = new Dictionary<string, string>();
        if (items.Where((x, y) => x == null || y == null)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_simple_lambda_identifier() => An_issue_is_reported_for(@"
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

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_parenthesized_lambda_identifier() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        var items = new List<string>();
        if (items.Where((s) => s == null)
        {
        }
    }
}
");

        // TODO RKN: Test naming for nested lambdas with multiple parameters
        [TestCase("using System; using System.Collections.Generic; using System.Linq; class T { void D(List<string> items) { if (items.Any(#1# => #1# == null) { } } }")]
        [TestCase("using System; using System.Collections.Generic; using System.Linq; class T { void D(List<string> items) { if (items.Any((#1#) => #1# == null) { } } }")]
        [TestCase("using System; using System.Collections.Generic; using System.Linq; class T { void D(List<string> items) { if (items.Any(#1# => #1#.Length == 0 || #1#.Length == 1) { } } }")]
        [TestCase("using System; using System.Collections.Generic; using System.Linq; class T { void D(List<string> items) { if (items.Any((#1#) => #1#.Length == 0 || #1#.Length == 1) { } } }")]
        [TestCase("using System; using System.Collections.Generic; using System.Linq; class T { void D(List<string> items) { if (items.Any(#1# => #1#.Any(#2# => #2#.Equals('a'))) { } } }")]
        [TestCase("using System; using System.Collections.Generic; using System.Linq; class T { void D(List<string> items) { if (items.Any((#1#) => #1#.Any(#2# => #2#.Equals('a'))) { } } }")]
        [TestCase("using System; using System.Collections.Generic; using System.Linq; class T { void D(List<string> items) { void LocalFunction() { if (items.Any(#1# => #1#.Any(#2# => #2#.Equals('a'))) { } } } }")]
        [TestCase("using System; using System.Collections.Generic; using System.Linq; class T { void D(List<string> items) { void LocalFunction() { if (items.Any((#1#) => #1#.Any((#2#) => #2#.Equals('a'))) { } } } }")]
        [TestCase("using System; using System.Collections.Generic; using System.Linq; class T { void D(List<string> items) { bool LocalFunction(char value) { return (items.Any(#1# => #1#.Any(#2# => #2#.Equals(value))) { } } } }")]
        [TestCase("using System; using System.Collections.Generic; using System.Linq; class T { void D(List<string> items) { bool LocalFunction(char value) { return (items.Any((#1#) => #1#.Any((#2#) => #2#.Equals(value))) { } } } }")]
        public void Code_gets_fixed_(string template)
        {
            var originalCode = template.Replace("#1#", "item").Replace("#2#", "c");
            var fixedCode = template.Replace("#1#", "_").Replace("#2#", "__");

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1300_CodeFixProvider();
    }
}