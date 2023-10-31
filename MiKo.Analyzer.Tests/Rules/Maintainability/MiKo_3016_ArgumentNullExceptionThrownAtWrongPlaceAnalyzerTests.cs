using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3016_ArgumentNullExceptionThrownAtWrongPlaceAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ExceptionNames =
                                                          {
                                                              nameof(ArgumentNullException),
                                                              typeof(ArgumentNullException).FullName,
                                                          };

        [Test]
        public void No_issue_is_reported_for_coalescence_based_correctly_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o ?? throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_coalescence_based_if_independent_correctly_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            throw new ArgumentException();
        }

        var x = s ?? throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_pattern_based_correctly_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (o is null) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_binary_based_correctly_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (o == null) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Equals_based_correctly_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (Equals(o, null)) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ReferenceEquals_based_correctly_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (ReferenceEquals(o, null)) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void No_issue_is_reported_on_pattern_based_parameter_property_comparison_for_thrown_([Values(nameof(ArgumentException), nameof(InvalidOperationException))] string exceptionName) => No_issue_is_reported_for(@"
using System;

public class SomeObject
{
    public object SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        if (o.SomeData is null) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void No_issue_is_reported_on_binary_based_parameter_property_comparison_for_thrown_([Values(nameof(ArgumentException), nameof(InvalidOperationException))] string exceptionName) => No_issue_is_reported_for(@"
using System;

public class SomeObject
{
    public object SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        if (o.SomeData == null) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void No_issue_is_reported_on_pattern_local_property_comparison_for_thrown_InvalidOperationException() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object SomeData { get; set; }

    public void DoSomething()
    {
        if (SomeData is null) throw new InvalidOperationException();
    }
}
");

        [Test]
        public void No_issue_is_reported_on_binary_based_local_property_comparison_for_thrown_InvalidOperationException() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object SomeData { get; set; }

    public void DoSomething()
    {
        if (SomeData == null) throw new InvalidOperationException();
    }
}
");

        [Test]
        public void An_issue_is_reported_on_pattern_based_parameter_property_comparison_for_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => An_issue_is_reported_for(@"
using System;

public class SomeObject
{
    public object SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        if (o.SomeData is null) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void An_issue_is_reported_on_binary_based_parameter_property_comparison_for_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => An_issue_is_reported_for(@"
using System;

public class SomeObject
{
    public object SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        if (o.SomeData == null) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void An_issue_is_reported_on_pattern_based_local_property_comparison_for_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object SomeData { get; set; }

    public void DoSomething()
    {
        if (SomeData is null) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void An_issue_is_reported_on_binary_based_local_property_comparison_for_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object SomeData { get; set; }

    public void DoSomething()
    {
        if (SomeData == null) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void An_issue_is_reported_on_method_with_some_arguments_but_pattern_based_local_property_comparison_for_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object SomeData { get; set; }

    public void DoSomething(object o)
    {
        if (SomeData is null) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void An_issue_is_reported_on_method_with_some_arguments_but_binary_based_local_property_comparison_for_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object SomeData { get; set; }

    public void DoSomething(object o)
    {
        if (SomeData == null) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_string_IsNullOrWhiteSpace_comparison_for_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s)
    {
        if (string.IsNullOrEmpty(s)) throw new " + exceptionName + @"();
    }
}
");

        [TestCase("==", "ArgumentNullException()", @"ArgumentException(""TODO"", nameof(o))")]
        [TestCase("==", @"ArgumentNullException(""o"")", @"ArgumentException(""TODO"", nameof(o))")]
        [TestCase("==", @"ArgumentNullException(nameof(o), ""some message"")", @"ArgumentException(""some message"", nameof(o))")]
        [TestCase("is", "ArgumentNullException()", @"ArgumentException(""TODO"", nameof(o))")]
        [TestCase("is", "ArgumentNullException(nameof(o))", @"ArgumentException(""TODO"", nameof(o))")]
        [TestCase("is", @"ArgumentNullException(""o"", ""some message"")", @"ArgumentException(""some message"", nameof(o))")]
        public void Code_gets_fixed_for_parameter_property_comparison_for_thrown_ArgumentNullException(string comparison, string originalCode, string fixedCode)
        {
            const string Template = @"
using System;

public class SomeObject
{
    public object SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        if (o.SomeData #1# null) throw new #2#;
    }
}
";

            VerifyCSharpFix(Template.Replace("#1#", comparison).Replace("#2#", originalCode), Template.Replace("#1#", comparison).Replace("#2#", fixedCode));
        }

        [TestCase("==", "ArgumentNullException()", @"InvalidOperationException(""TODO"")")]
        [TestCase("is", @"ArgumentNullException(""SomeData"")", @"InvalidOperationException(""TODO"")")]
        [TestCase("==", @"ArgumentNullException(nameof(SomeData), ""some message"")", @"InvalidOperationException(""some message"")")]
        [TestCase("is", "ArgumentNullException()", @"InvalidOperationException(""TODO"")")]
        [TestCase("is", "ArgumentNullException(nameof(SomeData))", @"InvalidOperationException(""TODO"")")]
        [TestCase("is", @"ArgumentNullException(""SomeData"", ""some message"")", @"InvalidOperationException(""some message"")")]
        public void Code_gets_fixed_for_local_property_comparison_for_thrown_ArgumentNullException(string comparison, string originalCode, string fixedCode)
        {
            const string Template = @"
using System;

public class TestMe
{
    public object SomeData { get; set; }

    public void DoSomething()
    {
        if (SomeData #1# null) throw new #2#;
    }
}
";

            VerifyCSharpFix(Template.Replace("#1#", comparison).Replace("#2#", originalCode), Template.Replace("#1#", comparison).Replace("#2#", fixedCode));
        }

        [TestCase("==", "ArgumentNullException()", @"InvalidOperationException(""TODO"")")]
        [TestCase("==", @"ArgumentNullException(""o"")", @"InvalidOperationException(""TODO"")")]
        [TestCase("==", @"ArgumentNullException(nameof(o), ""some message"")", @"InvalidOperationException(""some message"")")]
        [TestCase("is", "ArgumentNullException()", @"InvalidOperationException(""TODO"")")]
        [TestCase("is", "ArgumentNullException(nameof(o))", @"InvalidOperationException(""TODO"")")]
        [TestCase("is", @"ArgumentNullException(""o"", ""some message"")", @"InvalidOperationException(""some message"")")]
        public void Code_gets_fixed_for_method_with_some_arguments_but_local_property_comparison_for_thrown_ArgumentNullException(string comparison, string originalCode, string fixedCode)
        {
            const string Template = @"
using System;

public class TestMe
{
    public object SomeData { get; set; }

    public void DoSomething(object o)
    {
        if (SomeData #1# null) throw new #2#;
    }
}
";

            VerifyCSharpFix(Template.Replace("#1#", comparison).Replace("#2#", originalCode), Template.Replace("#1#", comparison).Replace("#2#", fixedCode));
        }

        [Test]
        public void Code_gets_fixed_for_string_IsNullOrWhiteSpace_comparison_for_thrown_ArgumentNullException()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object SomeData { get; set; }

    public void DoSomething(string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentNullException(""s"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object SomeData { get; set; }

    public void DoSomething(string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentException(""TODO"", nameof(s));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3016_ArgumentNullExceptionThrownAtWrongPlaceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3016_ArgumentNullExceptionThrownAtWrongPlaceAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3016_CodeFixProvider();
    }
}