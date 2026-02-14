using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2083_NonReadOnlyFieldAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Visibilities = ["protected", "public", "private", "internal"];

        [Test]
        public void No_issue_is_reported_for_undocumented_readonly_field_with_visibility_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    " + visibility + @" readonly string m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_non_readonly_field_with_visibility_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    " + visibility + @" string m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_readonly_field_with_readonly_comment_with_visibility_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// The field.
    /// This field is read-only.
    /// </summary>
    " + visibility + @" readonly string m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_non_readonly_field_with_readonly_comment_with_visibility_([ValueSource(nameof(Visibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// The field.
    /// This field is read-only.
    /// </summary>
    " + visibility + @" string m_field;
}
");

        [Test]
        public void Code_gets_fixed_to_remove_readonly_comment()
        {
            const string OriginalCode = @"
/// <summary>
/// Some text
/// </summary>
public sealed class TestMe
{
    /// <summary>
    /// Bla bla bla.
    /// This field is read-only.
    /// </summary>
    private string m_field;
}
";

            const string FixedCode = @"
/// <summary>
/// Some text
/// </summary>
public sealed class TestMe
{
    /// <summary>
    /// Bla bla bla.
    /// </summary>
    private string m_field;
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2083_NonReadOnlyFieldAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2083_NonReadOnlyFieldAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2083_CodeFixProvider();
    }
}