using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3052_DependencyPropertyKeyNonPublicStaticReadOnlyFieldAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_DependencyPropertyKey_field() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private int m_field;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_public_static_readonly_DependencyPropertyKey_field_([Values("protected", "internal", "protected internal", "private", "")] string visibility)
            => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        " + visibility + @" static readonly DependencyPropertyKey m_fieldProperty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_public_static_readonly_DependencyPropertyKey_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public static readonly DependencyPropertyKey m_fieldProperty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_static_only_DependencyPropertyKey_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        internal static DependencyPropertyKey m_fieldProperty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_readonly_only_DependencyPropertyKey_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        internal readonly DependencyPropertyKey m_fieldProperty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_static_non_readonly_DependencyPropertyKey_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        internal DependencyPropertyKey m_fieldProperty;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3052_DependencyPropertyKeyNonPublicStaticReadOnlyFieldAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3052_DependencyPropertyKeyNonPublicStaticReadOnlyFieldAnalyzer();
    }
}