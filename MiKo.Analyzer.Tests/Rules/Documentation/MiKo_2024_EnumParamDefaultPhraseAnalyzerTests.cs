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

        [TestCase("One of the enumeration members specifying something.")]
        [TestCase("One of the enumeration members that specifies something.")]
        [TestCase("One of the enumeration members that specifies something")]
        [TestCase("One of the enumeration members that determines something.")]
        [TestCase("One of the enumeration members that determines something")]
        [TestCase("One of the enumeration values specifying something.")]
        [TestCase("One of the enumeration values that specifies something.")]
        [TestCase("One of the enumeration values that specifies something")]
        [TestCase("One of the enumeration values that determines something.")]
        [TestCase("One of the enumeration values that determines something")]
        public void No_issue_is_reported_for_method_with_correct_comment_(string comment) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(StringComparison o) { }
}
");

        [TestCase("One of the enumeration members specifying something.")]
        [TestCase("One of the enumeration members that specifies something.")]
        [TestCase("One of the enumeration members that specifies something")]
        [TestCase("One of the enumeration members that determines something.")]
        [TestCase("One of the enumeration members that determines something")]
        [TestCase("One of the enumeration values specifying something.")]
        [TestCase("One of the enumeration values that specifies something.")]
        [TestCase("One of the enumeration values that specifies something")]
        [TestCase("One of the enumeration values that determines something.")]
        [TestCase("One of the enumeration values that determines something")]
        public void No_issue_is_reported_for_ctor_with_correct_comment_(string comment) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
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

        [TestCase(@"A <see cref=""StringComparison"" /> value specifying something.")]
        [TestCase(@"A <see cref=""StringComparison"" /> value that specifies something.")]
        [TestCase(@"A <see cref=""StringComparison""/> value specifying something.")]
        [TestCase(@"A <see cref=""StringComparison""/> value that specifies something.")]
        [TestCase(@"A <see cref=""System.StringComparison"" /> value specifying something.")]
        [TestCase(@"A <see cref=""System.StringComparison"" /> value that specifies something.")]
        [TestCase(@"A <see cref=""System.StringComparison""/> value specifying something.")]
        [TestCase(@"A <see cref=""System.StringComparison""/> value that specifies something.")]
        [TestCase(@"An <see cref=""StringComparison"" /> value specifying something.")]
        [TestCase(@"An <see cref=""StringComparison"" /> value that specifies something.")]
        [TestCase(@"An <see cref=""StringComparison""/> value specifying something.")]
        [TestCase(@"An <see cref=""StringComparison""/> value that specifies something.")]
        [TestCase(@"An <see cref=""System.StringComparison"" /> value specifying something.")]
        [TestCase(@"An <see cref=""System.StringComparison"" /> value that specifies something.")]
        [TestCase(@"An <see cref=""System.StringComparison""/> value specifying something.")]
        [TestCase(@"An <see cref=""System.StringComparison""/> value that specifies something.")]
        [TestCase(@"The <see cref=""StringComparison"" /> value specifying something.")]
        [TestCase(@"The <see cref=""StringComparison"" /> value that specifies something.")]
        [TestCase(@"The <see cref=""StringComparison""/> value specifying something.")]
        [TestCase(@"The <see cref=""StringComparison""/> value that specifies something.")]
        [TestCase(@"One of the <see cref=""StringComparison"" /> values specifying something.")]
        [TestCase(@"One of the <see cref=""StringComparison"" /> values that specifies something.")]
        [TestCase(@"One of the <see cref=""StringComparison""/> values specifying something.")]
        [TestCase(@"One of the <see cref=""StringComparison""/> values that specifies something.")]
        [TestCase(@"One of the <see cref=""StringComparison"" /> members specifying something.")]
        [TestCase(@"One of the <see cref=""StringComparison"" /> members that specifies something.")]
        [TestCase(@"One of the <see cref=""StringComparison""/> members specifying something.")]
        [TestCase(@"One of the <see cref=""StringComparison""/> members that specifies something.")]
        [TestCase(@"One of the <see cref=""StringComparison"" /> enumeration values specifying something.")]
        [TestCase(@"One of the <see cref=""StringComparison"" /> enumeration values that specifies something.")]
        [TestCase(@"One of the <see cref=""StringComparison""/> enumeration values specifying something.")]
        [TestCase(@"One of the <see cref=""StringComparison""/> enumeration values that specifies something.")]
        [TestCase(@"One of the <see cref=""StringComparison"" /> enumeration members specifying something.")]
        [TestCase(@"One of the <see cref=""StringComparison"" /> enumeration members that specifies something.")]
        [TestCase(@"One of the <see cref=""StringComparison""/> enumeration members specifying something.")]
        [TestCase(@"One of the <see cref=""StringComparison""/> enumeration members that specifies something.")]
        public void No_issue_is_reported_for_method_with_correct_type_comment_(string comment) => No_issue_is_reported_for(@"
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
using System.Threading;

public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever it is.</param>
    public void DoSomething(StringComparison o) { }
}
";

            const string FixedCode = @"
using System;
using System.Threading;

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
using System.Threading;

public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever it is.</param>
    public TestMe(StringComparison o) { }
}
";

            const string FixedCode = @"
using System;
using System.Threading;

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
using System.Threading;

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
using System.Threading;

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
using System.Threading;

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
using System.Threading;

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
        [TestCase("Indicator for")]
        [TestCase("Enum for")]
        [TestCase("enum indicating")]
        [TestCase("Enum indicating")]
        [TestCase("enum that indicates")]
        [TestCase("Enum that indicates")]
        [TestCase("enum which indicates")]
        [TestCase("Enum which indicates")]
        public void Code_gets_fixed_for_phrase_(string phrase)
        {
            var originalCode = @"
using System;
using System.Threading;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// " + phrase + @" whatever it is.
    /// </param>
    public void DoSomething(StringComparison o) { }
}
";

            const string FixedCode = @"
using System;
using System.Threading;

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

        //// TODO RKN: What about [Flags] enums and texts such as
        //// A bitmask representing whatever for each set bit.
        //// A bitwise combination of the enumeration values.

        protected override string GetDiagnosticId() => MiKo_2024_EnumParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2024_EnumParamDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2024_CodeFixProvider();
    }
}