using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3089_DoNotUsePropertyPatternForConditionsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_numeric_single_property_matching_as_return_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    static bool IsConferenceDay(DateTime date) => date is { Year: 2020 };
}");

        [Test]
        public void No_issue_is_reported_for_numeric_multi_property_matching_as_return_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    static bool IsConferenceDay(DateTime date) => date is { Year: 2020, Month: 5, Day: 19 or 20 or 21 };
}");

        [Test]
        public void No_issue_is_reported_for_negative_numeric_multi_property_matching_as_return_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    static bool IsConferenceDay(DateTime date) => date is not { Year: 2020, Month: 5, Day: 19 or 20 or 21 };
}");

        [Test]
        public void No_issue_is_reported_for_numeric_multi_property_matching_as_if_condition() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(DateTime date)
    {
        if (date is { Year: 2020, Month: 5, Day: 19 or 20 or 21 })
        {
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_negative_numeric_multi_property_matching_as_if_condition() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(DateTime date)
    {
        if (date is not { Year: 2020, Month: 5, Day: 19 or 20 or 21 })
        {
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_numeric_single_property_matching_as_if_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(DateTime date)
    {
        if (date is { Year: 2020 })
        {
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_negative_numeric_single_property_matching_as_if_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(DateTime date)
    {
        if (date is not { Year: 2020 })
        {
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_nullable_single_property_matching_as_if_condition() => An_issue_is_reported_for(@"
using System;

public record Dto
{
    public object Data { get; set; }
}

public class TestMe
{
    public void DoSomething(Dto dto)
    {
        if (dto is { Data: null })
        {
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_negative_nullable_single_property_matching_as_if_condition() => An_issue_is_reported_for(@"
using System;

public record Dto
{
    public object Data { get; set; }
}

public class TestMe
{
    public void DoSomething(Dto dto)
    {
        if (dto is not { Data: null })
        {
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_boolean_single_property_matching_as_if_condition_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public record Dto
{
    public bool Data { get; set; }
}

public class TestMe
{
    public void DoSomething(Dto dto)
    {
        if (dto is { Data: " + value + @" })
        {
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_negative_boolean_single_property_matching_as_if_condition_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public record Dto
{
    public bool Data { get; set; }
}

public class TestMe
{
    public void DoSomething(Dto dto)
    {
        if (dto is not { Data: " + value + @" })
        {
        }
    }
}");

        [Test]
        public void Code_gets_fixed_for_numeric_single_property_matching_as_if_condition()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(DateTime date)
    {
        if (date is { Year: 2020 })
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(DateTime date)
    {
        if (date.Year == 2020)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_negative_numeric_single_property_matching_as_if_condition()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(DateTime date)
    {
        if (date is not { Year: 2020 })
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(DateTime date)
    {
        if (date.Year != 2020)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_null_single_property_matching_as_if_condition()
        {
            const string OriginalCode = @"
using System;

public record Dto
{
    public object Data { get; set; }
}

public class TestMe
{
    public void DoSomething(Dto dto)
    {
        if (dto is { Data: null })
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public record Dto
{
    public object Data { get; set; }
}

public class TestMe
{
    public void DoSomething(Dto dto)
    {
        if (dto.Data is null)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_negative_null_single_property_matching_as_if_condition()
        {
            const string OriginalCode = @"
using System;

public record Dto
{
    public object Data { get; set; }
}

public class TestMe
{
    public void DoSomething(Dto dto)
    {
        if (dto is not { Data: null })
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public record Dto
{
    public object Data { get; set; }
}

public class TestMe
{
    public void DoSomething(Dto dto)
    {
        if (dto.Data is not null)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_boolean_single_property_matching_as_if_condition_([Values("true", "false")] string value)
        {
            var originalCode = @"
using System;

public record Dto
{
    public bool Data { get; set; }
}

public class TestMe
{
    public void DoSomething(Dto dto)
    {
        if (dto is { Data: " + value + @" })
        {
        }
    }
}";

            var fixedCode = @"
using System;

public record Dto
{
    public bool Data { get; set; }
}

public class TestMe
{
    public void DoSomething(Dto dto)
    {
        if (dto.Data is " + value + @")
        {
        }
    }
}";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_negative_boolean_single_property_matching_as_if_condition_([Values("true", "false")] string value)
        {
            var originalCode = @"
using System;

public record Dto
{
    public bool Data { get; set; }
}

public class TestMe
{
    public void DoSomething(Dto dto)
    {
        if (dto is not { Data: " + value + @" })
        {
        }
    }
}";

            var fixedCode = @"
using System;

public record Dto
{
    public bool Data { get; set; }
}

public class TestMe
{
    public void DoSomething(Dto dto)
    {
        if (dto.Data is not " + value + @")
        {
        }
    }
}";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3089_DoNotUsePropertyPatternForConditionsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3089_DoNotUsePropertyPatternForConditionsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3089_CodeFixProvider();
    }
}