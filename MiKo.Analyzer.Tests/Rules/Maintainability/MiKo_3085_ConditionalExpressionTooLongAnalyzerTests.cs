using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3085_ConditionalExpressionTooLongAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_conditional_expression_with_short_condition_and_paths() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o) => o != null ? true : false;
}");

        [Test]
        public void No_issue_is_reported_for_conditional_expression_with_Is_expression() => No_issue_is_reported_for(@"
using System;

public class TestMeWithSomeVeryLongNameThatCannotBeShortedAnymore
{
    public bool DoSomething(object o) => obj is TestMeWithSomeVeryLongNameThatCannotBeShortedAnymore ? true : false;
}");

        [Test]
        public void No_issue_is_reported_for_conditional_expression_with_Is_pattern_expression() => No_issue_is_reported_for(@"
using System;

public class TestMeWithSomeVeryLongNameThatCannotBeShortedAnymore
{
    public TestMeWithSomeVeryLongNameThatCannotBeShortedAnymore DoSomething(object o) => o is TestMeWithSomeVeryLongNameThatCannotBeShortedAnymore name ? name : null;
}");

        [Test]
        public void No_issue_is_reported_for_conditional_expression_with_simple_member_access() => No_issue_is_reported_for(@"
using System;

public static class Some
{
    public static class Constants
    {
        public static class That
        {
            public static class CannotBe
            {
                public const bool ShortenedAnymore = true;
            }
        }
    }
}

public class TestMe
{
    public bool DoSomething(object o) => o != null
                                         ? Some.Constants.That.CannotBe.ShortenedAnymore
                                         : false;
}");

        [TestCase("string.IsNullOrEmpty")]
        [TestCase("string.IsNullOrWhiteSpace")]
        [TestCase("!string.IsNullOrEmpty")]
        [TestCase("!string.IsNullOrWhiteSpace")]
        [TestCase(nameof(GetHashCode))]
        public void No_issue_is_reported_for_conditional_expression_using_(string value) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(string someVeryLongStringToCheckSoThatWeExceedTheLimits) => " + value + @"(someVeryLongStringToCheckSoThatWeExceedTheLimits)
                                         ? true
                                         : false;
}");

        [Test]
        public void No_issue_is_reported_for_object_creation_only() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    internal static ObjectDisposedException ObjectDisposed(string objectName, string message) => string.IsNullOrWhiteSpace(message)
                                                                                                 ? new ObjectDisposedException(objectName)
                                                                                                 : new ObjectDisposedException(objectName, message);
}");

        [Test]
        public void No_issue_is_reported_for_object_creation_with_literal() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, string message, object value) => string.IsNullOrWhiteSpace(message)
                                                                                                                      ? new ArgumentOutOfRangeException(paramName, 0815, string.Empty)
                                                                                                                      : new ArgumentOutOfRangeException(paramName, 0815, message);
}");

        [Test]
        public void No_issue_is_reported_for_object_creation_with_local_variable() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, string message, object value)
    {
        var someLongerValue = 0815;

        return value is null
               ? new ArgumentOutOfRangeException(paramName, someLongerValue, string.Empty)
               : new ArgumentOutOfRangeException(paramName, someLongerValue, message);
}");

        [Test]
        public void No_issue_is_reported_for_object_initializer() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Name { get; set; }

    public Guid ID { get; set; }

    public static TestMe Create(bool flag) => flag
                                            ? new TestMe { Name = ""my very long name to see whether it is too long"", ID = Guid.Empty }
                                            : new TestMe { Name = ""my very long name to see whether it is too long"", ID = new Guid(""45A3C8BA-2BC9-41F4-BA0D-997D38C8E6BA"") };
}");

        [Test]
        public void No_issue_is_reported_for_interpolated_string_literal() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Name { get; set; }

    public Guid ID { get; set; }

    public static string Create(bool flag) => flag
                                            ? $""my very long interpolated string with {Name} and {ID} to see whether it is too long""
                                            : $""my very long interpolated string with {Name} and {ID} to see whether it is too long"";
}");

        [Test]
        public void No_issue_is_reported_for_long_Try_term() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public static bool Do(string input) => Guid.TryParseExact(input, ""X"", out var veryLongGuidToMatchRule) ? 1 : 0;
}");

        [Test]
        public void No_issue_is_reported_for_Empty_enumerable() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMeWithAVeryLongName
{
    public static IEnumerable<TestMeWithAVeryLongName> Do(bool flag) => flag
                                                            ? new TestMeWithAVeryLongName[0]
                                                            : Enumerable.Empty<TestMeWithAVeryLongName>()
}");

        [Test]
        public void No_issue_is_reported_for_Empty_array() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMeWithAVeryLongName
{
    public static IEnumerable<TestMeWithAVeryLongName> Do(bool flag) => flag
                                                            ? new TestMeWithAVeryLongName[0]
                                                            : Array.Empty<TestMeWithAVeryLongName>()
}");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_long_condition_and_short_paths() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o) => (o != null && (o.GetHashCode() == 42 || o.GetHashCode() == 0815)) ? true : false;
}");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_short_condition_and_long_true_path() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        return items != null
                ? items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any()
                : false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_short_condition_and_long_false_path() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        return items == null
                ? false
                : items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any();
    }
}");

        [Test]
        public void An_issue_is_reported_for_object_creation_with_Linq() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, string[] messages, object value) => messages.Any()
                                                                                                                          ? new ArgumentOutOfRangeException(paramName, 0815, messages.Where(_ => _.Length > 1).FirstOrDefault())
                                                                                                                          : new ArgumentOutOfRangeException(paramName, 0815, string.Empty);
}");

        [Test]
        public void Code_gets_fixed_for_conditional_expression_with_long_condition_and_short_paths()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool DoSomething(object o) => (o != null && (o.GetHashCode() == 42 || o.GetHashCode() == 0815)) ? true : false;
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool DoSomething(object o)
    {
        if (o != null && (o.GetHashCode() == 42 || o.GetHashCode() == 0815))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_expression_with_short_condition_and_long_true_path()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        return items != null
                ? items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any()
                : false;
    }
}";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        if (items != null)
        {
            return items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any();
        }
        else
        {
            return false;
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_expression_with_short_condition_and_long_false_path()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        return items == null
                ? false
                : items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any();
    }
}";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        if (items == null)
        {
            return false;
        }
        else
        {
            return items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any();
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_object_creation_with_Linq()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, string[] messages, object value) => messages.Any()
                                                                                                                          ? new ArgumentOutOfRangeException(paramName, 0815, messages.Where(_ => _.Length > 1).FirstOrDefault())
                                                                                                                          : new ArgumentOutOfRangeException(paramName, 0815, string.Empty);
}";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, string[] messages, object value)
    {
        if (messages.Any())
        {
            return new ArgumentOutOfRangeException(paramName, 0815, messages.Where(_ => _.Length > 1).FirstOrDefault());
        }
        else
        {
            return new ArgumentOutOfRangeException(paramName, 0815, string.Empty);
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_object_creation_with_throw()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, string[] messages, object value)
    {
        throw messages.Any()
              ? new ArgumentOutOfRangeException(paramName, 0815, messages.Where(_ => _.Length > 1).FirstOrDefault())
              : new ArgumentOutOfRangeException(paramName, 0815, string.Empty);
    }
}";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, string[] messages, object value)
    {
        if (messages.Any())
        {
            throw new ArgumentOutOfRangeException(paramName, 0815, messages.Where(_ => _.Length > 1).FirstOrDefault());
        }
        else
        {
            throw new ArgumentOutOfRangeException(paramName, 0815, string.Empty);
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_expression_in_property()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private object o;

    public bool DoSomething => (o != null && (o.GetHashCode() == 42 || o.GetHashCode() == 0815)) ? true : false;
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    private object o;

    public bool DoSomething
    {
        get
        {
            if (o != null && (o.GetHashCode() == 42 || o.GetHashCode() == 0815))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_expression_in_property_getter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private object o;

    public bool DoSomething
    {
        get => (o != null && (o.GetHashCode() == 42 || o.GetHashCode() == 0815)) ? true : false;
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    private object o;

    public bool DoSomething
    {
        get
        {
            if (o != null && (o.GetHashCode() == 42 || o.GetHashCode() == 0815))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_expression_in_property_setter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private object o;
    private bool doSomething;

    public bool DoSomething
    {
        get => doSomething;
        set => doSomething = (o != null && (o.GetHashCode() == 42 || o.GetHashCode() == 0815)) ? true : false;
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    private object o;
    private bool doSomething;

    public bool DoSomething
    {
        get => doSomething;
        set
        {
            if (o != null && (o.GetHashCode() == 42 || o.GetHashCode() == 0815))
            {
                doSomething = true;
            }
            else
            {
                doSomething = false;
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_expression_with_var_variable_assignment()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        var result = items != null
                     ? items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any()
                     : false;

        return result;
    }
}";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        bool result;
        if (items != null)
        {
            result = items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any();
        }
        else
        {
            result = false;
        }

        return result;
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_expression_with_typed_variable_assignment()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        bool result = items != null
                      ? items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any()
                      : false;

        return result;
    }
}";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        bool result;
        if (items != null)
        {
            result = items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any();
        }
        else
        {
            result = false;
        }

        return result;
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_expression_with_typed_simple_variable_assignment()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        bool result;
        result = false;

        result = items != null
                 ? items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any()
                 : false;

        return result;
    }
}";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        bool result;
        result = false;

        if (items != null)
        {
            result = items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any();
        }
        else
        {
            result = false;
        }

        return result;
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test, Ignore("Just for now")]
        public void Code_gets_fixed_for_conditional_expression_as_parameter_assignment_in_method_expression_body_with_return_value()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items) => DoSomethingCore(items != null
                                                                   ? items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42)
                                                                   : Enumerable.Empty<int>());

    private void DoSomethingCore(List<int> numbers)
    {
    }
}";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        bool numbers;
        if (items != null)
        {
            numbers = items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42);
        }
        else
        {
            numbers = Enumerable.Empty<int>();
        }

        return DoSomethingCore(numbers);
    }

    private void DoSomethingCore(List<int> numbers)
    {
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test, Ignore("Just for now")]
        public void Code_gets_fixed_for_conditional_expression_as_parameter_assignment_in_method_expression_body_without_return_value()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    public void DoSomething(List<object> items) => DoSomethingCore(items != null
                                                                   ? items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42)
                                                                   : Enumerable.Empty<int>());

    private void DoSomethingCore(List<int> numbers)
    {
    }
}";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    public void DoSomething(List<object> items)
    {
        bool numbers;
        if (items != null)
        {
            numbers = items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42);
        }
        else
        {
            numbers = Enumerable.Empty<int>();
        }

        DoSomethingCore(numbers);
    }

    private void DoSomethingCore(List<int> numbers)
    {
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test, Ignore("Just for now")]
        public void Code_gets_fixed_for_conditional_expression_as_parameter_assignment_in_method_body()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        return DoSomethingCore(items != null
                               ? items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42)
                               : Enumerable.Empty<int>());
    }

    private void DoSomethingCore(List<int> numbers)
    {
    }
}";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        bool numbers;
        if (items != null)
        {
            numbers = items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42);
        }
        else
        {
            numbers = Enumerable.Empty<int>();
        }

        return DoSomethingCore(numbers);
    }

    private void DoSomethingCore(List<int> numbers)
    {
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        // TODO RKN: Conditional as parameter
        // TODO RKN: Conditional inside conditional
        // TODO RKN: Conditional within initializer
        protected override string GetDiagnosticId() => MiKo_3085_ConditionalExpressionTooLongAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3085_ConditionalExpressionTooLongAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3085_CodeFixProvider();
    }
}