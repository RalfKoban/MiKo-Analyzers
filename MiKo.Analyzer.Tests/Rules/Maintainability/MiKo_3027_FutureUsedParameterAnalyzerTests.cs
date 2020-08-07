using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3027_FutureUsedParameterAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ReservedForFuturePhrases =
            {
                "Reserved.",
                "For future use.",
                "Reserved for future.",
                "will be used in future",
                "it's reserved",
            };

        [Test]
        public void No_issue_is_reported_for_method_that_has_no_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            var j = 42;
            return j;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_has_no_reserved_parameter_([Values("ref", "out", "")] string modifier) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(" + modifier + @" int i)
        {
            return i;
        }
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_method_that_has_a_reserved_parameter_(
                                                                                [Values("ref", "out", "")] string modifier,
                                                                                [ValueSource(nameof(ReservedForFuturePhrases))] string phrase)
            => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>
        /// Does something.
        /// </summary>
        /// <param name=""i"">" + phrase + @"</param>
        public int DoSomething(" + modifier + @" int i)
        {
            return i;
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3027_FutureUsedParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3027_FutureUsedParameterAnalyzer();
    }
}