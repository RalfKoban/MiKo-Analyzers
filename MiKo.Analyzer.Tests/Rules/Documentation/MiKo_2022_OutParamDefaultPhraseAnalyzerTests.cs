using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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

        [TestCase("A Flag indicating the something.")]
        [TestCase("A Flag that indicates the something.")]
        [TestCase("A Flag which indicates the something.")]
        [TestCase("A flag indicating the something.")]
        [TestCase("A flag that indicates the something.")]
        [TestCase("A flag which indicates the something.")]
        [TestCase("A value indicating the something.")]
        [TestCase("A value that indicates the something.")]
        [TestCase("A value which indicates the something.")]
        [TestCase("After return contains a flag indicating the something.")]
        [TestCase("After return contains a flag that indicates the something.")]
        [TestCase("After return contains a flag which indicates the something.")]
        [TestCase("After return contains a value indicating the something.")]
        [TestCase("After return contains a value that indicates the something.")]
        [TestCase("After return contains a value which indicates the something.")]
        [TestCase("After return provides a flag indicating the something.")]
        [TestCase("After return provides a flag that indicates the something.")]
        [TestCase("After return provides a flag which indicates the something.")]
        [TestCase("After return provides a value indicating the something.")]
        [TestCase("After return provides a value that indicates the something.")]
        [TestCase("After return provides a value which indicates the something.")]
        [TestCase("After return receives a flag indicating the something.")]
        [TestCase("After return receives a flag that indicates the something.")]
        [TestCase("After return receives a flag which indicates the something.")]
        [TestCase("After return receives a value indicating the something.")]
        [TestCase("After return receives a value that indicates the something.")]
        [TestCase("After return receives a value which indicates the something.")]
        [TestCase("After return, contains a flag indicating the something.")]
        [TestCase("After return, contains a flag that indicates the something.")]
        [TestCase("After return, contains a flag which indicates the something.")]
        [TestCase("After return, contains a value indicating the something.")]
        [TestCase("After return, contains a value that indicates the something.")]
        [TestCase("After return, contains a value which indicates the something.")]
        [TestCase("After return, provides a flag indicating the something.")]
        [TestCase("After return, provides a flag that indicates the something.")]
        [TestCase("After return, provides a flag which indicates the something.")]
        [TestCase("After return, provides a value indicating the something.")]
        [TestCase("After return, provides a value that indicates the something.")]
        [TestCase("After return, provides a value which indicates the something.")]
        [TestCase("After return, receives a flag indicating the something.")]
        [TestCase("After return, receives a flag that indicates the something.")]
        [TestCase("After return, receives a flag which indicates the something.")]
        [TestCase("After return, receives a value indicating the something.")]
        [TestCase("After return, receives a value that indicates the something.")]
        [TestCase("After return, receives a value which indicates the something.")]
        [TestCase("After successful return contains a flag indicating the something.")]
        [TestCase("After successful return contains a flag that indicates the something.")]
        [TestCase("After successful return contains a flag which indicates the something.")]
        [TestCase("After successful return contains a value indicating the something.")]
        [TestCase("After successful return contains a value that indicates the something.")]
        [TestCase("After successful return contains a value which indicates the something.")]
        [TestCase("After successful return indicates the something.")]
        [TestCase("After successful return provides a flag indicating the something.")]
        [TestCase("After successful return provides a flag that indicates the something.")]
        [TestCase("After successful return provides a flag which indicates the something.")]
        [TestCase("After successful return provides a value indicating the something.")]
        [TestCase("After successful return provides a value that indicates the something.")]
        [TestCase("After successful return provides a value which indicates the something.")]
        [TestCase("After successful return receives a flag indicating the something.")]
        [TestCase("After successful return receives a flag that indicates the something.")]
        [TestCase("After successful return receives a flag which indicates the something.")]
        [TestCase("After successful return receives a value indicating the something.")]
        [TestCase("After successful return receives a value that indicates the something.")]
        [TestCase("After successful return receives a value which indicates the something.")]
        [TestCase("After successful return, contains a flag indicating the something.")]
        [TestCase("After successful return, contains a flag that indicates the something.")]
        [TestCase("After successful return, contains a flag which indicates the something.")]
        [TestCase("After successful return, contains a value indicating the something.")]
        [TestCase("After successful return, contains a value that indicates the something.")]
        [TestCase("After successful return, contains a value which indicates the something.")]
        [TestCase("After successful return, indicates the something.")]
        [TestCase("After successful return, provides a flag indicating the something.")]
        [TestCase("After successful return, provides a flag that indicates the something.")]
        [TestCase("After successful return, provides a flag which indicates the something.")]
        [TestCase("After successful return, provides a value indicating the something.")]
        [TestCase("After successful return, provides a value that indicates the something.")]
        [TestCase("After successful return, provides a value which indicates the something.")]
        [TestCase("After successful return, receives a flag indicating the something.")]
        [TestCase("After successful return, receives a flag that indicates the something.")]
        [TestCase("After successful return, receives a flag which indicates the something.")]
        [TestCase("After successful return, receives a value indicating the something.")]
        [TestCase("After successful return, receives a value that indicates the something.")]
        [TestCase("After successful return, receives a value which indicates the something.")]
        [TestCase("Flag indicating the something.")]
        [TestCase("Flag that indicates the something.")]
        [TestCase("Flag which indicates the something.")]
        [TestCase("On return contains a flag indicating the something.")]
        [TestCase("On return contains a flag that indicates the something.")]
        [TestCase("On return contains a flag which indicates the something.")]
        [TestCase("On return contains a value indicating the something.")]
        [TestCase("On return contains a value that indicates the something.")]
        [TestCase("On return contains a value which indicates the something.")]
        [TestCase("On return provides a flag indicating the something.")]
        [TestCase("On return provides a flag that indicates the something.")]
        [TestCase("On return provides a flag which indicates the something.")]
        [TestCase("On return provides a value indicating the something.")]
        [TestCase("On return provides a value that indicates the something.")]
        [TestCase("On return provides a value which indicates the something.")]
        [TestCase("On return receives a flag indicating the something.")]
        [TestCase("On return receives a flag that indicates the something.")]
        [TestCase("On return receives a flag which indicates the something.")]
        [TestCase("On return receives a value indicating the something.")]
        [TestCase("On return receives a value that indicates the something.")]
        [TestCase("On return receives a value which indicates the something.")]
        [TestCase("On return, contains a flag indicating the something.")]
        [TestCase("On return, contains a flag that indicates the something.")]
        [TestCase("On return, contains a flag which indicates the something.")]
        [TestCase("On return, contains a value indicating the something.")]
        [TestCase("On return, contains a value that indicates the something.")]
        [TestCase("On return, contains a value which indicates the something.")]
        [TestCase("On return, provides a flag indicating the something.")]
        [TestCase("On return, provides a flag that indicates the something.")]
        [TestCase("On return, provides a flag which indicates the something.")]
        [TestCase("On return, provides a value indicating the something.")]
        [TestCase("On return, provides a value that indicates the something.")]
        [TestCase("On return, provides a value which indicates the something.")]
        [TestCase("On return, receives a flag indicating the something.")]
        [TestCase("On return, receives a flag that indicates the something.")]
        [TestCase("On return, receives a flag which indicates the something.")]
        [TestCase("On return, receives a value indicating the something.")]
        [TestCase("On return, receives a value that indicates the something.")]
        [TestCase("On return, receives a value which indicates the something.")]
        [TestCase("Indicates the something.")]
        [TestCase("Indicating the something.")]
        [TestCase("OUT - The something.")]
        [TestCase("OUT -The something.")]
        [TestCase("OUT The something.")]
        [TestCase("OUT: The something.")]
        [TestCase("Out - The something.")]
        [TestCase("Out -The something.")]
        [TestCase("Out The something.")]
        [TestCase("Out: The something.")]
        [TestCase("Specifies the something.")]
        [TestCase("The something.")]
        [TestCase("Value indicating the something.")]
        [TestCase("Value that indicates the something.")]
        [TestCase("Value which indicates the something.")]
        [TestCase("[OUT] The something.")]
        [TestCase("[OUT]: The something.")]
        [TestCase("[Out] The something.")]
        [TestCase("[Out]: The something.")]
        [TestCase("[out] The something.")]
        [TestCase("[out]: The something.")]
        [TestCase("flag indicating the something.")]
        [TestCase("flag that indicates the something.")]
        [TestCase("flag which indicates the something.")]
        [TestCase("out - The something.")]
        [TestCase("out -The something.")]
        [TestCase("out: The something.")]
        [TestCase("value indicating the something.")]
        [TestCase("value that indicates the something.")]
        [TestCase("value which indicates the something.")]
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

        [TestCase("A variable that receives the object.")]
        [TestCase("A variable which receives the object.")]
        [TestCase("After return contains the object.")]
        [TestCase("After return provides the object.")]
        [TestCase("After return receives the object.")]
        [TestCase("After return, contains the object.")]
        [TestCase("After return, provides the object.")]
        [TestCase("After return, receives the object.")]
        [TestCase("After successful return contains the object.")]
        [TestCase("After successful return provides the object.")]
        [TestCase("After successful return receives the object.")]
        [TestCase("After successful return, contains the object.")]
        [TestCase("After successful return, provides the object.")]
        [TestCase("After successful return, receives the object.")]
        [TestCase("Contains the object.")]
        [TestCase("Indicates the object.")]
        [TestCase("OUT The object.")]
        [TestCase("On return contains the object.")]
        [TestCase("On return provides the object.")]
        [TestCase("On return receives the object.")]
        [TestCase("On return, contains the object.")]
        [TestCase("On return, provides the object.")]
        [TestCase("On return, receives the object.")]
        [TestCase("Out parameter that contains the object.")]
        [TestCase("Out parameter that provides the object.")]
        [TestCase("Out parameter that receives the object.")]
        [TestCase("Out parameter that returns the object.")]
        [TestCase("Out parameter which contains the object.")]
        [TestCase("Out parameter which provides the object.")]
        [TestCase("Out parameter which receives the object.")]
        [TestCase("Out parameter which returns the object.")]
        [TestCase("Out parameter, contains the object.")]
        [TestCase("Out parameter, provides the object.")]
        [TestCase("Out parameter, receives the object.")]
        [TestCase("Out parameter, returns the object.")]
        [TestCase("Provides the object.")]
        [TestCase("Receives the object.")]
        [TestCase("Return the object.")]
        [TestCase("Returned on the object.")]
        [TestCase("Returned when the object.")]
        [TestCase("Returns the object.")]
        [TestCase("The object.")]
        [TestCase("To return the object.")]
        [TestCase("When the method returns, contains the object.")]
        [TestCase("When the method returns, indicates the object.")]
        [TestCase("When the method returns, provides the object.")]
        [TestCase("When the method returns, receives the object.")]
        [TestCase("When the method returns, the object.")]
        [TestCase("When this method returns, contains the object.")]
        [TestCase("When this method returns, indicates the object.")]
        [TestCase("When this method returns, provides the object.")]
        [TestCase("When this method returns, receives the object.")]
        [TestCase("When this method returns, the object.")]
        [TestCase("Will be the object.")]
        [TestCase("Will contain the object.")]
        [TestCase("Will provide the object.")]
        [TestCase("Will receive the object.")]
        [TestCase("Will return the object.")]
        [TestCase("[OUT] parameter that contains the object.")]
        [TestCase("[OUT] parameter that provides the object.")]
        [TestCase("[OUT] parameter that receives the object.")]
        [TestCase("[OUT] parameter that returns the object.")]
        [TestCase("[OUT] parameter which contains the object.")]
        [TestCase("[OUT] parameter which provides the object.")]
        [TestCase("[OUT] parameter which receives the object.")]
        [TestCase("[OUT] parameter which returns the object.")]
        [TestCase("[OUT] parameter, contains the object.")]
        [TestCase("[OUT] parameter, provides the object.")]
        [TestCase("[OUT] parameter, receives the object.")]
        [TestCase("[OUT] parameter, returns the object.")]
        [TestCase("[Out] parameter that contains the object.")]
        [TestCase("[Out] parameter that provides the object.")]
        [TestCase("[Out] parameter that receives the object.")]
        [TestCase("[Out] parameter that returns the object.")]
        [TestCase("[Out] parameter which contains the object.")]
        [TestCase("[Out] parameter which provides the object.")]
        [TestCase("[Out] parameter which receives the object.")]
        [TestCase("[Out] parameter which returns the object.")]
        [TestCase("[Out] parameter, contains the object.")]
        [TestCase("[Out] parameter, provides the object.")]
        [TestCase("[Out] parameter, receives the object.")]
        [TestCase("[Out] parameter, returns the object.")]
        [TestCase("[out] parameter that contains the object.")]
        [TestCase("[out] parameter that provides the object.")]
        [TestCase("[out] parameter that receives the object.")]
        [TestCase("[out] parameter that returns the object.")]
        [TestCase("[out] parameter which contains the object.")]
        [TestCase("[out] parameter which provides the object.")]
        [TestCase("[out] parameter which receives the object.")]
        [TestCase("[out] parameter which returns the object.")]
        [TestCase("[out] parameter, contains the object.")]
        [TestCase("[out] parameter, provides the object.")]
        [TestCase("[out] parameter, receives the object.")]
        [TestCase("[out] parameter, returns the object.")]
        [TestCase("out - The object.")]
        [TestCase("out -The object.")]
        [TestCase("out The object.")]
        [TestCase("out parameter that contains the object.")]
        [TestCase("out parameter that provides the object.")]
        [TestCase("out parameter that receives the object.")]
        [TestCase("out parameter that returns the object.")]
        [TestCase("out parameter which contains the object.")]
        [TestCase("out parameter which provides the object.")]
        [TestCase("out parameter which receives the object.")]
        [TestCase("out parameter which returns the object.")]
        [TestCase("out parameter, contains the object.")]
        [TestCase("out parameter, provides the object.")]
        [TestCase("out parameter, receives the object.")]
        [TestCase("out parameter, returns the object.")]
        [TestCase("out: The object.")]
        [TestCase("return the object.")]
        [TestCase("returns the object.")]
        [TestCase("will contain the object.")]
        [TestCase("will provide the object.")]
        [TestCase("will receive the object.")]
        [TestCase("will return the object.")]
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

        [Test]
        public void Code_gets_fixed_for_empty_out_parameter_on_single_line()
        {
            const string OriginalCode = @"
using System.Windows;

public class TestMe
{
    /// <summary />
    /// <param name='o'></param>
    public void DoSomething(out object o) { }
}";

            const string FixedCode = @"
using System.Windows;

public class TestMe
{
    /// <summary />
    /// <param name='o'>On successful return, contains TODO.</param>
    public void DoSomething(out object o) { }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_out_parameter_on_separate_lines()
        {
            const string OriginalCode = @"
using System.Windows;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// </param>
    public void DoSomething(out object o) { }
}";

            const string FixedCode = @"
using System.Windows;

public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// On successful return, contains TODO.
    /// </param>
    public void DoSomething(out object o) { }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2022_OutParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2022_OutParamDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2022_CodeFixProvider();
    }
}