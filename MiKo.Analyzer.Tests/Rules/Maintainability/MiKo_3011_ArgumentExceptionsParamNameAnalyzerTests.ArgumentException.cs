using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3011_ArgumentExceptionsParamNameAnalyzerTests
    {
        [TestCase("")]
        [TestCase("\"some message\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\", null")]
        [TestCase("nameof(x), \"some message\", null")]
        [TestCase("\"some message\", \"X\"")]
        [TestCase("\"some message\", nameof(TestMe)")]
        [TestCase("\"some message\", \"X\", null")]
        [TestCase("\"some message\", nameof(TestMe), null")]
        [TestCase("\"some message\", new Exception()")]
        public void No_issue_is_reported_for_incorrectly_thrown_ArgumentException_on_parameterless_method_(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        throw new ArgumentException(" + parameters + @");
    }
}
");

        [TestCase("\"some message\", \"x\"")]
        [TestCase("\"some message\", nameof(x)")]
        [TestCase("\"some message\", \"x\", null")]
        [TestCase("\"some message\", nameof(x), null")]
        public void No_issue_is_reported_for_correctly_thrown_ArgumentException_(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(" + parameters + @");
    }
}
");

        [TestCase("")]
        [TestCase("\"some message\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\", null")]
        [TestCase("nameof(x), \"some message\", null")]
        [TestCase("\"some message\", \"X\"")]
        [TestCase("\"some message\", nameof(TestMe)")]
        [TestCase("\"some message\", \"X\", null")]
        [TestCase("\"some message\", nameof(TestMe), null")]
        [TestCase("\"some message\", new Exception()")]
        public void An_issue_is_reported_for_incorrectly_thrown_ArgumentException_(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(" + parameters + @");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_thrown_ArgumentException_with_string_literal() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException($""{x} is not allowed"");
    }
}
");

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_no_parameters_on_property()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int Something
    {
        get => 42;
        set
        {
            if (value == 42) throw new ArgumentException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int Something
    {
        get => 42;
        set
        {
            if (value == 42) throw new ArgumentException(""TODO"", nameof(value));
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_no_parameters_on_indexer_key()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int this[int key]
    {
        get => 42;
        set
        {
            if (key == 42) throw new ArgumentException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int this[int key]
    {
        get => 42;
        set
        {
            if (key == 42) throw new ArgumentException(""TODO"", nameof(key));
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_no_parameters_on_indexer_value()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int this[int key]
    {
        get => 42;
        set
        {
            if (value == 42) throw new ArgumentException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int this[int key]
    {
        get => 42;
        set
        {
            if (value == 42) throw new ArgumentException(""TODO"", nameof(value));
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_no_parameters_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""TODO"", nameof(x));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_no_parameters_on_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y)
    {
        if (y == 42) throw new ArgumentException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y)
    {
        if (y == 42) throw new ArgumentException(""TODO"", nameof(y));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_message_only_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""some message"", nameof(x));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_message_only_on_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42) throw new ArgumentException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42) throw new ArgumentException(""some message"", nameof(y));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_parameter_name_only_instead_of_message_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""x"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""TODO"", nameof(x));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_parameter_name_only_instead_of_message_on_method_with_multiple_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y)
    {
        if (y == 42) throw new ArgumentException(""y"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y)
    {
        if (y == 42) throw new ArgumentException(""TODO"", nameof(y));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_message_only_and_some_comment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42)
        {
            // some comment
            throw new ArgumentException(""some message""); // some more comment
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
        if (x == 42)
        {
            // some comment
            throw new ArgumentException(""some message"", nameof(x)); // some more comment
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_reversed_message_and_parameter_as_string()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""x"", ""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""some message"", nameof(x));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_reversed_message_and_parameter_as_nameof()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(nameof(x), ""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""some message"", nameof(x));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_multiple_parameters_on_method_and_return_in_if_statement()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42)
            return;

        throw new ArgumentException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42)
            return;

        throw new ArgumentException(""TODO"", nameof(y));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_multiple_parameters_on_method_and_in_else_statement()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42)
            return;
        else
            throw new ArgumentException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42)
            return;
        else
            throw new ArgumentException(""TODO"", nameof(y));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_multiple_parameters_on_method_and_string_interpolation()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42)
            throw new ArgumentException($""'y' should be 42 but was {y}"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42)
            throw new ArgumentException($""'y' should be 42 but was {y}"", nameof(y));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_multiple_parameters_on_method_and_exception()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, Exception ex)
    {
        if (y == 42)
            throw new ArgumentException(""y"", ""some message"", ex);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, Exception ex)
    {
        if (y == 42)
            throw new ArgumentException(""some message"", nameof(y), ex);
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}