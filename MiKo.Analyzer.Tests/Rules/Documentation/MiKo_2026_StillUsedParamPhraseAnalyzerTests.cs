using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2026_StillUsedParamPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] UnusedPhrases =
            {
                "Unused",
                "Unused.",
                "Not used",
                "Not used.",
                "Not in use",
                "Not in use.",
                "No use",
                "No use.",
                "No usage",
                "No usage.",
                "Ignore.",
                "Ignore",
                "Ignored.",
                "Ignored",
                "This parameter is not used.",
                "This parameter is not used",
                "This parameter is ignored.",
                "This parameter is ignored",
                "The parameter is not used.",
                "The parameter is not used",
                "The parameter is ignored.",
                "The parameter is ignored",
                "Parameter is not used.",
                "Parameter is not used",
                "Parameter is ignored.",
                "Parameter is ignored",

                "unused",
                "not used",
                "not in use",
                "no use",
                "no usage",
                "ignore",
                "ignored",
                "this parameter is not used",
                "this parameter is ignored",
                "the parameter is not used",
                "the parameter is ignored",
                "parameter is not used",
                "parameter is ignored",
            };

        [Test]
        public void No_issue_is_reported_for_method_that_has_no_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        public int DoSomething()
        {
            var j = 42;
            return j;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_that_has_no_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Ctor.</summary>
        public TestMe()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_has_correctly_documented_unused_parameter_([ValueSource(nameof(UnusedPhrases))] string comment) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""i"">" + comment + @"</param>
        public int DoSomething(int i)
        {
            var j = 42;
            return j;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_that_has_correctly_documented_unused_parameter_([ValueSource(nameof(UnusedPhrases))] string comment) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""i"">" + comment + @"</param>
        public TestMe(int i)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_has_correctly_documented_used_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""i"">The integer.</param>
        public int DoSomething(int i)
        {
            var j = 42 + i;
            return j;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_that_has_correctly_documented_used_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int m_i;

        /// <summary>Does something.</summary>
        /// <param name=""i"">The integer.</param>
        public TestMe(int i)
        {
            m_i = i;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_has_unused_variable_but_correctly_documented_used_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""i"">The integer.</param>
        public int DoSomething(int i)
        {
            var j = 42 + i;
            return i;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_that_has_unused_variable_but_correctly_documented_used_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int m_i;

        /// <summary>Does something.</summary>
        /// <param name=""i"">The integer.</param>
        public TestMe(int i)
        {
            var j = 42 + i;
            m_i = i;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_has_used_parameter_documented_as_unused_([ValueSource(nameof(UnusedPhrases))] string comment) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""i"">" + comment + @"</param>
        public int DoSomething(int i)
        {
            var j = 42 + i;
            return j;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_that_has_used_parameter_documented_as_unused_([ValueSource(nameof(UnusedPhrases))] string comment) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int m_i;

        /// <summary>Does something.</summary>
        /// <param name=""i"">" + comment + @"</param>
        public TestMe(int i)
        {
            m_i = i;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_yet_unfinished_method_that_has_parameter_documented_as_unused_([ValueSource(nameof(UnusedPhrases))] string comment) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""i"">" + comment + @"</param>
        public int DoSomething(int i)
    }
}
");

        [Test]
        public void No_issue_is_reported_for_yet_unfinished_ctor_that_has_parameter_documented_as_unused_([ValueSource(nameof(UnusedPhrases))] string comment) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int m_i;

        /// <summary>Does something.</summary>
        /// <param name=""i"">" + comment + @"</param>
        public TestMe(int i)
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2026_StillUsedParamPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2026_StillUsedParamPhraseAnalyzer();
    }
}