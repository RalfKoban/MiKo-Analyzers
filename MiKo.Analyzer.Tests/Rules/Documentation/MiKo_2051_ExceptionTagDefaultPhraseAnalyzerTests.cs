using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2051_ExceptionTagDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ExceptionTypes =
            {
                typeof(ArgumentException).Name,
                typeof(InvalidOperationException).Name,
                typeof(NotSupportedException).Name,
                typeof(Exception).Name,
            };

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

        [TestCase("Exception", "if the ", "")]
        [TestCase("Exception", "If the ", "")]
        [TestCase("Exception", @"Gets thrown when <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"Gets thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"in case <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"In case <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"in case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"In case the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"Is thrown when <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"Is thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"Thrown if <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"Thrown if any error ", @"any error ")]
        [TestCase("Exception", @"Thrown if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"Thrown when <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"Thrown when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"Throws if <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"Throws if the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"Throws when <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
        [TestCase("Exception", @"Throws when the <paramref name=""o""/> ", @"<paramref name=""o""/> ")]
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

        protected override string GetDiagnosticId() => MiKo_2051_ExceptionTagDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2051_ExceptionTagDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2051_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static string[] CreateForbiddenExceptionStartingPhrases()
        {
            var phrases = new[]
                              {
                                  "If ",
                                  "In case ",
                                  "When ",
                                  "Throw ",
                                  "Throws ",
                                  "Thrown ",
                                  "Is thrown ",
                                  "Gets thrown ",
                                  "Will be thrown ",
                                  "Can be thrown ",
                                  "Should be thrown ",
                                  "The exception in case ",
                                  "This exception should be thrown ",
                                  "Exception in case ",
                                  "A exception thrown ",
                                  "An exception thrown ",
                                  "Fired ",
                                  "Is fired ",
                              };

            return phrases.Concat(phrases.Select(_ => _.ToLower())).Distinct().OrderBy(_ => _).ToArray();
        }
    }
}