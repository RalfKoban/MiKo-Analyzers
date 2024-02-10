using NUnit.Framework;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3011_ArgumentExceptionsParamNameAnalyzerTests
    {
        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\"")]
        [TestCase("nameof(x), \"some message\"")]
        public void No_issue_is_reported_for_correctly_thrown_ArgumentNullException_(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentNullException(" + parameters + @");
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
        public void An_issue_is_reported_for_incorrectly_thrown_ArgumentNullException_(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentNullException(" + parameters + @");
    }
}
");

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_no_parameters_on_property()
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
            if (value is null) throw new ArgumentNullException();
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
            if (value is null) throw new ArgumentNullException(nameof(value), ""TODO"");
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_no_parameters_on_indexer_key()
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
            if (key is null) throw new ArgumentNullException();
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
            if (key is null) throw new ArgumentNullException(nameof(key), ""TODO"");
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_no_parameters_on_indexer_value()
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
            if (value is null) throw new ArgumentNullException();
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
            if (value is null) throw new ArgumentNullException(nameof(value), ""TODO"");
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_no_parameters_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(nameof(x), ""TODO"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_no_parameters_on_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y)
    {
        if (y is null) throw new ArgumentNullException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y)
    {
        if (y is null) throw new ArgumentNullException(nameof(y), ""TODO"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_message_only_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(nameof(x), ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_message_only_on_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y, object z)
    {
        if (y is null) throw new ArgumentNullException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y, object z)
    {
        if (y is null) throw new ArgumentNullException(nameof(y), ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_message_only_and_some_comment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null)
        {
            // some comment
            throw new ArgumentNullException(""some message""); // some more comment
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null)
        {
            // some comment
            throw new ArgumentNullException(nameof(x), ""some message""); // some more comment
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_reversed_message_and_parameter_as_string()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(""some message"", ""x"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(nameof(x), ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_reversed_message_and_parameter_as_nameof()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(""some message"", nameof(x));
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(nameof(x), ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_multiple_parameters_on_method_and_string_interpolation()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y, object z)
    {
        if (y is null)
            throw new ArgumentNullException($""'y' should be null but was {y}"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y, object z)
    {
        if (y is null)
            throw new ArgumentNullException(nameof(y), $""'y' should be null but was {y}"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}