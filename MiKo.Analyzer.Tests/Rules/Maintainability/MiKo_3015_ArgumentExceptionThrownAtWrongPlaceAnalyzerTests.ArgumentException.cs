using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3015_ArgumentExceptionThrownAtWrongPlaceAnalyzerTests
    {
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
    }
}