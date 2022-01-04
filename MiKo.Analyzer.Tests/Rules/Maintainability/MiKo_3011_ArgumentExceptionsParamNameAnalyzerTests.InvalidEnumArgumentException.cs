using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3011_ArgumentExceptionsParamNameAnalyzerTests
    {
        [TestCase("\"some message\", new Exception()")]
        [TestCase("\"x\", (int)x, typeof(StringComparison)")]
        [TestCase("nameof(x), (int)x, typeof(StringComparison)")]
        public void No_issue_is_reported_for_correctly_thrown_InvalidEnumArgumentException_(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(StringComparison x)
    {
        if (x == StringComparison.OrdinalIgnoreCase) throw new InvalidEnumArgumentException(" + parameters + @");
    }
}
");

        [TestCase("")]
        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"X\"")]
        [TestCase("nameof(TestMe)")]
        [TestCase("\"some message\"")]
        [TestCase("\"some message\", (int)x, typeof(StringComparison)")]
        public void An_issue_is_reported_for_incorrectly_thrown_InvalidEnumArgumentException_(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(StringComparison x)
    {
        if (x == StringComparison.OrdinalIgnoreCase) throw new InvalidEnumArgumentException(" + parameters + @");
    }
}
");

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_InvalidEnumArgumentException_with_no_parameters_on_property()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public StringComparison Something
    {
        get => StringComparison.Ordinal;
        set
        {
            if (value == StringComparison.Ordinal) throw new InvalidEnumArgumentException();
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public StringComparison Something
    {
        get => StringComparison.Ordinal;
        set
        {
            if (value == StringComparison.Ordinal) throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(StringComparison));
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_InvalidEnumArgumentException_with_no_parameters_on_indexer_key()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public StringComparison this[StringComparison key]
    {
        get => StringComparison.Ordinal;
        set
        {
            if (key == StringComparison.Ordinal) throw new InvalidEnumArgumentException();
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public StringComparison this[StringComparison key]
    {
        get => StringComparison.Ordinal;
        set
        {
            if (key == StringComparison.Ordinal) throw new InvalidEnumArgumentException(nameof(key), (int)key, typeof(StringComparison));
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_InvalidEnumArgumentException_with_no_parameters_on_indexer_value()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public StringComparison this[StringComparison key]
    {
        get => StringComparison.Ordinal;
        set
        {
            if (value == StringComparison.Ordinal) throw new InvalidEnumArgumentException();
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public StringComparison this[StringComparison key]
    {
        get => StringComparison.Ordinal;
        set
        {
            if (value == StringComparison.Ordinal) throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(StringComparison));
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_InvalidEnumArgumentException_with_no_parameters_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething(StringComparison x)
    {
        if (x == StringComparison.Ordinal) throw new InvalidEnumArgumentException();
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething(StringComparison x)
    {
        if (x == StringComparison.Ordinal) throw new InvalidEnumArgumentException(nameof(x), (int)x, typeof(StringComparison));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_InvalidEnumArgumentException_with_no_parameters_on_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething(StringComparison x, StringComparison y)
    {
        if (y == StringComparison.Ordinal) throw new InvalidEnumArgumentException();
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething(StringComparison x, StringComparison y)
    {
        if (y == StringComparison.Ordinal) throw new InvalidEnumArgumentException(nameof(y), (int)y, typeof(StringComparison));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_InvalidEnumArgumentException_with_message_only_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething(StringComparison x)
    {
        if (x == StringComparison.Ordinal) throw new InvalidEnumArgumentException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething(StringComparison x)
    {
        if (x == StringComparison.Ordinal) throw new InvalidEnumArgumentException(nameof(x), (int)x, typeof(StringComparison));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_InvalidEnumArgumentException_with_message_only_on_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething(StringComparison x, StringComparison y, StringComparison z)
    {
        if (y == StringComparison.Ordinal) throw new InvalidEnumArgumentException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething(StringComparison x, StringComparison y, StringComparison z)
    {
        if (y == StringComparison.Ordinal) throw new InvalidEnumArgumentException(nameof(y), (int)y, typeof(StringComparison));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}