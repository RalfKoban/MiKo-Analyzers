using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2022_OutParamDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(out object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_non_out_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever</param>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_correct_comment_phrase() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>On successful return, contains the object.</param>
    public void DoSomething(out object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_correct_comment_phrase_for_bool() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>On successful return, indicates the object.</param>
    public void DoSomething(out bool o) { }
}
");

        [TestCase("whatever.")]
        [TestCase("Whatever.")]
        [TestCase("On true, returns something.")]
        [TestCase("On <see langword='true' />, returns something.")]
        [TestCase("On return, contains something.")]
        [TestCase("On return, indicates something.")]
        [TestCase("On successful return, indicates something.")]
        [TestCase("")]
        public void An_issue_is_reported_for_method_with_wrong_comment_phrase_(string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(out object o) { }
}
");

        [TestCase("On true, returns something.")]
        [TestCase("On false, returns something.")]
        [TestCase("On successful return, contains something.")]
        [TestCase("")]
        public void An_issue_is_reported_for_method_with_wrong_comment_phrase_on_bool_(string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='b'>" + comment + @"</param>
    public void DoSomething(out bool b) { }
}
");

        [TestCase("<summary />")]
        [TestCase("<inheritdoc />")]
        [TestCase("<exclude />")]
        public void No_issue_is_reported_for_method_with_missing_documentation_(string xmlElement) => No_issue_is_reported_for(@"
public class TestMe
{
    /// " + xmlElement + @"
    public void DoSomething(out object o) { }
}
");

        [TestCase("[out] The something.")]
        [TestCase("[Out] The something.")]
        [TestCase("[OUT] The something.")]
        [TestCase("[out]: The something.")]
        [TestCase("[Out]: The something.")]
        [TestCase("[OUT]: The something.")]
        [TestCase("flag indicating the something.")]
        [TestCase("Flag indicating the something.")]
        [TestCase("flag that indicates the something.")]
        [TestCase("Flag that indicates the something.")]
        [TestCase("flag which indicates the something.")]
        [TestCase("Flag which indicates the something.")]
        [TestCase("Indicates the something.")]
        [TestCase("Indicating the something.")]
        [TestCase("out - The something.")]
        [TestCase("Out - The something.")]
        [TestCase("OUT - The something.")]
        [TestCase("out -The something.")]
        [TestCase("Out -The something.")]
        [TestCase("OUT The something.")]
        [TestCase("OUT -The something.")]
        [TestCase("out: The something.")]
        [TestCase("Out: The something.")]
        [TestCase("OUT: The something.")]
        [TestCase("Specifies the something.")]
        [TestCase("The something.")]
        public void Code_gets_fixed_for_boolean_out_parameter_(string text)
        {
            var originalCode = @"
using System.Windows;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + text + @"</param>
    public void DoSomething(out bool o) { }
}";

            const string FixedCode = @"
using System.Windows;

public class TestMe
{
    /// <summary />
    /// <param name='o'>On successful return, indicates the something.</param>
    public void DoSomething(out bool o) { }
}";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase("[out] parameter that returns the object.")]
        [TestCase("[Out] parameter that returns the object.")]
        [TestCase("[OUT] parameter that returns the object.")]
        [TestCase("Contains the object.")]
        [TestCase("Indicates the object.")]
        [TestCase("On return contains the object.")]
        [TestCase("On return, contains the object.")]
        [TestCase("out - The object.")]
        [TestCase("out parameter that returns the object.")]
        [TestCase("Out parameter that returns the object.")]
        [TestCase("out The object.")]
        [TestCase("out -The object.")]
        [TestCase("OUT The object.")]
        [TestCase("out: The object.")]
        [TestCase("Provides the object.")]
        [TestCase("return the object.")]
        [TestCase("Return the object.")]
        [TestCase("Returned on the object.")]
        [TestCase("Returned when the object.")]
        [TestCase("returns the object.")]
        [TestCase("Returns the object.")]
        [TestCase("The object.")]
        [TestCase("To return the object.")]
        [TestCase("When the method returns, the object.")]
        [TestCase("When this method returns, the object.")]
        [TestCase("Will be the object.")]
        [TestCase("Will contain the object.")]
        public void Code_gets_fixed_for_out_parameter_(string text)
        {
            var originalCode = @"
using System.Windows;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + text + @"</param>
    public void DoSomething(out object o) { }
}";

            const string FixedCode = @"
using System.Windows;

public class TestMe
{
    /// <summary />
    /// <param name='o'>On successful return, contains the object.</param>
    public void DoSomething(out object o) { }
}";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2022_OutParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2022_OutParamDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2022_CodeFixProvider();
    }
}