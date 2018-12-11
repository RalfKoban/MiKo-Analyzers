using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3053_DependencyPropertyKeyRegisterAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_DependencyPropertyKey_field_that_is_not_registered() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private DependencyPropertyKey m_fieldProperty;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_DependencyPropertyKey_field_that_is_registered_with_nameof() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyPropertyKey m_fieldKey = DependencyProperty.RegisterReadOnly(nameof(MyField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DependencyPropertyKey_field_that_is_registered_with_StringLiteral() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyPropertyKey m_fieldKey = DependencyProperty.RegisterReadOnly(""MyField"", typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DependencyPropertyKey_field_that_is_registered_with_wrong_property_name() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
    }

    public class TestMe2 : TestMe
    {
        private static readonly DependencyPropertyKey m_fieldKey = DependencyProperty.RegisterReadOnly(nameof(MyField), typeof(int), typeof(TestMe2), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DependencyPropertyKey_field_that_is_registered_with_wrong_property_type() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyPropertyKey m_fieldKey = DependencyProperty.RegisterReadOnly(nameof(MyField), typeof(string), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DependencyPropertyKey_field_that_is_registered_with_wrong_owning_type() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyPropertyKey m_fieldKey = DependencyProperty.RegisterReadOnly(nameof(MyField), typeof(int), typeof(int), new PropertyMetadata(default(int)));
    }
}
");


        protected override string GetDiagnosticId() => MiKo_3053_DependencyPropertyKeyRegisterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3053_DependencyPropertyKeyRegisterAnalyzer();
    }
}