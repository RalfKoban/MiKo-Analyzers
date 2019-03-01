using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2026_StillUsedParamPhraseAnalyzerTests : CodeFixVerifier
    {
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
        public void An_issue_is_reported_for_method_that_has_used_parameter_documented_as_unused([Values("Unused", "Unused.")] string comment) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_ctor_that_has_used_parameter_documented_as_unused([Values("Unused", "Unused.")] string comment) => An_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_yet_unfinished_method_that_has_parameter_documented_as_unused([Values("Unused", "Unused.")] string comment) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_yet_unfinished_ctor_that_has_parameter_documented_as_unused([Values("Unused", "Unused.")] string comment) => No_issue_is_reported_for(@"
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