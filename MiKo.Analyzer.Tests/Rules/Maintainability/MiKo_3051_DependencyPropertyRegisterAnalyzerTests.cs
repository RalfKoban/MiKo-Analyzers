﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3051_DependencyPropertyRegisterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_DependencyProperty_field() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_DependencyProperty_field_that_is_not_registered() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_DependencyProperty_field_that_is_registered_with_nameof() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(nameof(MyField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        [TestCase("ObservableCollection<Control>")]
        [TestCase("ObservableCollection<System.Windows.Controls.Control>")]
        [TestCase("System.Collections.ObjectModel.ObservableCollection<Control>")]
        [TestCase("System.Collections.ObjectModel.ObservableCollection<System.Windows.Controls.Control>")]
        public void No_issue_is_reported_for_DependencyProperty_field_that_is_registered_with_correct_collection_type_(string propertyType) => No_issue_is_reported_for(@"

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Bla
{
    public class TestMe
    {
        public TestMe()
        {
            AdditionalItems = new " + propertyType + @"();
        }

        public " + propertyType + @" AdditionalItems
        {
            get { return (" + propertyType + @")GetValue(AdditionalItemsProperty); }
            set { SetValue(AdditionalItemsProperty, value); }
        }

        public static readonly DependencyProperty AdditionalItemsProperty = DependencyProperty.Register(nameof(AdditionalItems), typeof(" + propertyType + @"), typeof(TestMe), new PropertyMetadata());
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DependencyProperty_field_that_is_registered_with_StringLiteral() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(""MyField"", typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DependencyProperty_field_that_is_registered_with_wrong_property_name() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
    }

    public class TestMe2 : TestMe
    {
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(nameof(MyField), typeof(int), typeof(TestMe2), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DependencyProperty_field_that_is_registered_with_wrong_property_type() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(nameof(MyField), typeof(string), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DependencyProperty_field_that_is_registered_with_wrong_owning_type() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(nameof(MyField), typeof(int), typeof(int), new PropertyMetadata(default(int)));
    }
}
");

        [Test]
        public void Code_gets_fixed_for_DependencyProperty_field_that_is_registered_with_StringLiteral()
        {
            const string OriginalCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(""MyField"", typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
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
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(nameof(MyField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_DependencyProperty_field_that_is_registered_with_wrong_property_type()
        {
            const string OriginalCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(nameof(MyField), typeof(string), typeof(TestMe), new PropertyMetadata(default(int)));
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
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(nameof(MyField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_DependencyProperty_field_that_is_registered_with_wrong_owning_type()
        {
            const string OriginalCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(nameof(MyField), typeof(int), typeof(int), new PropertyMetadata(default(int)));
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
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(nameof(MyField), typeof(int), typeof(TestMe), new PropertyMetadata(default(int)));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_DependencyProperty_field_that_is_registered_with_StringLiteral_spanning_multiple_lines()
        {
            const string OriginalCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(
                                                                                            ""MyField"",
                                                                                            typeof(int),
                                                                                            typeof(TestMe),
                                                                                            new PropertyMetadata(default(int)));
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
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(
                                                                                            nameof(MyField),
                                                                                            typeof(int),
                                                                                            typeof(TestMe),
                                                                                            new PropertyMetadata(default(int)));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_DependencyProperty_field_that_is_registered_with_wrong_owning_type_spanning_multiple_lines()
        {
            const string OriginalCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public int MyField { get; set; }
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(
                                                                                            nameof(MyField),
                                                                                            typeof(int),
                                                                                            typeof(int),
                                                                                            new PropertyMetadata(default(int)));
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
        
        private static readonly DependencyProperty m_fieldProperty = DependencyProperty.Register(
                                                                                            nameof(MyField),
                                                                                            typeof(int),
                                                                                            typeof(TestMe),
                                                                                            new PropertyMetadata(default(int)));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3051_DependencyPropertyRegisterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3051_DependencyPropertyRegisterAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3051_CodeFixProvider();
    }
}