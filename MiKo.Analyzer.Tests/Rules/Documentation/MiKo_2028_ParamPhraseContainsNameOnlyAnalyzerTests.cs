using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2028_ParamPhraseContainsNameOnlyAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_that_has_no_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        public int DoSomething() => 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_correctly_commented_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""result"">An <see cref=""Int32""/> that contains the result to return.</param>
        public int DoSomething(int result) => result;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_incorrectly_commented_parameter_([Values(
                                                                                                 "A result.",
                                                                                                 "a result",
                                                                                                 "The result.",
                                                                                                 "the result",
                                                                                                 "An result.",
                                                                                                 "   a result   ",
                                                                                                 "Result.",
                                                                                                 "result",
                                                                                                 "  result  ")]
                                                                                         string documentation)
            => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""result"">" + documentation + @"</param>
        public int DoSomething(int result) => result;
    }
}
");

        /// <inheritdoc />
        protected override string GetDiagnosticId() => MiKo_2028_ParamPhraseContainsNameOnlyAnalyzer.Id;

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2028_ParamPhraseContainsNameOnlyAnalyzer();
    }
}