using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public class MiKo_3076_StaticMemberInitializerRefersToStaticMemberBelowOrInOtherPartAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_enum() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public enum TestMe
    {
        None,
        Something,
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_static_field() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int m_field;

        private static readonly int s_field;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_static_field_that_is_initialized_with_value() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private static readonly int A = -1;

        private static readonly int B = 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_static_field_that_is_initialized_with_static_field_value_from_above() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private static readonly int A = 42;

        private static readonly int B = A;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_static_field_that_is_initialized_with_static_field_value_from_below() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private static readonly int A = B;

        private static readonly int B = 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_static_field_array_that_is_initialized_with_static_field_value_from_above() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private static readonly int A = 42;

        private static readonly int[] B =
            {
                A,
            };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_static_field_array_that_is_initialized_with_static_field_value_from_below() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private static readonly int[] A =
            {
                B,
            };

        private static readonly int B = 42;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3076_StaticMemberInitializerRefersToStaticMemberBelowOrInOtherPartAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3076_StaticMemberInitializerRefersToStaticMemberBelowOrInOtherPartAnalyzer();
    }
}