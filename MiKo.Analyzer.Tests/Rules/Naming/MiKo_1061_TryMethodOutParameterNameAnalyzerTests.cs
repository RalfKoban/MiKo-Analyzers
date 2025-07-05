﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1061_TryMethodOutParameterNameAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_no_out_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_non_Try_method_with_an_out_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(out int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_Try_method_with_no_parameters() => No_issue_is_reported_for(@"

public class TestMe
{
    public void TryDoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_Try_method_with_no_out_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    public void TryDoSomething(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_Try_method_with_correctly_named_out_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    public void TryDoSomething(out int result) { }
}
");

        [Test]
        public void No_issue_is_reported_for_Try_method_with_multiple_out_parameters() => No_issue_is_reported_for(@"

public class TestMe
{
    public void TryDoSomething(out int someData, out int someOtherData) { }
}
");

        [Test]
        public void No_issue_is_reported_for_TryGet_method_with_correctly_named_out_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    public void TryGetMyOwnValue(out int myOwnValue) { }
}
");

        [Test]
        public void No_issue_is_reported_for_TryGet_method_with_out_parameter_named_value() => No_issue_is_reported_for(@"

public class TestMe
{
    public void TryGet(out int value) { }
}
");

        [Test]
        public void No_issue_is_reported_for_TryGet_method_with_multiple_out_parameters() => No_issue_is_reported_for(@"

public class TestMe
{
    public void TryGet(out int someData, out int someOtherData) { }
}
");

        [Test]
        public void An_issue_is_reported_for_Try_method_with_incorrectly_named_out_parameter() => An_issue_is_reported_for(@"

public class TestMe
{
    public void TryDoSomething(out int i) { }
}
");

        [Test]
        public void An_issue_is_reported_for_TryGet_method_with_out_parameter_named_result() => An_issue_is_reported_for(@"

public class TestMe
{
    public void TryGetMyOwnValue(out int result) { }
}
");

        [Test]
        public void An_issue_is_reported_for_TryGet_method_with_incorrectly_named_out_parameter() => An_issue_is_reported_for(@"

public class TestMe
{
    public void TryGetMyOwnValue(out int i) { }
}
");

        [TestCase("class TestMe { void TryGet(out int i) { } }", "class TestMe { void TryGet(out int value) { } }")]
        [TestCase("class TestMe { void TryGetWhatever(out int i) { } }", "class TestMe { void TryGetWhatever(out int whatever) { } }")]
        [TestCase("class TestMe { void TryParse(out int i) { } }", "class TestMe { void TryParse(out int result) { } }")]
        [TestCase("class TestMe { void TryGetObject(out object o) { } }", "class TestMe { void TryGetObject(out object result) { } }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1061_TryMethodOutParameterNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1061_TryMethodOutParameterNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1061_CodeFixProvider();
    }
}