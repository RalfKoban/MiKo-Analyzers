using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3015_ArgumentExceptionThrownAtWrongPlaceAnalyzerTests
    {
        [Test]
        public void Code_gets_fixed_for_InvalidEnumArgumentException_without_parameters()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething()
    {
        throw new InvalidEnumArgumentException();
    }

}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        throw new InvalidOperationException(""TODO"");
    }

}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_InvalidEnumArgumentException_with_parameter_as_string()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething()
    {
        throw new InvalidEnumArgumentException(""TestMe"");
    }

}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        throw new InvalidOperationException(""TODO"");
    }

}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_InvalidEnumArgumentException_with_parameter_as_nameof()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething()
    {
        throw new InvalidEnumArgumentException(nameof(TestMe));
    }

}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        throw new InvalidOperationException(""TODO"");
    }

}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_InvalidEnumArgumentException_with_message_value_and_type()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething()
    {
        var x = StringComparison.Ordinal;

        if (x != StringComparison.OrdinalIgnoreCase)
            throw new InvalidEnumArgumentException(""some message"", x, typeof(StringComparison));
    }

}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = StringComparison.Ordinal;

        if (x != StringComparison.OrdinalIgnoreCase)
            throw new InvalidOperationException(""some message"");
    }

}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_InvalidEnumArgumentException_with_string_variable_as_message_value_and_type()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething()
    {
        var x = StringComparison.Ordinal;

        if (x != StringComparison.OrdinalIgnoreCase)
            throw new InvalidEnumArgumentException(""x"", x, typeof(StringComparison));
    }

}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = StringComparison.Ordinal;

        if (x != StringComparison.OrdinalIgnoreCase)
            throw new InvalidOperationException(""TODO"");
    }

}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_InvalidEnumArgumentException_with_nameof_variable_as_message_value_and_type()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething()
    {
        var x = StringComparison.Ordinal;

        if (x != StringComparison.OrdinalIgnoreCase)
            throw new InvalidEnumArgumentException(nameof(x), x, typeof(StringComparison));
    }

}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = StringComparison.Ordinal;

        if (x != StringComparison.OrdinalIgnoreCase)
            throw new InvalidOperationException(""TODO"");
    }

}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}