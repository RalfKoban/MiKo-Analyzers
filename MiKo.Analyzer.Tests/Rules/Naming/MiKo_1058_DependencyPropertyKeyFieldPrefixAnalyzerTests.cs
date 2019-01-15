using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1058_DependencyPropertyKeyFieldPrefixAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_correctly_named_unassigned_DependencyPropertyKey_field() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }

        private DependencyPropertyKey MyFieldKey;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_DependencyPropertyKey_field_with_nameof() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }

        private static readonly DependencyPropertyKey MyFieldKey = DependencyProperty.RegisterReadOnly(nameof(MyField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_DependencyPropertyKey_field_with_string() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }

        private static readonly DependencyPropertyKey MyFieldKey = DependencyProperty.RegisterReadOnly(""MyField"", typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
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
        public static readonly DependencyPropertyKey OtherNameKey = DependencyProperty.RegisterAttachedReadOnly(
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
        public static readonly DependencyPropertyKey OtherNameKey = DependencyProperty
                                                                                    .RegisterAttachedReadOnly(
                                                                                                    ""Bla"",
                                                                                                    typeof(int),
                                                                                                    typeof(TestMe),
                                                                                                    new PropertyMetadata(default(int)));
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
        public int MyField { get; set; }

        private DependencyPropertyKey MyFieldPropertyKey;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_DependencyPropertyKey_field_([Values("m_field", "m_fieldKey", "m_fieldProperty", "Field", "FieldKey", "FieldProperty")] string fieldName)
            => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private int MyProperty { get; set; }

        private DependencyPropertyKey " + fieldName + @";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_DependencyPropertyKey_field_if_class_has_no_properties([Values("m_field", "m_fieldKey", "m_fieldProperty", "Field", "FieldKey", "FieldProperty")] string fieldName)
            => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private DependencyPropertyKey " + fieldName + @";
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_DependencyPropertyKey_field_with_nameof() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        public int MyField2 { get; set; }

        private static readonly DependencyPropertyKey MyFieldKey = DependencyProperty.RegisterReadOnly(nameof(MyField2), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_DependencyPropertyKey_field_with_string() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        public int MyField2 { get; set; }

        private static readonly DependencyPropertyKey MyFieldKey = DependencyProperty.RegisterReadOnly(""MyField2"", typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1058_DependencyPropertyKeyFieldPrefixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1058_DependencyPropertyKeyFieldPrefixAnalyzer();
    }
}