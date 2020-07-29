﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public class MiKo_2011_UnsealedClassSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Struct_is_not_reported() => No_issue_is_reported_for(@"
/// <summary>
/// Something.
/// </summary>
public struct TestMe
{
}
");

        [Test]
        public void Sealed_public_class_is_not_reported() => No_issue_is_reported_for(@"
/// <summary>
/// This class cannot be inherited.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Sealed_non_public_class_is_not_reported() => No_issue_is_reported_for(@"
/// <summary>
/// Something.
/// </summary>
private sealed class TestMe
{
}
");

        [Test]
        public void No_documentation_is_not_reported() => No_issue_is_reported_for(@"
public sealed class TestMe
{
}
");

        [Test]
        public void Correct_documentation_is_not_reported() => No_issue_is_reported_for(@"
/// <summary>
/// Something.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void Wrong_start_placed_documentation_is_reported() => An_issue_is_reported_for(@"
/// <summary>
/// This class cannot be inherited.
/// Something.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void Wrong_end_placed_documentation_is_reported() => An_issue_is_reported_for(@"
/// <summary>
/// Something.
/// This class cannot be inherited.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void Wrong_documentation_is_not_reported_for_TestClass_([ValueSource(nameof(TestFixtures))] string testFixture) => No_issue_is_reported_for(@"
/// <summary>
/// Some documentation
/// This class cannot be inherited.
/// </summary>
[" + testFixture + @"]
public class TestMe
{
}
");

        [Test]
        public void Malformed_documentation_is_not_reported() => No_issue_is_reported_for(@"
/// <summary>
/// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
/// This class cannot be inherited.
/// </summary>
public class TestMe
{
}
");

        protected override string GetDiagnosticId() => MiKo_2011_UnsealedClassSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2011_UnsealedClassSummaryAnalyzer();
    }
}