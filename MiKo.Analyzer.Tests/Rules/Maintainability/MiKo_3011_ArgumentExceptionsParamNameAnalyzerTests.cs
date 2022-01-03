using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3011_ArgumentExceptionsParamNameAnalyzerTests : CodeFixVerifier
    {
        [TestCase("")]
        [TestCase("\"some message\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\", null")]
        [TestCase("nameof(x), \"some message\", null")]
        [TestCase("\"some message\", \"X\"")]
        [TestCase("\"some message\", nameof(TestMe)")]
        [TestCase("\"some message\", \"X\", null")]
        [TestCase("\"some message\", nameof(TestMe), null")]
        [TestCase("\"some message\", new Exception()")]
        public void No_issue_is_reported_for_incorrectly_thrown_ArgumentException_on_parameterless_method_(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        throw new ArgumentException(" + parameters + @");
    }
}
");

        [TestCase("\"some message\", \"x\"")]
        [TestCase("\"some message\", nameof(x)")]
        [TestCase("\"some message\", \"x\", null")]
        [TestCase("\"some message\", nameof(x), null")]
        public void No_issue_is_reported_for_correctly_thrown_ArgumentException_(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(" + parameters + @");
    }
}
");

        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\"")]
        [TestCase("nameof(x), \"some message\"")]
        public void No_issue_is_reported_for_correctly_thrown_ArgumentNullException_(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentNullException(" + parameters + @");
    }
}
");

        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\"")]
        [TestCase("nameof(x), \"some message\"")]
        [TestCase("\"x\", 42, \"some message\"")]
        [TestCase("nameof(x), 42, \"some message\"")]
        public void No_issue_is_reported_for_correctly_thrown_ArgumentOutOfRangeException_(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(" + parameters + @");
    }
}
");

        [TestCase("\"x\", 42, typeof(StringComparison)")]
        [TestCase("nameof(x), 42, typeof(StringComparison)")]
        public void No_issue_is_reported_for_correctly_thrown_InvalidEnumArgumentException_(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(StringComparison x)
    {
        if (x == StringComparison.OrdinalIgnoreCase) throw new InvalidEnumArgumentException(" + parameters + @");
    }
}
");

        [TestCase("")]
        [TestCase("\"some message\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\", null")]
        [TestCase("nameof(x), \"some message\", null")]
        [TestCase("\"some message\", \"X\"")]
        [TestCase("\"some message\", nameof(TestMe)")]
        [TestCase("\"some message\", \"X\", null")]
        [TestCase("\"some message\", nameof(TestMe), null")]
        [TestCase("\"some message\", new Exception()")]
        public void An_issue_is_reported_for_incorrectly_thrown_ArgumentException_(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(" + parameters + @");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_thrown_ArgumentException_with_string_literal() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException($""{x} is not allowed"");
    }
}
");

        [TestCase("")]
        [TestCase("\"X\"")]
        [TestCase("nameof(TestMe)")]
        [TestCase("\"X\", \"some message\"")]
        [TestCase("nameof(TestMe), \"some message\"")]
        [TestCase("\"some message\"")]
        [TestCase("\"some message\", \"x\"")]
        [TestCase("\"some message\", nameof(x)")]
        [TestCase("\"some message\", new Exception()")]
        public void An_issue_is_reported_for_incorrectly_thrown_ArgumentNullException_(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentNullException(" + parameters + @");
    }
}
");

        [TestCase("")]
        [TestCase("\"X\"")]
        [TestCase("nameof(TestMe)")]
        [TestCase("\"X\", \"some message\"")]
        [TestCase("nameof(TestMe), \"some message\"")]
        [TestCase("\"some message\"")]
        [TestCase("\"some message\", \"x\"")]
        [TestCase("\"some message\", nameof(x)")]
        [TestCase("\"some message\", new Exception()")]
        [TestCase("\"some message\", 42, \"x\"")]
        [TestCase("\"some message\", 42, nameof(x)")]
        public void An_issue_is_reported_for_incorrectly_thrown_ArgumentOutOfRangeException_(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(" + parameters + @");
    }
}
");

        [TestCase("")]
        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"X\"")]
        [TestCase("nameof(TestMe)")]
        [TestCase("\"some message\"")]
        [TestCase("\"some message\", new Exception()")]
        [TestCase("\"some message\", 42, typeof(StringComparison)")]
        [TestCase("\"some message\", 42, \"some message\"")]
        public void An_issue_is_reported_for_incorrectly_thrown_InvalidEnumArgumentException_(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new InvalidEnumArgumentException(" + parameters + @");
    }
}
");

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_no_parameters_on_property()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int Something
    {
        get => 42;
        set
        {
            if (value == 42) throw new ArgumentException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int Something
    {
        get => 42;
        set
        {
            if (value == 42) throw new ArgumentException(""TODO"", nameof(value));
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_no_parameters_on_indexer_key()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int this[int key]
    {
        get => 42;
        set
        {
            if (key == 42) throw new ArgumentException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int this[int key]
    {
        get => 42;
        set
        {
            if (key == 42) throw new ArgumentException(""TODO"", nameof(key));
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_no_parameters_on_indexer_value()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int this[int key]
    {
        get => 42;
        set
        {
            if (value == 42) throw new ArgumentException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int this[int key]
    {
        get => 42;
        set
        {
            if (value == 42) throw new ArgumentException(""TODO"", nameof(value));
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_no_parameters_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""TODO"", nameof(x));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_no_parameters_on_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y)
    {
        if (y == 42) throw new ArgumentException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y)
    {
        if (y == 42) throw new ArgumentException(""TODO"", nameof(y));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_message_only_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""some message"", nameof(x));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_message_only_on_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42) throw new ArgumentException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42) throw new ArgumentException(""some message"", nameof(y));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_parameter_name_only_instead_of_message_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""x"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""TODO"", nameof(x));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_parameter_name_only_instead_of_message_on_method_with_multiple_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y)
    {
        if (y == 42) throw new ArgumentException(""y"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y)
    {
        if (y == 42) throw new ArgumentException(""TODO"", nameof(y));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_message_only_and_some_comment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42)
        {
            // some comment
            throw new ArgumentException(""some message""); // some more comment
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42)
        {
            // some comment
            throw new ArgumentException(""some message"", nameof(x)); // some more comment
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_reversed_message_and_parameter_as_string()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""x"", ""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""some message"", nameof(x));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_reversed_message_and_parameter_as_nameof()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(nameof(x), ""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(""some message"", nameof(x));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_multiple_parameters_on_method_and_return_in_if_statement()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42)
            return;

        throw new ArgumentException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42)
            return;

        throw new ArgumentException(""TODO"", nameof(y));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_multiple_parameters_on_method_and_in_else_statement()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42)
            return;
        else
            throw new ArgumentException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42)
            return;
        else
            throw new ArgumentException(""TODO"", nameof(y));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentException_with_multiple_parameters_on_method_and_string_interpolation()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42)
            throw new ArgumentException($""'y' should be 42 but was {y}"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        if (y == 42)
            throw new ArgumentException($""'y' should be 42 but was {y}"", nameof(y));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_no_parameters_on_property()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object Something
    {
        get => null;
        set
        {
            if (value is null) throw new ArgumentNullException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object Something
    {
        get => null;
        set
        {
            if (value is null) throw new ArgumentNullException(nameof(value), ""TODO"");
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_no_parameters_on_indexer_key()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object this[object key]
    {
        get => null;
        set
        {
            if (key is null) throw new ArgumentNullException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object this[object key]
    {
        get => null;
        set
        {
            if (key is null) throw new ArgumentNullException(nameof(key), ""TODO"");
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_no_parameters_on_indexer_value()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object this[object key]
    {
        get => null;
        set
        {
            if (value is null) throw new ArgumentNullException();
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object this[object key]
    {
        get => null;
        set
        {
            if (value is null) throw new ArgumentNullException(nameof(value), ""TODO"");
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_no_parameters_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(nameof(x), ""TODO"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_no_parameters_on_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y)
    {
        if (y is null) throw new ArgumentNullException();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y)
    {
        if (y is null) throw new ArgumentNullException(nameof(y), ""TODO"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_message_only_on_method_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(nameof(x), ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_message_only_on_method_with_multiple_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y, object z)
    {
        if (y is null) throw new ArgumentNullException(""some message"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y, object z)
    {
        if (y is null) throw new ArgumentNullException(nameof(y), ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_message_only_and_some_comment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null)
        {
            // some comment
            throw new ArgumentNullException(""some message""); // some more comment
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null)
        {
            // some comment
            throw new ArgumentNullException(nameof(x), ""some message""); // some more comment
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_reversed_message_and_parameter_as_string()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(""some message"", ""x"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(nameof(x), ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_reversed_message_and_parameter_as_nameof()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(""some message"", nameof(x));
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x)
    {
        if (x is null) throw new ArgumentNullException(nameof(x), ""some message"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_thrown_ArgumentNullException_with_multiple_parameters_on_method_and_string_interpolation()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y, object z)
    {
        if (y is null)
            throw new ArgumentNullException($""'y' should be null but was {y}"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object x, object y, object z)
    {
        if (y is null)
            throw new ArgumentNullException(nameof(y), $""'y' should be null but was {y}"");
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        // TODO RKN: Codefix for ArgumentOutOfRangeException
        // TODO RKN: Codefix for InvalidEnumArgumentException
        // TODO RKN: Codefix for Properties, Indexers
        protected override string GetDiagnosticId() => MiKo_3011_ArgumentExceptionsParamNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3011_ArgumentExceptionsParamNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3011_CodeFixProvider();
    }
}