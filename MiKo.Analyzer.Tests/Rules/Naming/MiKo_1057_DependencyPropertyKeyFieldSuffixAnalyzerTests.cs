using Microsoft.CodeAnalysis.CodeFixes;
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

        [Test]
        public void Code_gets_fixed_([Values("m_field", "m_fieldProperty", "Field", "FieldProperty", "XyzDependencyProperty")] string fieldName)
        {
            const string Template = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private int Xyz { get; set; }

        private DependencyPropertyKey ###;
    }
}
";

            VerifyCSharpFix(Template.Replace("###", fieldName), Template.Replace("###", "XyzKey"));
        }

        protected override string GetDiagnosticId() => MiKo_1057_DependencyPropertyKeyFieldSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1057_DependencyPropertyKeyFieldSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1057_1058_CodeFixProvider();
    }
}