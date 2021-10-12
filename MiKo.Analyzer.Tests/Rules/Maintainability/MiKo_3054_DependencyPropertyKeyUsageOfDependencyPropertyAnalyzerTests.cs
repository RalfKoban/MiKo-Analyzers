using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3054_DependencyPropertyKeyUsageOfDependencyPropertyAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_DependencyPropertyKey_field() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private int m_field = 5;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_DependencyProperty_field_for_DependencyPropertyKey_field() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyPropertyKey m_fieldKey = DependencyProperty.RegisterReadOnly(nameof(MyField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty m_fieldProperty = m_fieldKey.DependencyProperty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DependencyPropertyKey_field_that_has_no_corresponding_DependencyProperty() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }

        public int OtherField { get; set; }
        
        private static readonly DependencyPropertyKey m_fieldKey = DependencyProperty.RegisterReadOnly(nameof(MyField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty m_otherProperty = DependencyProperty.Register(nameof(OtherField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void Code_gets_fixed_for_DependencyPropertyKey_field_that_has_no_corresponding_DependencyProperty_using_nameof()
        {
            const string OriginalCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }

        public int OtherField { get; set; }
        
        private static readonly DependencyPropertyKey m_fieldKey = DependencyProperty.RegisterReadOnly(nameof(MyField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty OtherFieldProperty = DependencyProperty.Register(nameof(OtherField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
";

            const string FixedCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }

        public int OtherField { get; set; }
        
        private static readonly DependencyPropertyKey m_fieldKey = DependencyProperty.RegisterReadOnly(nameof(MyField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty MyFieldProperty = m_fieldKey.DependencyProperty;
        public static readonly DependencyProperty OtherFieldProperty = DependencyProperty.Register(nameof(OtherField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_DependencyPropertyKey_field_that_has_no_corresponding_DependencyProperty_using_string()
        {
            const string OriginalCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }

        public int OtherField { get; set; }
        
        private static readonly DependencyPropertyKey m_fieldKey = DependencyProperty.RegisterReadOnly(""MyField"", typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty OtherFieldProperty = DependencyProperty.Register(nameof(OtherField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
";

            const string FixedCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }

        public int OtherField { get; set; }
        
        private static readonly DependencyPropertyKey m_fieldKey = DependencyProperty.RegisterReadOnly(""MyField"", typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty MyFieldProperty = m_fieldKey.DependencyProperty;
        public static readonly DependencyProperty OtherFieldProperty = DependencyProperty.Register(nameof(OtherField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3054_DependencyPropertyKeyUsageOfDependencyPropertyAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3054_DependencyPropertyKeyUsageOfDependencyPropertyAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3054_CodeFixProvider();
    }
}