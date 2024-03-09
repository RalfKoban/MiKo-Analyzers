using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3015_ArgumentExceptionThrownAtWrongPlaceAnalyzerTests
    {
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
    }
}