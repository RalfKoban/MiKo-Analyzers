using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3011_ArgumentExceptionsParamNameAnalyzerTests
    {
        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\"")]
        [TestCase("nameof(x), \"some message\"")]
        [TestCase("\"x\", 42, \"some message\"")]
        [TestCase("nameof(x), 42, \"some message\"")]
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
        [TestCase("\"X\"")]
        [TestCase("nameof(TestMe)")]
        [TestCase("\"X\", \"some message\"")]
        [TestCase("nameof(TestMe), \"some message\"")]
        [TestCase("\"some message\"")]
        [TestCase("\"some message\", \"x\"")]
        [TestCase("\"some message\", nameof(x)")]
        [TestCase("\"some message\", new Exception()")]
        [TestCase("\"some message\", 42, \"x\"")]
        [TestCase("\"some message\", 42, nameof(x)")]
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

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentOutOfRangeException_with_no_parameters_on_property()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object Something
    {
        get => null;
        set
        {
            if (value is null) throw new ArgumentOutOfRangeException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object Something
    {
        get => null;
        set
        {
            if (value is null) throw new ArgumentOutOfRangeException(nameof(value), value, ""TODO"");
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentOutOfRangeException_with_no_parameters_on_indexer_key()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object this[object key]
    {
        get => null;
        set
        {
            if (key is null) throw new ArgumentOutOfRangeException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object this[object key]
    {
        get => null;
        set
        {
            if (key is null) throw new ArgumentOutOfRangeException(nameof(key), key, ""TODO"");
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentOutOfRangeException_with_no_parameters_on_indexer_value()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object this[object key]
    {
        get => null;
        set
        {
            if (value is null) throw new ArgumentOutOfRangeException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object this[object key]
    {
        get => null;
        set
        {
            if (value is null) throw new ArgumentOutOfRangeException(nameof(value), value, ""TODO"");
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentOutOfRangeException_with_no_parameters_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentOutOfRangeException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentOutOfRangeException(nameof(x), x, ""TODO"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentOutOfRangeException_with_no_parameters_on_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y)
    {
        if (y is null) throw new ArgumentOutOfRangeException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y)
    {
        if (y is null) throw new ArgumentOutOfRangeException(nameof(y), y, ""TODO"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentOutOfRangeException_with_message_only_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentOutOfRangeException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentOutOfRangeException(nameof(x), x, ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentOutOfRangeException_with_message_only_on_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y, object z)
    {
        if (y is null) throw new ArgumentOutOfRangeException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y, object z)
    {
        if (y is null) throw new ArgumentOutOfRangeException(nameof(y), y, ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentOutOfRangeException_with_reversed_message_and_parameter_as_string()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentOutOfRangeException(""some message"", ""x"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentOutOfRangeException(nameof(x), x, ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentOutOfRangeException_with_reversed_message_and_parameter_as_nameof()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentOutOfRangeException(""some message"", nameof(x));
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentOutOfRangeException(nameof(x), x, ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}