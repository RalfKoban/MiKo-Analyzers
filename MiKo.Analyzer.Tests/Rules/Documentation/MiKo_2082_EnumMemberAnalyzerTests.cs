using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2082_EnumMemberAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_field_of_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Indicates something.
    /// </summary>
    private bool SomeFlag;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_enum_member() => No_issue_is_reported_for(@"
using System;

public enum TestMe
{
    None = 0,
    Something = 1,
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_enum_member() => No_issue_is_reported_for(@"
using System;

public enum TestMe
{
    /// <summary>
    /// Nothing to do.
    /// </summary>
    None = 0,

    /// <summary>
    /// There is something to do.
    /// </summary>
    Something = 1,
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_enum_member_([Values("Defines", "Indicates", "Specifies")] string startingPhrase) => An_issue_is_reported_for(@"
using System;

public enum TestMe
{
    /// <summary>
    /// Nothing to do.
    /// </summary>
    None = 0,

    /// <summary>
    /// " + startingPhrase + @" something to do.
    /// </summary>
    Something = 1,
}
");

        protected override string GetDiagnosticId() => MiKo_2082_EnumMemberAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2082_EnumMemberAnalyzer();
    }
}