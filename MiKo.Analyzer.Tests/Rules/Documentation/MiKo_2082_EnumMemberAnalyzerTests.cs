using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
        public void An_issue_is_reported_for_empty_commented_enum_member() => An_issue_is_reported_for(@"
using System;

public enum TestMe
{
    /// <summary>
    /// Nothing to do.
    /// </summary>
    None = 0,

    /// <summary>
    /// 
    /// </summary>
    Something = 1,
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_enum_member_([Values("Defines", "Indicates", "Represents", "Specifies", "Enum")] string startingPhrase) => An_issue_is_reported_for(@"
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

        [Test, Combinatorial]
        public void Code_gets_fixed_for_incorrectly_commented_enum_member_(
                                                                       [Values("Defines", "Indicates", "Represents", "Specifies", "Enum")] string startingWord,
                                                                       [Values("", " that", ", that", " whether", ", whether", " for", ", for")] string continuation)
        {
            const string Template = @"
using System;

public enum TestMe
{
    /// <summary>
    /// Nothing to do.
    /// </summary>
    None = 0,

    /// <summary>
    /// ###
    /// </summary>
    Something = 1,
}
";

            VerifyCSharpFix(Template.Replace("###", startingWord + continuation + " something to do."), Template.Replace("###", "Something to do."));
        }

        [Test]
        public void Code_gets_fixed_for_empty_commented_enum_member()
        {
            const string OriginalCode = @"
using System;

public enum TestMe
{
    /// <summary>
    /// Nothing to do.
    /// </summary>
    None = 0,

    /// <summary>
    /// 
    /// </summary>
    Something = 1,
}
";

            VerifyCSharpFix(OriginalCode, OriginalCode);
        }

        protected override string GetDiagnosticId() => MiKo_2082_EnumMemberAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2082_EnumMemberAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2082_CodeFixProvider();
    }
}