﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2231_InheritdocGetHashCodeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_method_GetHashCode() => No_issue_is_reported_for(@"
public class TestMe
{
    public override int GetHashCode()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_inheritdoc_documented_method_GetHashCode() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <inheritdoc/>
    public override int GetHashCode()
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_documented_method_GetHashCode() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>Some text</summary>
    public override int GetHashCode()
    { }
}
");

        [Test]
        public void Code_gets_fixed_for_documented_method_GetHashCode_on_same_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <summary>Some text</summary>
    /// <returns>Some value</returns>
    public override int GetHashCode()
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <inheritdoc/>
    public override int GetHashCode()
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documented_method_GetHashCode_on_different_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <summary>
    /// Some text
    /// </summary>
    /// <returns>
    /// Some value
    /// </returns>
    public override int GetHashCode()
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <inheritdoc/>
    public override int GetHashCode()
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2231_InheritdocGetHashCodeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2231_InheritdocGetHashCodeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2231_CodeFixProvider();
    }
}