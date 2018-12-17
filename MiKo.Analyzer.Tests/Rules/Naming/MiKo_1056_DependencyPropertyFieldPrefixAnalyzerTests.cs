using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1056_DependencyPropertyFieldPrefixAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_unassigned_correctly_named_DependencyProperty_field() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }

        private static readonly DependencyProperty MyFieldProperty;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_DependencyProperty_field_with_nameof() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }

        private static readonly DependencyProperty MyFieldProperty = DependencyProperty.Register(nameof(MyField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_DependencyProperty_field_with_string() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }

        private static readonly DependencyProperty MyFieldProperty = DependencyProperty.Register(""MyField"", typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_DependencyProperty_field_for_DependencyPropertyKey() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public static readonly DependencyProperty OtherNameProperty = MyFieldKey.DependencyProperty;

        private DependencyPropertyKey MyFieldKey;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_attached_DependencyProperty_field() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public static readonly DependencyProperty OtherNameProperty = DependencyProperty.RegisterAttached(
                                                                                                    ""Bla"",
                                                                                                    typeof(int),
                                                                                                    typeof(TestMe),
                                                                                                    new PropertyMetadata(default(int)));
    }
}
");


        [Test]
        public void No_issue_is_reported_for_a_strangely_formatted_attached_DependencyProperty_field() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public static readonly DependencyProperty OtherNameProperty = DependencyProperty
                                                                                    .RegisterAttached(
                                                                                                    ""Bla"",
                                                                                                    typeof(int),
                                                                                                    typeof(TestMe),
                                                                                                    new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_DependencyProperty_field([Values("m_field", "m_fieldKey", "m_fieldProperty", "Field", "FieldKey", "FieldProperty")] string fieldName)
            => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private int MyProperty { get; set; }

        private DependencyProperty " + fieldName + @";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_DependencyProperty_field_if_class_has_no_properties(
                                                                                           [Values("m_field", "m_fieldKey", "m_fieldProperty", "Field", "FieldKey", "FieldProperty")] string fieldName)
            => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private DependencyProperty " + fieldName + @";
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_DependencyProperty_field_with_nameof() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        public int MyField2 { get; set; }

        private static readonly DependencyProperty MyFieldProperty = DependencyProperty.Register(nameof(MyField2), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_DependencyProperty_field_with_string() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        public int MyField2 { get; set; }

        private static readonly DependencyProperty MyFieldProperty = DependencyProperty.Register(""MyField2"", typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1056_DependencyPropertyFieldPrefixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1056_DependencyPropertyFieldPrefixAnalyzer();
    }
}