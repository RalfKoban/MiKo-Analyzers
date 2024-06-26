using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2051_ExceptionTagDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ExceptionTypes =
                                                          [
                                                              nameof(ArgumentException),
                                                              nameof(InvalidOperationException),
                                                              nameof(NotSupportedException),
                                                              nameof(Exception),
                                                          ];

        private static readonly string[] ForbiddenExceptionStartingPhrases = CreateForbiddenExceptionStartingPhrases();

        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method_without_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_throwing_an_([ValueSource(nameof(ExceptionTypes))] string exceptionType) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""" + exceptionType + @""">
    /// <paramref name=""o""/> is something.
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_method_throwing_an_(
                                                                                    [ValueSource(nameof(ExceptionTypes))] string exceptionType,
                                                                                    [ValueSource(nameof(ForbiddenExceptionStartingPhrases))] string startingPhrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""" + exceptionType + @""">
    /// " + startingPhrase + @" something.
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_with_para_tag_throwing_an_ObjectDisposedException() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">
    /// <para>
    /// If the object is disposed.
    /// </para>
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [TestCase(nameof(Exception), "If it's ", "It's ")]
        [TestCase(nameof(Exception), @"Gets thrown when <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"gets thrown when <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"in case <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"In case <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"In case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Is thrown when <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Is thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Thrown if <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), "Thrown if any error ", "Any error ")]
        [TestCase(nameof(Exception), @"Thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Thrown when <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Throws if <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Throws if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Throws when <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Throws when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"thrown if <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), "thrown if any error ", "Any error ")]
        [TestCase(nameof(Exception), @"thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"thrown when <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"throws if <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"throws if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"throws when <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception gets thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception gets thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception is thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception is thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception is thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception that gets thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception that gets thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception that gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception that is thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception that is thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception that is thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception that will be thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception that will be thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception that will be thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception which gets thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception which gets thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception which gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception which is thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception which is thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception which is thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception which will be thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception which will be thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception which will be thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception will be thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception will be thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"The exception will be thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"This exception gets thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"This exception gets thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"This exception gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"This exception is thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"This exception is thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"This exception is thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"This exception will be thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"This exception will be thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"This exception will be thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception that gets thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception that gets thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception that gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception that is thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception that is thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception that is thrown in case that the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception that is thrown in case which the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception that is thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception that will be thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception that will be thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception that will be thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception which gets thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception which gets thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception which gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception which is thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception which is thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception which is thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception which will be thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception which will be thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"A exception which will be thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception that gets thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception that gets thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception that gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception that is thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception that is thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception that is thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception that will be thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception that will be thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception that will be thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception which gets thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception which gets thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception which gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception which is thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception which is thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception which is thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception which will be thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception which will be thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"An exception which will be thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception that gets thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception that gets thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception that gets thrown in case that the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception that gets thrown in case which the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception that gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception that is thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception that is thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception that is thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception that will be thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception that will be thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception that will be thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception which gets thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception which gets thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception which gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception which is thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception which is thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception which is thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception which will be thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception which will be thrown in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception which will be thrown in case that the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception which will be thrown in case which the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(Exception), @"Exception which will be thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase(nameof(InvalidOperationException), "Throw in case that the file cannot be converted to the ", "The file cannot be converted to the ")]
        [TestCase(nameof(InvalidOperationException), "Throw in case that a module cannot be loaded into ", "A module cannot be loaded into ")]
        [TestCase(nameof(InvalidOperationException), "Thrown in case that the file cannot be converted to the ", "The file cannot be converted to the ")]
        [TestCase(nameof(InvalidOperationException), "Thrown in case that a module cannot be loaded into ", "A module cannot be loaded into ")]
        public void Code_gets_fixed_for_(string exceptionType, string startingPhrase, string fixedPhrase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""" + exceptionType + @""">
    /// " + startingPhrase + @"something.
    /// </exception>
    public void DoSomething(object o) { }
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""" + exceptionType + @""">
    /// " + fixedPhrase + @"something.
    /// </exception>
    public void DoSomething(object o) { }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("If the ")]
        [TestCase("Gets thrown when the")]
        [TestCase("In case the")]
        [TestCase("Is thrown when the")]
        [TestCase("Thrown if the")]
        [TestCase("Thrown when the")]
        [TestCase("Throws if the")]
        [TestCase("Throws when the")]
        public void Code_gets_fixed_for_ObjectDisposedException_(string startingPhrase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">
    /// " + startingPhrase + @"something.
    /// </exception>
    public void DoSomething(object o) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">
    /// The current instance has been disposed.
    /// </exception>
    public void DoSomething(object o) { }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ObjectDisposedException_with_para_tag()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">
    /// <para>
    /// If the object is disposed.
    /// </para>
    /// </exception>
    public void DoSomething(object o) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">
    /// The current instance has been disposed.
    /// </exception>
    public void DoSomething(object o) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase(nameof(ArgumentNullException), "If null")]
        [TestCase(nameof(ArgumentNullException), @"If <paramref name=""o""/> is null")]
        [TestCase(nameof(ArgumentNullException), @"If <paramref name=""o""/> is <see langword=""null""/>")]
        [TestCase(nameof(ArgumentNullException), @"If the <paramref name=""o""/> is <see langword=""null""/>.")]
        [TestCase("System." + nameof(ArgumentNullException), "If null")]
        [TestCase("System." + nameof(ArgumentNullException), @"If <paramref name=""o""/> is null")]
        [TestCase("System." + nameof(ArgumentNullException), @"If <paramref name=""o""/> is <see langword=""null""/>")]
        [TestCase("System." + nameof(ArgumentNullException), @"If the <paramref name=""o""/> is <see langword=""null""/>.")]
        public void Code_gets_fixed_for_ArgumentNullException_and_single_parameter_(string exceptionType, string text)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""" + exceptionType + @""">
    /// " + text + @"
    /// </exception>
    public void DoSomething(object o) { }
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""" + exceptionType + @""">
    /// <paramref name=""o""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o) { }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_and_2_referenced_parameters()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// If <paramref name=""o1""/> or <paramref name=""o2""/> are null.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o1""/> is <see langword=""null""/>.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_and_2_parameters_but_only_1_referenced_one()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// If <paramref name=""o1""/> is no item
    /// </exception>
    /// <exception cref=""ArgumentNullException"">
    /// If <paramref name=""o2""/> is null
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    /// <paramref name=""o1""/> is no item
    /// </exception>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o2""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_and_3_parameters_but_only_2_referenced_ones_variant_1()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// If <paramref name=""o1""/> or <paramref name=""o2""/> are null.
    /// </exception>
    public void DoSomething(object o1, object o2, object o3) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o1""/> is <see langword=""null""/>.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o1, object o2, object o3) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentNullException_and_3_parameters_but_only_2_referenced_ones_variant_2()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    ///     If o1 or o2 are null.
    /// </exception>
    public void DoSomething(object o1, object o2, object o3) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentNullException"">
    /// <paramref name=""o1""/> is <see langword=""null""/>.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is <see langword=""null""/>.
    /// </exception>
    public void DoSomething(object o1, object o2, object o3) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_and_method_with_1_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// Thrown if i is negative.
    /// </exception>
    public void DoSomething(int i) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// <paramref name=""i""/> is negative.
    /// </exception>
    public void DoSomething(int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_and_method_with_3_parameters_but_only_1_mentioned()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// If k is negative.
    /// </exception>
    public void DoSomething(int i, int j, int k) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// <paramref name=""k""/> is negative.
    /// </exception>
    public void DoSomething(int i, int j, int k) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_and_method_with_3_parameters_but_only_2_mentioned()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// Thrown if i or k is negative.
    /// </exception>
    public void DoSomething(int i, int j, int k) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// <paramref name=""i""/> or <paramref name=""k""/> is negative.
    /// </exception>
    public void DoSomething(int i, int j, int k) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentOutOfRangeException_and_method_with_3_parameters_and_all_3_mentioned()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// Thrown if i, j or k is negative.
    /// </exception>
    public void DoSomething(int i, int j, int k) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// <paramref name=""i""/>, <paramref name=""j""/> or <paramref name=""k""/> is negative.
    /// </exception>
    public void DoSomething(int i, int j, int k) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ArgumentException_on_property_indexer()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    ///     If key is -1.
    /// </exception>
    public object this[int key] { get; set; }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentException"">
    ///     <paramref name=""key""/> is -1.
    /// </exception>
    public object this[int key] { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2051_ExceptionTagDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2051_ExceptionTagDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2051_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static string[] CreateForbiddenExceptionStartingPhrases()
        {
            string[] phrases =
                               [
                                   "A exception that gets thrown ",
                                   "A exception that is thrown ",
                                   "A exception that should be be thrown ",
                                   "A exception that will be thrown ",
                                   "A exception thrown ",
                                   "A exception which gets thrown ",
                                   "A exception which is thrown ",
                                   "A exception which should be be thrown ",
                                   "A exception which will be thrown ",
                                   "An exception that gets thrown ",
                                   "An exception that is thrown ",
                                   "An exception that should be be thrown ",
                                   "An exception that will be thrown ",
                                   "An exception thrown ",
                                   "An exception which gets thrown ",
                                   "An exception which is thrown ",
                                   "An exception which should be be thrown ",
                                   "An exception which will be thrown ",
                                   "Can be thrown ",
                                   "Exception in case ",
                                   "Exception that gets thrown ",
                                   "Exception that is thrown ",
                                   "Exception that should be be thrown ",
                                   "Exception that will be thrown ",
                                   "Exception which gets thrown ",
                                   "Exception which is thrown ",
                                   "Exception which should be be thrown ",
                                   "Exception which will be thrown ",
                                   "Fired ",
                                   "Gets thrown ",
                                   "If ",
                                   "In case ",
                                   "Is fired ",
                                   "Is thrown ",
                                   "Should be thrown ",
                                   "The exception gets thrown ",
                                   "The exception in case ",
                                   "The exception is thrown ",
                                   "The exception should be be thrown ",
                                   "The exception that gets thrown ",
                                   "The exception that is thrown ",
                                   "The exception that should be be thrown ",
                                   "The exception that will be thrown ",
                                   "The exception which gets thrown ",
                                   "The exception which is thrown ",
                                   "The exception which should be be thrown ",
                                   "The exception which will be thrown ",
                                   "The exception will be thrown ",
                                   "This exception gets thrown ",
                                   "This exception is thrown ",
                                   "This exception should be thrown ",
                                   "This exception will be thrown ",
                                   "Throw ",
                                   "Thrown ",
                                   "Throws ",
                                   "When ",
                                   "Will be thrown ",
                               ];

            return [.. phrases.Union(phrases.Select(_ => _.ToLower(CultureInfo.CurrentCulture))).OrderBy(_ => _)];
        }
    }
}