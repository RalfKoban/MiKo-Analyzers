using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1055_DependencyPropertyFieldSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_DependencyProperty_field() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_named_DependencyProperty_field() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private DependencyProperty m_fieldProperty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_DependencyProperty_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private DependencyProperty m_field;
    }
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string Template = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private int Xyz { get; set; }

        private DependencyProperty ###;
    }
}
";

            VerifyCSharpFix(Template.Replace("###", "m_field"), Template.Replace("###", "XyzProperty"));
        }

        protected override string GetDiagnosticId() => MiKo_1055_DependencyPropertyFieldSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1055_DependencyPropertyFieldSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1055_1056_CodeFixProvider();
    }
}