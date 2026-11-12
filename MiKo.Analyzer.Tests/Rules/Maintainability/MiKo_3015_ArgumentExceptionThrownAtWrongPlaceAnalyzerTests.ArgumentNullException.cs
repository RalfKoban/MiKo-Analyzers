using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3015_ArgumentExceptionThrownAtWrongPlaceAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_ArgumentNullException_ThrowIfNull_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s)
    {
        ArgumentNullException.ThrowIfNull(s);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentNullException_ThrowIfNullOrEmpty_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(s);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentNullException_ThrowIfNullOrWhiteSpace_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(s);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentNullException_ThrowIfNull_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentNullException.ThrowIfNull(SomeData);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentNullException_ThrowIfNullOrEmpty_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentNullException.ThrowIfNullOrEmpty(SomeData);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentNullException_ThrowIfNullOrWhiteSpace_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(SomeData);
    }
}
");

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_ThrowIfNull_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentNullException.ThrowIfNull(SomeData);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        if (SomeData is null)
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_ThrowIfNull_in_unbraced_if_branch_with_else()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething(bool ready)
    {
    }

    public void DoSomething()
    {
        var ready = true;

        if (ready)
            ArgumentNullException.ThrowIfNull(SomeData);
        else
            Recover();
    }

    private void Recover()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething(bool ready)
    {
    }

    public void DoSomething()
    {
        var ready = true;

        if (ready)
        {
            if (SomeData is null)
            {
                throw new InvalidOperationException(""TODO"");
            }
        }
        else
            Recover();
    }

    private void Recover()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_ThrowIfNullOrEmpty_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentNullException.ThrowIfNullOrEmpty(SomeData);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        if (string.IsNullOrEmpty(SomeData))
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_ThrowIfNullOrEmpty_in_unbraced_if_branch_with_else()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        var ready = true;

        if (ready)
            ArgumentNullException.ThrowIfNullOrEmpty(SomeData);
        else
            Recover();
    }

    private void Recover()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        var ready = true;

        if (ready)
        {
            if (string.IsNullOrEmpty(SomeData))
            {
                throw new InvalidOperationException(""TODO"");
            }
        }
        else
            Recover();
    }

    private void Recover()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_ThrowIfNullOrWhiteSpace_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(SomeData);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        if (string.IsNullOrWhiteSpace(SomeData))
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_with_message_and_parameter_as_nameof()
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

            VerifyCSharpFix(Template.Replace("###", @"ArgumentNullException(nameof(TestMe), ""some message"")"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_with_message_and_parameter_as_string()
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

            VerifyCSharpFix(Template.Replace("###", @"ArgumentNullException(""TestMe"", ""some message"")"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_with_parameter_as_nameof()
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

            VerifyCSharpFix(Template.Replace("###", "ArgumentNullException(nameof(TestMe))"), Template.Replace("###", @"InvalidOperationException(""TODO"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_with_parameter_as_string()
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

            VerifyCSharpFix(Template.Replace("###", @"ArgumentNullException(""TestMe"")"), Template.Replace("###", @"InvalidOperationException(""TODO"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_with_switched_message_and_parameter_as_nameof()
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

            VerifyCSharpFix(Template.Replace("###", @"ArgumentNullException(""some message"", nameof(TestMe))"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_with_switched_message_and_parameter_as_string()
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

            VerifyCSharpFix(Template.Replace("###", @"ArgumentNullException(""some message"", ""TestMe"")"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_without_parameters()
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

            VerifyCSharpFix(Template.Replace("###", "ArgumentNullException()"), Template.Replace("###", @"InvalidOperationException(""TODO"")"));
        }
    }
}