using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3097_DoNotCastAndReturnUnnecessaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_without_cast() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething()
        {
            return new object();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_cast_assignment_to_variable() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object o)
        {
            var testee = (TestMe)o;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_cast_assignment_to_variable_and_returning_that_typed() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TestMe DoSomething(object o)
        {
            var testee = (TestMe)o;

            return testee;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_cast_assignment_to_variable_and_returning_that_as_object() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(object o)
        {
            var testee = (TestMe)o;

            return testee;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_cast_and_returning_that_as_object() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(object o)
        {
            return (TestMe)o;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_expression_body_with_cast_and_returning_that_as_object() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(object o) => (TestMe)o;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3097_DoNotCastAndReturnUnnecessaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3097_DoNotCastAndReturnUnnecessaryAnalyzer();
    }
}