using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2313_CommentShouldBeDocumentationAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] DocumentationCommentMarkers = ["Summary:", "Remark:", "Remarks:", "Return:", "Returns:", "Parameter:", "Parameters:", "Param:", "Exception:", "Exceptions:", "Value:", "ReturnValue:", "Return value:"];

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
    }
}");

        [Test]
        public void No_issue_is_reported_for_comment_before_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        // some comment
        public void DoSomething()
        {
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_comment_within_method_([ValueSource(nameof(DocumentationCommentMarkers))] string comment) => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            // " + comment + @" some comment
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_comment_after_method_([ValueSource(nameof(DocumentationCommentMarkers))] string comment) => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething() => 42; // " + comment + @" some comment
    }
}");

        [Test]
        public void An_issue_is_reported_for_comment_before_method_([ValueSource(nameof(DocumentationCommentMarkers))] string comment) => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        // " + comment + @"
        //     some comment
        public void DoSomething()
        {
        }
    }
}");

        [Test]
        public void Code_gets_fixed_for_summary_comment_before_type()
        {
            const string OriginalCode = @"
namespace Bla
{
    // Summary:
    //     some comment
    public class TestMe
    {
        public void DoSomething()
        {
        }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    /// <summary>
    /// some comment
    /// </summary>
    public class TestMe
    {
        public void DoSomething()
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_summary_comment_before_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        // Summary:
        //     some comment
        public void DoSomething()
        {
        }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// some comment
        /// </summary>
        public void DoSomething()
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_summary_comment_before_enum_type()
        {
            const string OriginalCode = @"
namespace Bla
{
    // Summary:
    //     some comment
    public enum TestMe
    {
        Something = 0,
    }
}";

            const string FixedCode = @"
namespace Bla
{
    /// <summary>
    /// some comment
    /// </summary>
    public enum TestMe
    {
        Something = 0,
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_summary_comment_before_enum_member()
        {
            const string OriginalCode = @"
namespace Bla
{
    public enum TestMe
    {
        // Summary:
        //     some comment
        Something = 1,
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public enum TestMe
    {
        /// <summary>
        /// some comment
        /// </summary>
        Something = 1,
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_summary_comment_before_every_enum_member()
        {
            const string OriginalCode = @"
namespace Bla
{
    public enum TestMe
    {
        // Summary:
        //     this is nothing
        Nothing = 0,

        // Summary:
        //     this is a something comment
        Something = 1,

        // Summary:
        //     this is an anything comment
        Anything = 2,
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public enum TestMe
    {
        /// <summary>
        /// this is nothing
        /// </summary>
        Nothing = 0,

        /// <summary>
        /// this is a something comment
        /// </summary>
        Something = 1,

        /// <summary>
        /// this is an anything comment
        /// </summary>
        Anything = 2,
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_comment_when_value_is_at_end()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        // Summary:
        //     Gets the error message for the property with the given name.
        //
        // Value:
        //     The error message for the property. The default is an empty string ("""").
        public string ErrorMessage { get; }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <value>
        /// The error message for the property. The default is an empty string ("""").
        /// </value>
        public string ErrorMessage { get; }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_comment_when_remark_is_at_end_([Values("Remark", "Remarks")] string marker)
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        // Summary:
        //     Gets the error message for the property with the given name.
        //
        // ###:
        //     The error message for the property. The default is an empty string ("""").
        public string ErrorMessage { get; }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <remarks>
        /// The error message for the property. The default is an empty string ("""").
        /// </remarks>
        public string ErrorMessage { get; }
    }
}";

            VerifyCSharpFix(OriginalCode.Replace("###", marker), FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_comment_when_returns_is_at_end_([Values("Return", "Returns", "Return value", "ReturnValue")] string marker)
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        // Summary:
        //     Gets the error message for the property with the given name.
        //
        // ###:
        //     The error message for the property. The default is an empty string ("""").
        public string GetErrorMessage()
        {
            return ""some value"";
        }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <returns>
        /// The error message for the property. The default is an empty string ("""").
        /// </returns>
        public string GetErrorMessage()
        {
            return ""some value"";
        }
    }
}";

            VerifyCSharpFix(OriginalCode.Replace("###", marker), FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_comment_with_parameters_when_returns_is_at_end()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        // Summary:
        //     Gets the error message for the property with the given name.
        //
        // Parameters:
        //   columnName:
        //     The name of the property whose error message to get.
        //
        // Returns:
        //     The error message for the property. The default is an empty string ("""").
        public string GetErrorMessage(string columnName)
        {
            return ""some value"";
        }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name=""columnName"">
        /// The name of the property whose error message to get.
        /// </param>
        /// <returns>
        /// The error message for the property. The default is an empty string ("""").
        /// </returns>
        public string GetErrorMessage(string columnName)
        {
            return ""some value"";
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_comment_with_parameters_when_parameters_comments_are_at_end()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        // Summary:
        //     Gets the error message for the property with the given name.
        //
        // Returns:
        //     The error message for the property. The default is an empty string ("""").
        //
        // Parameters:
        //   columnName:
        //     The name of the property whose error message to get.
        public string GetErrorMessage(string columnName)
        {
            return ""some value"";
        }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name=""columnName"">
        /// The name of the property whose error message to get.
        /// </param>
        /// <returns>
        /// The error message for the property. The default is an empty string ("""").
        /// </returns>
        public string GetErrorMessage(string columnName)
        {
            return ""some value"";
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_comment_with_parameters_when_parameters_comments_are_at_end_and_on_single_lines()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        // Summary:
        //     Gets the error message for the property with the given name.
        //
        // Returns:
        //     The error message for the property. The default is an empty string ("""").
        //
        // Parameters:
        //   columnName1: The name of the property whose error message to get.
        //   columnName2: Some other information to get.
        public string GetErrorMessage(string columnName1, string columnName2)
        {
            return ""some value"";
        }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name=""columnName1"">
        /// The name of the property whose error message to get.
        /// </param>
        /// <param name=""columnName2"">
        /// Some other information to get.
        /// </param>
        /// <returns>
        /// The error message for the property. The default is an empty string ("""").
        /// </returns>
        public string GetErrorMessage(string columnName1, string columnName2)
        {
            return ""some value"";
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_comment_with_exceptions_when_exception_comments_are_at_end()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        // Summary:
        //     Gets the error message for the property with the given name.
        //
        // Returns:
        //     The error message for the property. The default is an empty string ("""").
        //
        // Exceptions:
        //
        //   System.ArgumentNullException:
        //     The name parameter is null.
        //
        //   System.InvalidOperationException:
        //     The value of the specified resource is not a string.
        //
        public string GetErrorMessage(string columnName)
        {
            return ""some value"";
        }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <returns>
        /// The error message for the property. The default is an empty string ("""").
        /// </returns>
        /// <exception cref=""System.ArgumentNullException"">
        /// The name parameter is null.
        /// </exception>
        /// <exception cref=""System.InvalidOperationException"">
        /// The value of the specified resource is not a string.
        /// </exception>
        public string GetErrorMessage(string columnName)
        {
            return ""some value"";
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_comment_with_exceptions_when_exception_comments_are_at_end_and_on_single_lines()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        // Summary:
        //     Gets the error message for the property with the given name.
        //
        // Returns:
        //     The error message for the property. The default is an empty string ("""").
        //
        // Exceptions:
        //
        //   System.ArgumentNullException: The name parameter is null.
        //   System.InvalidOperationException: The value of the specified resource is not a string.
        //
        public string GetErrorMessage(string columnName)
        {
            return ""some value"";
        }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <returns>
        /// The error message for the property. The default is an empty string ("""").
        /// </returns>
        /// <exception cref=""System.ArgumentNullException"">
        /// The name parameter is null.
        /// </exception>
        /// <exception cref=""System.InvalidOperationException"">
        /// The value of the specified resource is not a string.
        /// </exception>
        public string GetErrorMessage(string columnName)
        {
            return ""some value"";
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_comment_with_exceptions_when_exception_comments_are_in_between()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        // Summary:
        //     Gets the error message for the property with the given name.
        //
        // Exceptions:
        //
        //   System.ArgumentNullException:
        //     The name parameter is null.
        //
        //   System.InvalidOperationException:
        //     The value of the specified resource is not a string.
        //
        // Returns:
        //     The error message for the property. The default is an empty string ("""").
        //
        public string GetErrorMessage(string columnName)
        {
            return ""some value"";
        }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <returns>
        /// The error message for the property. The default is an empty string ("""").
        /// </returns>
        /// <exception cref=""System.ArgumentNullException"">
        /// The name parameter is null.
        /// </exception>
        /// <exception cref=""System.InvalidOperationException"">
        /// The value of the specified resource is not a string.
        /// </exception>
        public string GetErrorMessage(string columnName)
        {
            return ""some value"";
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2313_CommentShouldBeDocumentationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2313_CommentShouldBeDocumentationAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2313_CodeFixProvider();
    }
}