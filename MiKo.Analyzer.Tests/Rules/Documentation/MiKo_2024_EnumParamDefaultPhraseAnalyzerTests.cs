using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2024_EnumParamDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectStartingPhrases =
                                                                  [
                                                                      "One of the enumeration members specifying",
                                                                      "One of the enumeration members that specifies",
                                                                      "One of the enumeration members that determines",
                                                                      "One of the enumeration members that defines",
                                                                      "One of the enumeration values specifying",
                                                                      "One of the enumeration values that specifies",
                                                                      "One of the enumeration values that determines",
                                                                      "One of the enumeration values that defines",
                                                                  ];

        private static readonly string[] CorrectFlagsStartingPhrases =
                                                                       [
                                                                           "A bitwise combination of enumeration values that",
                                                                           "A bitwise combination of the enumeration values that",
                                                                       ];

        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object o) { }
}
");

        [TestCase("bool")]
        [TestCase("System.Boolean")]
        [TestCase(nameof(Boolean))]
        [TestCase(nameof(Object))]
        public void No_issue_is_reported_for_method_with_(string type) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever</param>
    public void DoSomething(" + type + @" o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_out_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever</param>
    public void DoSomething(out StringComparison o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_correct_comment_(
                                                                      [ValueSource(nameof(CorrectStartingPhrases))] string comment,
                                                                      [Values("something", "something.")] string continuation)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + " " + continuation + @"</param>
    public void DoSomething(StringComparison o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_correct_bitmask_comment_([ValueSource(nameof(CorrectFlagsStartingPhrases))] string comment) => No_issue_is_reported_for(@"
using System;
using System.Globalization;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @" does something.</param>
    public void DoSomething(NumberStyles o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_with_correct_comment_(
                                                                    [ValueSource(nameof(CorrectStartingPhrases))] string comment,
                                                                    [Values("something", "something.")] string continuation)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + " " + continuation + @"</param>
    public TestMe(StringComparison o) { }
}
");

        [TestCase("Unused")]
        [TestCase("Unused.")]
        public void No_issue_is_reported_for_method_with_correct_unused_comment_(string comment) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(StringComparison o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_correct_type_comment_(
                                                                           [Values(
                                                                               """A <see cref="StringComparison" /> value""",
                                                                               """A <see cref="StringComparison"/> value""",
                                                                               """A <see cref="System.StringComparison" /> value""",
                                                                               """A <see cref="System.StringComparison"/> value""",
                                                                               """An <see cref="StringComparison" /> value""",
                                                                               """An <see cref="StringComparison"/> value""",
                                                                               """An <see cref="System.StringComparison" /> value""",
                                                                               """An <see cref="System.StringComparison"/> value""",
                                                                               """One of the <see cref="StringComparison" /> enumeration members""",
                                                                               """One of the <see cref="StringComparison" /> enumeration values""",
                                                                               """One of the <see cref="StringComparison" /> members""",
                                                                               """One of the <see cref="StringComparison" /> values""",
                                                                               """One of the <see cref="StringComparison"/> enumeration members""",
                                                                               """One of the <see cref="StringComparison"/> enumeration values""",
                                                                               """One of the <see cref="StringComparison"/> members""",
                                                                               """One of the <see cref="StringComparison"/> values""",
                                                                               """The <see cref="StringComparison" /> value""",
                                                                               """The <see cref="StringComparison"/> value""",
                                                                               """The <see cref="System.StringComparison" /> value""",
                                                                               """The <see cref="System.StringComparison"/> value""")] string comment,
                                                                           [Values("that specifies", "specifying", "that defines", "defining")] string continuation)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + " " + continuation + @" something.</param>
    public void DoSomething(StringComparison o) { }
}
");

        [TestCase("whatever.")]
        [TestCase("Whatever.")]
        [TestCase("A bitwise combination of enumeration values that does something")]
        [TestCase("A bitwise combination of the enumeration values that does something")]
        public void An_issue_is_reported_for_method_with_wrong_comment_phrase_(string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(StringComparison o) { }
}
");

        [TestCase("whatever.")]
        [TestCase("Whatever.")]
        public void An_issue_is_reported_for_ctor_with_wrong_comment_phrase_(string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public TestMe(StringComparison o) { }
}
");

        [TestCase("<summary />")]
        [TestCase("<inheritdoc />")]
        [TestCase("<exclude />")]
        public void No_issue_is_reported_for_method_with_missing_documentation_(string xmlElement) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// " + xmlElement + @"
    public void DoSomething(StringComparison o) { }
}
");

        [Test]
        public void Code_gets_fixed_for_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever it is.</param>
    public void DoSomething(StringComparison o) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// One of the enumeration members that specifies whatever it is.
    /// </param>
    public void DoSomething(StringComparison o) { }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ctor()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever it is.</param>
    public TestMe(StringComparison o) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// One of the enumeration members that specifies whatever it is.
    /// </param>
    public TestMe(StringComparison o) { }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_on_different_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// Whatever it is.
    /// </param>
    public void DoSomething(StringComparison o) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// One of the enumeration members that specifies whatever it is.
    /// </param>
    public void DoSomething(StringComparison o) { }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_inside_para_tag()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// <para>
    /// Whatever it is.
    /// </para>
    /// </param>
    public void DoSomething(StringComparison o) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// <para>
    /// One of the enumeration members that specifies whatever it is.
    /// </para>
    /// </param>
    public void DoSomething(StringComparison o) { }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("Specifies")]
        [TestCase("A value specifying")]
        [TestCase("A value that specifies")]
        [TestCase("A value which specifies")]
        [TestCase("An value specifying")]
        [TestCase("An value that specifies")]
        [TestCase("An value which specifies")]
        [TestCase("The value specifying")]
        [TestCase("The value that specifies")]
        [TestCase("The value which specifies")]
        [TestCase("One of the values which specifies")]
        [TestCase("One of the enumeration members which specifies")]
        [TestCase("One of the enumeration values which specifies")]
        [TestCase("Determines")]
        [TestCase("A value determining")]
        [TestCase("A value that determines")]
        [TestCase("A value which determines")]
        [TestCase("An value determining")]
        [TestCase("An value that determines")]
        [TestCase("An value which determines")]
        [TestCase("The value determining")]
        [TestCase("The value that determines")]
        [TestCase("The value which determines")]
        [TestCase("One of the values which determines")]
        [TestCase("One of the enumeration members which determines")]
        [TestCase("One of the enumeration values which determines")]
        [TestCase("Value indicating")]
        [TestCase("Value for")]
        [TestCase("Indicator for")]
        [TestCase("Enum for")]
        [TestCase("enum indicating")]
        [TestCase("Enum indicating")]
        [TestCase("enum that indicates")]
        [TestCase("Enum that indicates")]
        [TestCase("enum which indicates")]
        [TestCase("Enum which indicates")]
        public void Code_gets_fixed_for_phrase_(string originalPhrase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// " + originalPhrase + @" whatever it is.
    /// </param>
    public void DoSomething(StringComparison o) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// One of the enumeration members that specifies whatever it is.
    /// </param>
    public void DoSomething(StringComparison o) { }
}
";
            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase("The minimum expected C# language version.", "the minimum expected C# language version.")]
        public void Code_gets_fixed_for_phrase_(string originalPhrase, string fixedPhrase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// " + originalPhrase + @"
    /// </param>
    public void DoSomething(StringComparison o) { }
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// One of the enumeration members that specifies " + fixedPhrase + @"
    /// </param>
    public void DoSomething(StringComparison o) { }
}
";
            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("Specifies", "specifies")]
        [TestCase("A value specifying", "specifies")]
        [TestCase("A value that specifies", "specifies")]
        [TestCase("A value which specifies", "specifies")]
        [TestCase("An value specifying", "specifies")]
        [TestCase("An value that specifies", "specifies")]
        [TestCase("An value which specifies", "specifies")]
        [TestCase("The value specifying", "specifies")]
        [TestCase("The value that specifies", "specifies")]
        [TestCase("The value which specifies", "specifies")]
        [TestCase("One of the values which specifies", "specifies")]
        [TestCase("One of the enumeration members that specifies", "specifies")]
        [TestCase("One of the enumeration members which specifies", "specifies")]
        [TestCase("One of the enumeration values that specifies", "specifies")]
        [TestCase("One of the enumeration values which specifies", "specifies")]
        [TestCase("Determines", "determines")]
        [TestCase("A value determining", "determines")]
        [TestCase("A value that determines", "determines")]
        [TestCase("A value which determines", "determines")]
        [TestCase("An value determining", "determines")]
        [TestCase("An value that determines", "determines")]
        [TestCase("An value which determines", "determines")]
        [TestCase("The value determining", "determines")]
        [TestCase("The value that determines", "determines")]
        [TestCase("The value which determines", "determines")]
        [TestCase("One of the values which determines", "determines")]
        [TestCase("One of the enumeration members that determines", "determines")]
        [TestCase("One of the enumeration members which determines", "determines")]
        [TestCase("One of the enumeration values that determines", "determines")]
        [TestCase("One of the enumeration values which determines", "determines")]
        [TestCase("Value indicating", "indicates")]
        [TestCase("Value for", "specifies")]
        [TestCase("Indicator for", "indicates")]
        [TestCase("Enum for", "specifies")]
        [TestCase("enum indicating", "indicates")]
        [TestCase("Enum indicating", "indicates")]
        [TestCase("enum that indicates", "indicates")]
        [TestCase("Enum that indicates", "indicates")]
        [TestCase("enum which indicates", "indicates")]
        [TestCase("Enum which indicates", "indicates")]
        [TestCase("A bitmask representing", "represents")]
        [TestCase("Bitmask representing", "represents")]
        [TestCase("Bitwise combination of values that specify", "specifies")]
        [TestCase("Bitwise combination of values that specifies", "specifies")]
        [TestCase("Bitwise combination of the values that specify", "specifies")]
        [TestCase("Bitwise combination of the values that specifies", "specifies")]
        [TestCase("A", "specifies a")]
        [TestCase("An", "specifies an")]
        [TestCase("The", "specifies the")]
        public void Code_gets_fixed_for_flags_phrase_(string originalPhrase, string fixedContinuation)
        {
            var originalCode = @"
using System;
using System.Globalization;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// " + originalPhrase + @" whatever it is.
    /// </param>
    public void DoSomething(NumberStyles o) { }
}
";

            var fixedCode = @"
using System;
using System.Globalization;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// A bitwise combination of enumeration values that " + fixedContinuation + @" whatever it is.
    /// </param>
    public void DoSomething(NumberStyles o) { }
}
";
            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2024_EnumParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2024_EnumParamDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2024_CodeFixProvider();
    }
}