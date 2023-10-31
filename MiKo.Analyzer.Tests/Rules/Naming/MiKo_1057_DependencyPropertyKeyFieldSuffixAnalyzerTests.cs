using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1057_DependencyPropertyKeyFieldSuffixAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_correctly_named_DependencyPropertyKey_field() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private DependencyPropertyKey m_fieldKey;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_DependencyPropertyKey_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private DependencyPropertyKey m_field;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1057_DependencyPropertyKeyFieldSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1057_DependencyPropertyKeyFieldSuffixAnalyzer();
    }
}