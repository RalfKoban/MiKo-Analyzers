using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
                typeof(ArgumentNullException).Name,
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

        protected override string GetDiagnosticId() => MiKo_2051_ExceptionTagDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2051_ExceptionTagDefaultPhraseAnalyzer();

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
                                  "The exception in case ",
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