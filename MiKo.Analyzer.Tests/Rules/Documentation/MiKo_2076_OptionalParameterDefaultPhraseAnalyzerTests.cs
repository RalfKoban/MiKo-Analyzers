using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2076_OptionalParameterDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method_with_no_optional_value() => No_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method_with_no_optional_value() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">
    /// Some value.
    /// </param>
    public bool DoSomething(bool value)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_method_with_optional_value() => No_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(bool value = false)
    {
    }
}
");

        [TestCase("The default is")]
        [TestCase("The default value is")]
        [TestCase("Default is")]
        [TestCase("Default value is")]
        [TestCase("Defaults to")]
        public void No_issue_is_reported_for_documented_method_with_optional_value_(string phrase) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">
    /// Some value. " + phrase + @" <see langword=""null""/>.
    /// </param>
    public bool DoSomething(string value = null)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method_with_optional_value_marked_with_CallerMemberName_attribute() => No_issue_is_reported_for(@"
using System.Runtime.CompilerServices;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">
    /// Some value.
    /// </param>
    public bool DoSomething([CallerMemberName] string value = null)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method_with_optional_value_marked_with_Optional_attribute() => No_issue_is_reported_for(@"
using System.Runtime.InteropServices;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">
    /// Some value.
    /// </param>
    public bool DoSomething([Optional] string value)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_parameter_on_method_([Values("", "     ")] string parameter) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">" + parameter + @"</param>
    public bool DoSomething(bool value = false)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_documented_method_with_optional_value_with_missing_default_documentation() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">
    /// Some value.
    /// </param>
    public bool DoSomething(bool value = false)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_documented_method_with_multiple_parameters_and_an_optional_value_with_missing_default_documentation() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""exception"">
    /// Some exception.
    /// </param>
    /// <param name=""value"">
    /// Some value.
    /// </param>
    public bool DoSomething(Exception exception, bool value = false)
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_documented_method_with_optional_value_with_missing_default_documentation_for_boolean_([Values("false", "true")] string value)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">
    /// Some value.
    /// </param>
    public bool DoSomething(bool value = " + value + @")
    {
    }
}
";

            var fixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">
    /// Some value.
    /// The default is <see langword=""" + value + @"""/>.
/// </param>
    public bool DoSomething(bool value = " + value + @")
    {
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documented_method_with_optional_value_with_missing_default_documentation_for_object()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">
    /// Some value.
    /// </param>
    public bool DoSomething(object value = null)
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">
    /// Some value.
    /// The default is <see langword=""null""/>.
/// </param>
    public bool DoSomething(object value = null)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documented_method_in_namespace_with_optional_value_with_missing_default_documentation_for_object()
        {
            const string OriginalCode = @"
public namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Does something.
        /// </summary>
        /// <param name=""value"">
        /// Some value.
        /// </param>
        public bool DoSomething(object value = null)
        {
        }
    }
}
";

            const string FixedCode = @"
public namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Does something.
        /// </summary>
        /// <param name=""value"">
        /// Some value.
        /// The default is <see langword=""null""/>.
/// </param>
        public bool DoSomething(object value = null)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documented_method_in_namespace_with_optional_value_with_missing_default_documentation_for_integer()
        {
            const string OriginalCode = @"
public namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Does something.
        /// </summary>
        /// <param name=""value"">
        /// Some value.
        /// </param>
        public bool DoSomething(int value = -1)
        {
        }
    }
}
";

            const string FixedCode = @"
public namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Does something.
        /// </summary>
        /// <param name=""value"">
        /// Some value.
        /// The default is <c>-1</c>.
/// </param>
        public bool DoSomething(int value = -1)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documented_method_in_namespace_with_optional_value_with_missing_default_documentation_for_enum()
        {
            const string OriginalCode = @"
using System;

public namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Does something.
        /// </summary>
        /// <param name=""value"">
        /// Some value.
        /// </param>
        public bool DoSomething(StringComparison value = StringComparison.OrdinalIgnoreCase)
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

public namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Does something.
        /// </summary>
        /// <param name=""value"">
        /// Some value.
        /// The default is <see cref=""StringComparison.OrdinalIgnoreCase""/>.
/// </param>
        public bool DoSomething(StringComparison value = StringComparison.OrdinalIgnoreCase)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documented_method_in_namespace_with_optional_value_with_missing_default_documentation_for_enum_on_same_line()
        {
            const string OriginalCode = @"
using System;

public namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""value"">Some value.</param>
        public bool DoSomething(StringComparison value = StringComparison.OrdinalIgnoreCase)
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

public namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""value"">Some value. The default is <see cref=""StringComparison.OrdinalIgnoreCase""/>.</param>
        public bool DoSomething(StringComparison value = StringComparison.OrdinalIgnoreCase)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documented_method_in_namespace_with_optional_value_with_missing_default_documentation_for_hardcoded_string_on_same_line()
        {
            const string OriginalCode = @"
using System;

public namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""value"">Some value.</param>
        public bool DoSomething(string value = ""*.*"")
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

public namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""value"">Some value. The default is <c>""*.*""</c>.</param>
        public bool DoSomething(string value = ""*.*"")
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documented_method_in_namespace_with_optional_value_with_missing_default_documentation_for_number_set_by_constant_field_on_same_line()
        {
            const string OriginalCode = @"
using System;

public namespace Bla
{
    public class TestMe
    {
        public const int DEFAULT_VALUE = 1234;

        /// <summary>Does something.</summary>
        /// <param name=""value"">Some value.</param>
        public bool DoSomething(int value = DEFAULT_VALUE))
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

public namespace Bla
{
    public class TestMe
    {
        public const int DEFAULT_VALUE = 1234;

        /// <summary>Does something.</summary>
        /// <param name=""value"">Some value. The default is <see cref=""DEFAULT_VALUE""/>.</param>
        public bool DoSomething(int value = DEFAULT_VALUE))
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("bool", "<see langword=\"false\"/>")]
        [TestCase("byte", "<c>0</c>")]
        [TestCase("CancellationToken", "<see cref=\"CancellationToken\"/>")]
        [TestCase("char", "<c>'\\0' (U+0000)</c>")]
        [TestCase("DateTime", "<see cref=\"DateTime.MinValue\"/>")]
        [TestCase("double", "<c>0</c>")]
        [TestCase("float", "<c>0</c>")]
        [TestCase("int", "<c>0</c>")]
        [TestCase("long", "<c>0</c>")]
        [TestCase("object", "<see langword=\"null\"/>")]
        [TestCase("StringComparison", "<see cref=\"StringComparison.CurrentCulture\"/>")]
        public void Code_gets_fixed_for_documented_method_in_namespace_with_optional_value_with_missing_default_documentation_on_same_line_for_default_(string type, string defaultText)
        {
            var originalCode = @"
using System;
using System.Threading;

public namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""value"">Some value.</param>
        public bool DoSomething(" + type + @" value = default)
        {
        }
    }
}
";

            var fixedCode = @"
using System;
using System.Threading;

public namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""value"">Some value. The default is " + defaultText + @".</param>
        public bool DoSomething(" + type + @" value = default)
        {
        }
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("bool", "<see langword=\"false\"/>")]
        [TestCase("byte", "<c>0</c>")]
        [TestCase("CancellationToken", "<see cref=\"CancellationToken\"/>")]
        [TestCase("char", "<c>'\\0' (U+0000)</c>")]
        [TestCase("DateTime", "<see cref=\"DateTime.MinValue\"/>")]
        [TestCase("double", "<c>0</c>")]
        [TestCase("float", "<c>0</c>")]
        [TestCase("int", "<c>0</c>")]
        [TestCase("long", "<c>0</c>")]
        [TestCase("object", "<see langword=\"null\"/>")]
        [TestCase("StringComparison", "<see cref=\"StringComparison.CurrentCulture\"/>")]
        public void Code_gets_fixed_for_documented_method_in_namespace_with_optional_value_with_missing_default_documentation_on_same_line_for_default_typed_(string type, string defaultText)
        {
            var originalCode = @"
using System;
using System.Threading;

public namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""value"">Some value.</param>
        public bool DoSomething(" + type + " value = default(" + type + @"))
        {
        }
    }
}
";

            var fixedCode = @"
using System;
using System.Threading;

public namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""value"">Some value. The default is " + defaultText + @".</param>
        public bool DoSomething(" + type + " value = default(" + type + @"))
        {
        }
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2076_OptionalParameterDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2076_OptionalParameterDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2076_CodeFixProvider();
    }
}