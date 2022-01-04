using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_thrown_ArgumentException() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_thrown_ArgumentNullException() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentNullException();
    }
}
");

        [TestCase("\"x\", 42, \"some message\"")]
        [TestCase("nameof(x), 42, \"some message\"")]
        [TestCase("\"some message\", new Exception()")]
        public void No_issue_is_reported_for_correctly_thrown_ArgumentOutOfRangeException_(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(" + parameters + @");
    }
}
");

        [TestCase("")]
        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\"")]
        [TestCase("nameof(x), \"some message\"")]
        [TestCase("\"some message\"")]
        [TestCase("\"some message\", \"x\"")]
        [TestCase("\"some message\", nameof(x)")]
        public void An_issue_is_reported_for_incorrectly_thrown_ArgumentOutOfRangeException_(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(" + parameters + @");
    }
}
");

        [TestCase("\"x\", (int)x, typeof(StringComparison)")]
        [TestCase("nameof(x), (int)x, typeof(StringComparison)")]
        [TestCase("\"some message\", new Exception()")]
        public void No_issue_is_reported_for_correctly_thrown_InvalidEnumArgumentException_(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(StringComparison x)
    {
        if (x == StringComparison.Ordinal) throw new InvalidEnumArgumentException(" + parameters + @");
    }
}
");

        [TestCase("")]
        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"some message\"")]
        public void An_issue_is_reported_for_incorrectly_thrown_InvalidEnumArgumentException_(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == StringComparison.Ordinal) throw new InvalidEnumArgumentException(" + parameters + @");
    }
}
");

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_in_switch_statement_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        switch (x)
        {
            case 1:
                return;
            case 2:
            case 3:
                return;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        switch (x)
        {
            case 1:
                return;
            case 2:
            case 3:
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(x), x, ""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_in_switch_statement_without_parameters_in_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o, int x)
    {
        switch (x)
        {
            case 1:
                return;
            case 2:
            case 3:
                return;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o, int x)
    {
        switch (x)
        {
            case 1:
                return;
            case 2:
            case 3:
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(x), x, ""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(nameof(x), x, ""TODO"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_single_paramName_parameter_as_string()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(""x"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(nameof(x), x, ""TODO"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_single_paramName_parameter_as_nameof()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(nameof(x));
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(nameof(x), x, ""TODO"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_single_misused_text_message_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(nameof(x), x, ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_single_misused_constant_text_message_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private const string SomeMessage = ""some message"";

    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(SomeMessage);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private const string SomeMessage = ""some message"";

    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(nameof(x), x, SomeMessage);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_single_misused_localized_text_message_parameter()
        {
            const string OriginalCode = @"
using System;

public class Resources
{
    public static string Text { get; } => ""some message"";
}

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(Resources.Text);
    }
}
";

            const string FixedCode = @"
using System;

public class Resources
{
    public static string Text { get; } => ""some message"";
}

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(nameof(x), x, Resources.Text);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_paramName_as_string_and_message_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(""x"", ""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(nameof(x), x, ""some message"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_paramName_as_nameof_and_message_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(nameof(x), ""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(nameof(x), x, ""some message"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_switched_paramName_as_string_and_message_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(""some message"", ""x"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(nameof(x), x, ""some message"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_switched_paramName_as_nameof_and_message_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(""some message"", nameof(x));
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(nameof(x), x, ""some message"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_InvalidEnumArgumentException_in_switch_statement_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(StringComparison c)
    {
        switch (c)
        {
            case StringComparison.Ordinal:
                return;
            case StringComparison.OrdinalIgnoreCase:
            case StringComparison.InvariantCulture:
                return;

            default:
                throw new InvalidEnumArgumentException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(StringComparison c)
    {
        switch (c)
        {
            case StringComparison.Ordinal:
                return;
            case StringComparison.OrdinalIgnoreCase:
            case StringComparison.InvariantCulture:
                return;

            default:
                throw new InvalidEnumArgumentException(nameof(c), (int)c, typeof(StringComparison));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_InvalidEnumArgumentException_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(StringComparison c)
    {
        if (c == StringComparison.Ordinal) throw new InvalidEnumArgumentException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(StringComparison c)
    {
        if (c == StringComparison.Ordinal) throw new InvalidEnumArgumentException(nameof(c), (int)c, typeof(StringComparison));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_InvalidEnumArgumentException_with_single_message_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(StringComparison c)
    {
        if (c == StringComparison.Ordinal) throw new InvalidEnumArgumentException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(StringComparison c)
    {
        if (c == StringComparison.Ordinal) throw new InvalidEnumArgumentException(nameof(c), (int)c, typeof(StringComparison));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_InvalidEnumArgumentException_in_switch_statement_without_parameters_in_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int i, StringComparison c)
    {
        switch (c)
        {
            case StringComparison.Ordinal:
                return;
            case StringComparison.OrdinalIgnoreCase:
            case StringComparison.InvariantCulture:
                return;

            default:
                throw new InvalidEnumArgumentException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int i, StringComparison c)
    {
        switch (c)
        {
            case StringComparison.Ordinal:
                return;
            case StringComparison.OrdinalIgnoreCase:
            case StringComparison.InvariantCulture:
                return;

            default:
                throw new InvalidEnumArgumentException(nameof(c), (int)c, typeof(StringComparison));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3012_CodeFixProvider();
    }
}