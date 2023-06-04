using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5013_EmptyArrayAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ValidArrayCreations =
                                                               {
                                                                   "Array.Empty<int>()",
                                                                   "new int[1]",
                                                                   "new int[] { 42 }",
                                                                   "new[] { 42 }",
                                                               };

        private static readonly string[] WrongArrayCreations =
                                                               {
                                                                   "new int[0]",
                                                                   "new int[] { }",
                                                                   "new int[] { /* some comment */ }",
                                                               };

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_valid_assignment_to_variable_([ValueSource(nameof(ValidArrayCreations))] string creation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var i = " + creation + @";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_valid_assignment_to_parameter_([ValueSource(nameof(ValidArrayCreations))] string creation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomething(" + creation + @");
    }

    public void DoSomething(int[] array)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_valid_assignment_to_return_value_([ValueSource(nameof(ValidArrayCreations))] string creation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int[] DoSomething()
    {
        return " + creation + @";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_valid_assignment_to_arrow_expression_return_value_([ValueSource(nameof(ValidArrayCreations))] string creation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int[] DoSomething() => " + creation + @";
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_assignment_to_variable_([ValueSource(nameof(WrongArrayCreations))] string creation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var i = " + creation + @";
    }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_assignment_to_parameter_([ValueSource(nameof(WrongArrayCreations))] string creation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomething(" + creation + @");
    }

    public void DoSomething(int[] array)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_assignment_to_return_value([ValueSource(nameof(WrongArrayCreations))] string creation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int[] DoSomething()
    {
        return " + creation + @";
    }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_assignment_to_arrow_expression_return_value_([ValueSource(nameof(WrongArrayCreations))] string creation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int[] DoSomething() => " + creation + @";
}
");

        [Test]
        public void Code_gets_fixed_for_wrong_assignment_to_variable_([ValueSource(nameof(WrongArrayCreations))] string creation)
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var i = ###;
    }
}
";

            VerifyCSharpFix(Template.Replace("###", creation), Template.Replace("###", "Array.Empty<int>()"));
        }

        [Test]
        public void Code_gets_fixed_for_wrong_assignment_to_parameter_([ValueSource(nameof(WrongArrayCreations))] string creation)
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomething(###);
    }

    public void DoSomething(int[] array)
    {
    }
}
";

            VerifyCSharpFix(Template.Replace("###", creation), Template.Replace("###", "Array.Empty<int>()"));
        }

        [Test]
        public void Code_gets_fixed_for_wrong_assignment_to_return_value_([ValueSource(nameof(WrongArrayCreations))] string creation)
        {
            const string Template = @"
using System;

public class TestMe
{
    public int[] DoSomething()
    {
        return ###;
    }
}
";

            VerifyCSharpFix(Template.Replace("###", creation), Template.Replace("###", "Array.Empty<int>()"));
        }

        [Test]
        public void Code_gets_fixed_for_wrong_assignment_to_arrow_expression_return_value_([ValueSource(nameof(WrongArrayCreations))] string creation)
        {
            const string Template = @"
using System;

public class TestMe
{
    public int[] DoSomething() => ###;
}
";

            VerifyCSharpFix(Template.Replace("###", creation), Template.Replace("###", "Array.Empty<int>()"));
        }

        protected override string GetDiagnosticId() => MiKo_5013_EmptyArrayAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5013_EmptyArrayAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5013_CodeFixProvider();
    }
}