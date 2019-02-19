using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3000_EmptyRegionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void An_issue_is_reported_for_empty_region_with_comment() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        #region Empty directive

        // some comment

        #endregion
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_region_without_comment() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        #region Empty directive

        #endregion
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_empty_region() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        #region Empty directive

        #region Nested

        #endregion

        #endregion
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_empty_region_with_field() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        #region Fields

        private const int Number = 42;

        #endregion
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3000_EmptyRegionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3000_EmptyRegionAnalyzer();
    }
}