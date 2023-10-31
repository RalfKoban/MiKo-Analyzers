using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2036_PropertyDefaultValuePhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] BooleanReturnValues =
                                                               {
                                                                   "bool",
                                                                   "Boolean",
                                                                   "System.Boolean",
                                                               };

        private CodeFixProvider CurrentCodeFixProvider { get; set; }

        [Test, Combinatorial]
        public void No_issue_is_reported_for_commented_method_([ValueSource(nameof(BooleanReturnValues))] string returnType, [Values("returns", "value")] string xmlTag) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// Something.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property_([ValueSource(nameof(BooleanReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_method_that_returns_a_(
                                                                [Values("returns", "value")] string xmlTag,
                                                                [Values("void", "int", "Task", "Task<int>", "Task<string>")] string returnType) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// Something.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => throw new NotSupportedException();
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_commented_Boolean_property_with_missing_default_value_(
                                                                                                [Values("returns", "value")] string xmlTag,
                                                                                                [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                                [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                                [ValueSource(nameof(BooleanReturnValues))] string returnType)
            => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + trueValue + " if something happens; otherwise, " + falseValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Boolean_property_with_default_phrase_(
                                                                                                   [Values("returns", "value")] string xmlTag,
                                                                                                   [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                                   [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                                   [Values("<see langword=\"true\"/>", "<see langword=\"false\"/>")] string defaultValue,
                                                                                                   [ValueSource(nameof(BooleanReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + trueValue + " if something happens; otherwise, " + falseValue + ". The default is " + defaultValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Boolean_property_with_default_phrase_and_line_break_(
                                                                                                                  [Values("returns", "value")] string xmlTag,
                                                                                                                  [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                                                  [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                                                  [Values("<see langword=\"true\"/>", "<see langword=\"false\"/>")] string defaultValue,
                                                                                                                  [ValueSource(nameof(BooleanReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + trueValue + " if something happens; otherwise, " + falseValue + @"
    /// The default is " + defaultValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void An_issue_is_reported_for_commented_Enum_property_with_missing_default_value_([Values("returns", "value")] string xmlTag) => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public enum MyEnum
{
    A = 0,
    B,
}

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// One of the MyEnum values.
    /// </" + xmlTag + @">
    public MyEnum DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_commented_Enum_property_with_default_value_([Values("returns", "value")] string xmlTag) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public enum MyEnum
{
    A = 0,
    B,
}

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// One of the MyEnum values. The default is <see cref=""MyEnum.A""/>.
    /// </" + xmlTag + @">
    public MyEnum DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_commented_Enum_property_with_default_value_and_line_break_([Values("returns", "value")] string xmlTag) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public enum MyEnum
{
    A = 0,
    B,
}

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// One of the MyEnum values.
    /// The default is <see cref=""MyEnum.A"" />.
    /// </" + xmlTag + @">
    public MyEnum DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_commented_Enum_property_with_explicitly_commented_no_default_value_([Values("returns", "value")] string xmlTag) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public enum MyEnum
{
    A = 0,
    B,
}

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// One of the MyEnum values.
    /// This property has no default value.
    /// </" + xmlTag + @">
    public MyEnum DoSomething { get; set; }
}
");

        [Test]
        public void Code_gets_fixed_for_boolean_having_no_default_value()
        {
            CurrentCodeFixProvider = new MiKo_2036_NoDefault_CodeFixProvider();

            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <value>
    /// <see langword=""true""/> if something happens; otherwise, <see langword=""false""/>.
    /// </value>
    public bool DoSomething { get; set; }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <value>
    /// <see langword=""true""/> if something happens; otherwise, <see langword=""false""/>.
    /// This property has no default value.
    /// </value>
    public bool DoSomething { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_boolean_having_false_as_default_value()
        {
            CurrentCodeFixProvider = new MiKo_2036_DefaultFalse_CodeFixProvider();

            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <value>
    /// <see langword=""true""/> if something happens; otherwise, <see langword=""false""/>.
    /// </value>
    public bool DoSomething { get; set; }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <value>
    /// <see langword=""true""/> if something happens; otherwise, <see langword=""false""/>.
    /// The default is <see langword=""false""/>.
    /// </value>
    public bool DoSomething { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_boolean_having_true_as_default_value()
        {
            CurrentCodeFixProvider = new MiKo_2036_DefaultTrue_CodeFixProvider();

            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <value>
    /// <see langword=""true""/> if something happens; otherwise, <see langword=""false""/>.
    /// </value>
    public bool DoSomething { get; set; }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <value>
    /// <see langword=""true""/> if something happens; otherwise, <see langword=""false""/>.
    /// The default is <see langword=""true""/>.
    /// </value>
    public bool DoSomething { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Enum_as_default_value()
        {
            CurrentCodeFixProvider = new MiKo_2036_Enum_CodeFixProvider();

            const string OriginalCode = @"
using System;

public enum MyEnum
{
    A = 0,
    B,
}

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <value>
    /// One of the MyEnum values.
    /// </value>
    public MyEnum DoSomething { get; set; }
}
";

            const string FixedCode = @"
using System;

public enum MyEnum
{
    A = 0,
    B,
}

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <value>
    /// One of the MyEnum values.
    /// The default is <see cref=""MyEnum.A""/>.
    /// </value>
    public MyEnum DoSomething { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2036_PropertyDefaultValuePhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2036_PropertyDefaultValuePhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => CurrentCodeFixProvider;
    }
}