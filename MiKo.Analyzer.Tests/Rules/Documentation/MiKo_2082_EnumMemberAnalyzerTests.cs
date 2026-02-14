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
        public void No_issue_is_reported_for_undocumented_enum_member() => No_issue_is_reported_for(@"
using System;

public enum TestMe
{
    None = 0,
    Something = 1,
}
");

        [Test]
        public void No_issue_is_reported_for_enum_member_without_redundant_starting_phrase() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_enum_member_with_empty_summary() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_enum_member_starting_with_redundant_phrase_([Values("Defines", "Indicates", "Represents", "Specifies", "Enum")] string startingPhrase) => An_issue_is_reported_for(@"
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

        [Test]
        public void An_issue_is_reported_for_enum_member_starting_with_see_cref() => An_issue_is_reported_for(@"
using System;

public enum TestMe
{
    /// <summary>
    /// Nothing to do.
    /// </summary>
    None = 0,

    /// <summary>
    /// <see cref=""TestMe""/> something to do.
    /// </summary>
    Something = 1,
}
");

        [Test, Combinatorial]
        public void Code_gets_fixed_for_enum_member_starting_with_redundant_phrase_(
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
        public void Code_gets_fixed_for_enum_member_with_empty_summary()
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

        [Test]
        public void Code_gets_fixed_for_enum_member_with_Enum_for_pattern()
        {
            const string OriginalCode = @"
using System;

public enum TestMeKind
{
    /// <summary>
    /// Enum WhateverIdentifierEnum for WhateverIdentifier
    /// </summary>
    WhateverIdentifier = 0,
}
";

            const string FixedCode = @"
using System;

public enum TestMeKind
{
    /// <summary>
    /// The test me is a whateverIdentifier.
    /// </summary>
    WhateverIdentifier = 0,
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_enum_member_with_Enum_for_pattern_when_member_is_suffixed_with_Enum()
        {
            const string OriginalCode = @"
using System;

public enum TestMeKind
{
    /// <summary>
    /// Enum WhateverIdentifierEnum for WhateverIdentifier
    /// </summary>
    WhateverIdentifierEnum = 0,
}
";

            const string FixedCode = @"
using System;

public enum TestMeKind
{
    /// <summary>
    /// The test me is a whateverIdentifier.
    /// </summary>
    WhateverIdentifierEnum = 0,
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("TypeEnum")]
        [TestCase("TypesEnum")]
        [TestCase("KindsEnum")]
        public void Code_gets_fixed_for_enum_member_with_Enum_for_pattern_when_type_is_suffixed_with_(string suffix)
        {
            var originalCode = @"
using System;

public enum TestMe" + suffix + @"
{
    /// <summary>
    /// Enum WhateverIdentifierEnum for WhateverIdentifier
    /// </summary>
    WhateverIdentifierEnum = 0,
}
";

            var fixedCode = @"
using System;

public enum TestMe" + suffix + @"
{
    /// <summary>
    /// The test me is a whateverIdentifier.
    /// </summary>
    WhateverIdentifierEnum = 0,
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("On")]
        [TestCase("Off")]
        [TestCase("Undefined")]
        [TestCase("None")]
        [TestCase("Initiated")]
        [TestCase("Running")]
        [TestCase("Completed")]
        [TestCase("Canceled")]
        [TestCase("Failed")]
        public void Code_gets_fixed_for_enum_member_with_Enum_for_pattern_for_state_(string phrase1)
        {
            var phrase2 = phrase1.ToLowerCaseAt(0);

            const string OriginalCode = @"
public enum MessageType
{
    /// <summary>
    /// Enum #1#Enum for #2#
    /// </summary>
    #1#Enum = 0,

";

            const string FixedCode = @"
public enum MessageType
{
    /// <summary>
    /// The message is #2#.
    /// </summary>
    #1#Enum = 0,

";

            VerifyCSharpFix(OriginalCode.Replace("#1#", phrase1).Replace("#2#", phrase2), FixedCode.Replace("#1#", phrase1).Replace("#2#", phrase2));
        }

        [TestCase("Information", "an information")]
        [TestCase("Warning", "a warning")]
        [TestCase("Error", "an error")]
        [TestCase("Exception", "an exception")]
        public void Code_gets_fixed_for_enum_member_with_Enum_for_pattern_for_message_(string phrase, string fixedPhrase)
        {
            var phrase2 = phrase.ToLowerCaseAt(0);

            const string OriginalCode = @"
public enum MessageType
{
    /// <summary>
    /// Enum #1#Enum for #2#
    /// </summary>
    #1#Enum = 0,

";

            const string FixedCode = @"
public enum MessageType
{
    /// <summary>
    /// The message is #2#.
    /// </summary>
    #1#Enum = 0,

";

            VerifyCSharpFix(OriginalCode.Replace("#1#", phrase).Replace("#2#", phrase2), FixedCode.Replace("#1#", phrase).Replace("#2#", fixedPhrase));
        }

        [Test]
        public void Code_gets_fixed_for_enum_member_with_Enum_for_pattern_for_plural()
        {
            const string OriginalCode = @"
public enum ItemsType
{
    /// <summary>
    /// Enum MessagesEnum for messages
    /// </summary>
    MessagesEnum = 0,

";

            const string FixedCode = @"
public enum ItemsType
{
    /// <summary>
    /// The items are messages.
    /// </summary>
    MessagesEnum = 0,

";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2082_EnumMemberAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2082_EnumMemberAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2082_CodeFixProvider();
    }
}