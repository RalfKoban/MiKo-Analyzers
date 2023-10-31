using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

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
    /// <param name='o'>One of the enumeration members that specifies whatever it is.</param>
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
    /// <param name='o'>One of the enumeration members that specifies whatever it is.</param>
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

        protected override string GetDiagnosticId() => MiKo_2024_EnumParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2024_EnumParamDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2024_CodeFixProvider();
    }
}