using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2081_ReadOnlyFieldAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_readonly_field_with_visibility([Values("protected", "public", "private", "internal")] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    " + visibility + @" string m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_commented_readonly_field_with_visibility([Values("private", "internal")] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// The field.
    /// </summary>
    " + visibility + @" readonly string m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_readonly_field_with_visibility([Values("protected", "public")] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// The field.
    /// </summary>
    " + visibility + @" readonly string m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_readonly_field_with_visibility([Values("protected", "public")] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Bla bla bla. This field is read-only.
    /// </summary>
    " + visibility + @" readonly string m_field;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_incorrectly_commented_readonly_TestClass_field_with_visibility(
                                                                                                        [Values("protected", "public")] string visibility,
                                                                                                        [ValueSource(nameof(TestFixtures))] string testFixture)
            => No_issue_is_reported_for(@"
using System;

[ " + testFixture + @"]
public class TestMe
{
    /// <summary>
    /// Bla bla bla.
    /// </summary>
    " + visibility + @" readonly string m_field;
}
");

        protected override string GetDiagnosticId() => MiKo_2081_ReadOnlyFieldAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2081_ReadOnlyFieldAnalyzer();
    }
}