using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3029_EventRegistrationOrderAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_add_and_subtract_of_numbers() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int result = 0;
        int value1 = 1;
        int value2 = 2;

        result += value1;
        result -= value2;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_single_add_only_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.CancelKeyPress += OnCancelKeyPress;
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_add_only_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.CancelKeyPress += OnCancelKeyPress;
        Console.CancelKeyPress += OnCancelKeyPress;
        Console.CancelKeyPress += OnCancelKeyPress;
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_single_remove_only_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.CancelKeyPress -= OnCancelKeyPress;
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_remove_only_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.CancelKeyPress -= OnCancelKeyPress;
        Console.CancelKeyPress -= OnCancelKeyPress;
        Console.CancelKeyPress -= OnCancelKeyPress;
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_differences_in_add_and_remove() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.CancelKeyPress += OnCancelKeyPress;
        Console.CancelKeyPress += OnCancelKeyPress;
        Console.CancelKeyPress -= OnCancelKeyPress;
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_more_adds_than_removes_using_parenthesized_lambda_expressions() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.CancelKeyPress += (sender, args) => OnCancelKeyPress(sender, args);
        Console.CancelKeyPress += (sender, args) => OnCancelKeyPress(sender, args);
        Console.CancelKeyPress -= (sender, args) => OnCancelKeyPress(sender, args);
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_less_adds_than_removes_using_parenthesized_lambda_expressions() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.CancelKeyPress += (sender, args) => OnCancelKeyPress(sender, args);
        Console.CancelKeyPress -= (sender, args) => OnCancelKeyPress(sender, args);
        Console.CancelKeyPress -= (sender, args) => OnCancelKeyPress(sender, args);
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_same_add_and_remove_amount_using_parenthesized_lambda_expressions() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.CancelKeyPress += (sender, args) => OnCancelKeyPress(sender, args);
        Console.CancelKeyPress -= (sender, args) => OnCancelKeyPress(sender, args);
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_single_add_using_parenthesized_lambda_expressions() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.CancelKeyPress += (sender, args) => OnCancelKeyPress(sender, args);
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_single_remove_using_parenthesized_lambda_expressions() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.CancelKeyPress -= (sender, args) => OnCancelKeyPress(sender, args);
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expressions_in_unit_test() => No_issue_is_reported_for(@"
using System;
using NUnit.Framework;

public class TestMe
{
    [OneTimeSetUp]
    public void PrepareTestEnvironment()
    {
        Console.CancelKeyPress += (sender, args) => OnCancelKeyPress(sender, args);
    }

    [OneTimeTearDown]
    public void CleanupTestEnvironment()
    {
        Console.CancelKeyPress -= (sender, args) => OnCancelKeyPress(sender, args);
    }

    [SetUp]
    public void PrepareTest()
    {
        Console.CancelKeyPress += (sender, args) => OnCancelKeyPress(sender, args);
    }

    [TearDown]
    public void CleanupTest()
    {
        Console.CancelKeyPress -= (sender, args) => OnCancelKeyPress(sender, args);
    }

    [Test]
    public void DoSomething()
    {
        Console.CancelKeyPress -= (sender, args) => OnCancelKeyPress(sender, args);
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3029_EventRegistrationOrderAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3029_EventRegistrationOrderAnalyzer();
    }
}