using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5019_InParameterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_gets_reported_for_method_with_no_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test] // this is the case for the Analyze methods of the MiKo analyzers that act as callbacks
        public void No_issue_gets_reported_for_analyze_method_with_context_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void AnalyzeSomething(int context) { }
}
");

        [Test]
        public void No_issue_gets_reported_for_reference_type_as_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) { }
}
");

        [Test]
        public void No_issue_gets_reported_for_interface_as_parameter() => No_issue_is_reported_for(@"
using System;

public interface IMyInterface { }

public class TestMe
{
    public void DoSomething(IMyInterface value) { }
}
");

        [Test]
        public void No_issue_gets_reported_for_record_as_parameter() => No_issue_is_reported_for(@"
using System;

public record MyRecord { }

public class TestMe
{
    public void DoSomething(MyRecord value) { }
}
");

        [Test]
        public void No_issue_gets_reported_for_non_readonly_struct() => No_issue_is_reported_for(@"
using System;

public struct MyStruct { }

public class TestMe
{
    public void DoSomething(MyStruct value)
    {
    }
}
");

        [Test]
        public void No_issue_gets_reported_if_parameter_gets_assigned() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value)
    {
        value = default;
    }
}
");

        [Test]
        public void No_issue_gets_reported_if_parameter_gets_incremented_as_prefix() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value)
    {
        ++value;
    }
}
");

        [Test]
        public void No_issue_gets_reported_if_parameter_gets_decremented_as_prefix() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value)
    {
        --value;
    }
}
");

        [Test]
        public void No_issue_gets_reported_if_parameter_gets_incremented_as_postfix() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value)
    {
        value++;
    }
}
");

        [Test]
        public void No_issue_gets_reported_if_parameter_gets_decremented_as_postfix() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value)
    {
        value--;
    }
}
");

        [Test]
        public void No_issue_gets_reported_if_parameter_gets_used_in_lambda() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value)
    {
        DoSomethingCore(() => value);
    }

    public void DoSomethingCore(Func<int> callback) { }
}
");

        [Test]
        public void No_issue_gets_reported_if_parameter_is_part_of_interface_implementation() => No_issue_is_reported_for(@"
using System;

public class TestMe : IEquatable<int>
{
    public bool Equals(int obj) => 42;
}
");

        [Test]
        public void No_issue_gets_reported_for_yield_returning_method() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<int> DoSomething(int value)
    {
        yield return value;
    }
}
");

        [Test]
        public void No_issue_gets_reported_for_Task_returning_method() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(int value) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_gets_reported_for_async_method() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomethingAsync(int value) { }
}
");

        [Test]
        public void No_issue_gets_reported_for_overridden_method() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe : EqualityComparer<int>
{
    public override bool Equals(int x, int y) => true;

    public override int GetHashCode(int obj) => 42;
}
");

        [Test]
        public void No_issue_gets_reported_for_readonly_struct_as_parameter_with_in_modifier() => No_issue_is_reported_for(@"
using System;

public readonly struct MyStruct { }

public class TestMe
{
    public void DoSomething(in MyStruct value) { }
}
");

        [Test]
        public void No_issue_gets_reported_for_non_readonly_struct_as_parameter_without_in_modifier_but_Range_attribute() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

public class TestMe
{
    public void DoSomething([Range(1, 3)] int value) { }
}
");

        [Test]
        public void No_issue_gets_reported_for_enum_with_in_modifier_as_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(in StringComparison value) { }
}
");

        [Test]
        public void An_issue_gets_reported_for_readonly_struct_as_parameter() => An_issue_is_reported_for(@"
using System;

public readonly struct MyStruct { }

public class TestMe
{
    public void DoSomething(MyStruct value) { }
}
");

        [Test]
        public void An_issue_gets_reported_for_enum_with_no_in_modifier_as_parameter() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(StringComparison value) { }
}
");

        [Test]
        public void Code_gets_fixed_for_readonly_struct_as_parameter()
        {
            const string OriginalCode = @"
using System;

public readonly struct MyStruct { }

public class TestMe
{
    public void DoSomething(MyStruct value) { }
}
";

            const string FixedCode = @"
using System;

public readonly struct MyStruct { }

public class TestMe
{
    public void DoSomething(in MyStruct value) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_enum_as_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(StringComparison value) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(in StringComparison value) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_5019_InParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5019_InParameterAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5019_CodeFixProvider();
    }
}