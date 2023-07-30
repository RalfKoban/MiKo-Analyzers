using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public sealed class MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_there_is_no_multi_line_call_chain() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString();
    }
}
");

        [Test]
        public void No_issue_is_reported_if_call_chain_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        var x = o.ToString().ToString().ToString();
    }
}
");

        [Test]
        public void No_issue_is_reported_if_multi_line_call_chain_is_indented_correctly() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        var x = o.ToString()
                 .ToString()
                 .ToString();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_multi_line_call_chain_contains_calls_indented_more_to_the_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        var x = o.ToString()
                  .ToString()
                 .ToString();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_multi_line_call_chain_contains_calls_indented_more_to_the_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        var x = o.ToString()
                .ToString()
                 .ToString();
    }
}
");

        [Test]
        public void Code_gets_fixed_if_multi_line_call_chain_contains_calls_indented_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        var x = o.ToString()
                  .GetHashCode()
                 .GetType();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        var x = o.ToString()
                 .GetHashCode()
                 .GetType();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_multi_line_call_chain_contains_calls_indented_more_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        var x = o.ToString()
                .GetHashCode()
                 .GetType();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        var x = o.ToString()
                 .GetHashCode()
                 .GetType();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_multi_line_call_chain_contains_calls_indented_more_to_the_left_and_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        var x = o.ToString()
                .GetHashCode()
             .GetType()
                  .GetElementType()
                    .GetEnumName();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        var x = o.ToString()
                 .GetHashCode()
                 .GetType()
                 .GetElementType()
                 .GetEnumName();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6040_CodeFixProvider();
    }
}