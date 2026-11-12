using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3015_ArgumentExceptionThrownAtWrongPlaceAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfEqual_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(i, 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfGreaterThan_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(i, 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfGreaterThanOrEqual_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(i, 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfLessThan_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(i, 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfLessThanOrEqual_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(i, 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfNegative_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(i);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfNegativeOrZero_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(i);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfNotEqual_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(i, 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfZero_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfZero(i);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfEqual_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfEqual(SomeValue, 42);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfGreaterThan_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(SomeValue, 42);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfGreaterThanOrEqual_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(SomeValue, 42);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfLessThan_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(SomeValue, 42);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfLessThanOrEqual_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(SomeValue, 42);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfNegative_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(SomeValue);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfNegativeOrZero_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(SomeValue);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfNotEqual_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(SomeValue, 42);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfZero_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfZero(SomeValue);
    }
}
");

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_ThrowIfEqual_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfEqual(SomeValue, 42);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        if (SomeValue is 42)
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_ThrowIfGreaterThan_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(SomeValue, 42);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        if (SomeValue > 42)
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_ThrowIfGreaterThanOrEqual_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(SomeValue, 42);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        if (SomeValue >= 42)
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_ThrowIfLessThan_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(SomeValue, 42);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        if (SomeValue < 42)
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_ThrowIfLessThanOrEqual_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(SomeValue, 42);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        if (SomeValue <= 42)
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_ThrowIfNegative_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(SomeValue);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        if (SomeValue < 0)
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_ThrowIfNegativeOrZero_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(SomeValue);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        if (SomeValue <= 0)
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_ThrowIfNotEqual_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(SomeValue, 42);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        if (SomeValue != 42)
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_ThrowIfZero_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        ArgumentOutOfRangeException.ThrowIfZero(SomeValue);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int SomeValue { get; set; }

    public void DoSomething()
    {
        if (SomeValue is 0)
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_message_and_parameter_as_nameof()
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        throw new ###;
    }

}
";

            VerifyCSharpFix(Template.Replace("###", @"ArgumentOutOfRangeException(nameof(TestMe), ""some message"")"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_message_and_parameter_as_string()
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        throw new ###;
    }

}
";

            VerifyCSharpFix(Template.Replace("###", @"ArgumentOutOfRangeException(""TestMe"", ""some message"")"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_message_value_and_parameter_as_nameof()
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = StringComparison.Ordinal;

        if (x != StringComparison.OrdinalIgnoreCase)
            throw new ###;
    }

}
";

            VerifyCSharpFix(Template.Replace("###", @"ArgumentOutOfRangeException(nameof(TestMe), x, ""some message"")"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_message_value_and_parameter_as_string()
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = StringComparison.Ordinal;

        if (x != StringComparison.OrdinalIgnoreCase)
            throw new ###;
    }

}
";

            VerifyCSharpFix(Template.Replace("###", @"ArgumentOutOfRangeException(""TestMe"", x, ""some message"")"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_parameter_as_nameof()
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        throw new ###;
    }

}
";

            VerifyCSharpFix(Template.Replace("###", "ArgumentOutOfRangeException(nameof(TestMe))"), Template.Replace("###", @"InvalidOperationException(""TODO"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_parameter_as_string()
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        throw new ###;
    }

}
";

            VerifyCSharpFix(Template.Replace("###", @"ArgumentOutOfRangeException(""TestMe"")"), Template.Replace("###", @"InvalidOperationException(""TODO"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_switched_message_and_parameter_as_nameof()
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        throw new ###;
    }

}
";

            VerifyCSharpFix(Template.Replace("###", @"ArgumentOutOfRangeException(""some message"", nameof(TestMe))"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_switched_message_and_parameter_as_string()
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        throw new ###;
    }

}
";

            VerifyCSharpFix(Template.Replace("###", @"ArgumentOutOfRangeException(""some message"", ""TestMe"")"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_switched_message_value_and_parameter_as_nameof()
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = StringComparison.Ordinal;

        if (x != StringComparison.OrdinalIgnoreCase)
            throw new ###;
    }

}
";

            VerifyCSharpFix(Template.Replace("###", @"ArgumentOutOfRangeException(""some message"", x, nameof(TestMe))"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_with_switched_message_value_and_parameter_as_string()
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = StringComparison.Ordinal;

        if (x != StringComparison.OrdinalIgnoreCase)
            throw new ###;
    }

}
";

            VerifyCSharpFix(Template.Replace("###", @"ArgumentOutOfRangeException(""some message"", x, ""TestMe"")"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_without_parameters()
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        throw new ###;
    }

}
";

            VerifyCSharpFix(Template.Replace("###", "ArgumentOutOfRangeException()"), Template.Replace("###", @"InvalidOperationException(""TODO"")"));
        }
    }
}