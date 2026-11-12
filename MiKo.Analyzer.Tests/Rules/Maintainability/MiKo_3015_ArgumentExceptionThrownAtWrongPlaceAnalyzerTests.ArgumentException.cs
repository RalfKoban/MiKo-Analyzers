using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3015_ArgumentExceptionThrownAtWrongPlaceAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_ArgumentException_ThrowIfNullOrEmpty_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s)
    {
        ArgumentException.ThrowIfNullOrEmpty(s);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentException_ThrowIfNullOrWhiteSpace_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(s);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentException_ThrowIfNullOrEmpty_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentException.ThrowIfNullOrEmpty(SomeData);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ArgumentException_ThrowIfNullOrWhiteSpace_in_method_without_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(SomeData);
    }
}
");

        [Test]
        public void Code_gets_fixed_for_ArgumentException_ThrowIfNullOrEmpty_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentException.ThrowIfNullOrEmpty(SomeData);
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
        public void Code_gets_fixed_for_ArgumentException_ThrowIfNullOrWhiteSpace_in_method_without_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(SomeData);
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
        public void Code_gets_fixed_for_ArgumentException_with_message_and_parameter_as_nameof()
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

            VerifyCSharpFix(Template.Replace("###", @"ArgumentException(""some message"", nameof(TestMe))"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentException_with_message_and_parameter_as_string()
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

            VerifyCSharpFix(Template.Replace("###", @"ArgumentException(""some message"", ""TestMe"")"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentException_with_message_only()
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

            VerifyCSharpFix(Template.Replace("###", @"ArgumentException(""some message"")"), Template.Replace("###", @"InvalidOperationException(""some message"")"));
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentException_without_parameters()
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

            VerifyCSharpFix(Template.Replace("###", "ArgumentException()"), Template.Replace("###", @"InvalidOperationException(""TODO"")"));
        }
    }
}