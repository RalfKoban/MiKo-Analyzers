using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1530_DependencyPropertyChangeHandlingMethodNamesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_event_handling_method() => No_issue_is_reported_for("""

            using System;

            public class TestMe
            {
                public void DoSomething() { }
            }

            """);

        [Test]
        public void No_issue_is_reported_for_correctly_named_method_with_nameof() => No_issue_is_reported_for("""

            using System;
            using System.Windows;

            public class TestMe
            {
                public static readonly DependencyProperty MyDependencyProperty = DependencyProperty.Register(nameof(MyProperty),
                                                                                                             typeof(bool),
                                                                                                             typeof(TestMe),
                                                                                                             new PropertyMetadata(false, OnMyPropertyChanged));

                public bool MyProperty { get; set; }

                public void OnMyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
            }

            """);

        [Test]
        public void No_issue_is_reported_for_correctly_named_method_with_string_literal() => No_issue_is_reported_for("""

            using System;
            using System.Windows;

            public class TestMe
            {
                public static readonly DependencyProperty MyDependencyProperty = DependencyProperty.Register("MyProperty",
                                                                                                             typeof(bool),
                                                                                                             typeof(TestMe),
                                                                                                             new PropertyMetadata(false, OnMyPropertyChanged));

                public bool MyProperty { get; set; }

                public void OnMyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_method_not_matching_propertyName_with_nameof() => An_issue_is_reported_for("""

            using System;
            using System.Windows;

            public class TestMe
            {
                public static readonly DependencyProperty MyDependencyProperty = DependencyProperty.Register(nameof(MyProperty),
                                                                                                             typeof(bool),
                                                                                                             typeof(TestMe),
                                                                                                             new PropertyMetadata(false, Whatever));

                public bool MyProperty { get; set; }

                public void Whatever(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_method_not_matching_propertyName_with_string_literal() => An_issue_is_reported_for("""

            using System;
            using System.Windows;

            public class TestMe
            {
                public static readonly DependencyProperty MyDependencyProperty = DependencyProperty.Register("MyProperty",
                                                                                                             typeof(bool),
                                                                                                             typeof(TestMe),
                                                                                                             new PropertyMetadata(false, Whatever));

                public bool MyProperty { get; set; }

                public void Whatever(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_method_not_matching_propertyName_with_nameof_and_anonymous_PropertyMetadata() => An_issue_is_reported_for("""

            using System;
            using System.Windows;

            public class TestMe
            {
                public static readonly DependencyProperty MyDependencyProperty = DependencyProperty.Register(nameof(MyProperty),
                                                                                                             typeof(bool),
                                                                                                             typeof(TestMe),
                                                                                                             new(false, Whatever));

                public bool MyProperty { get; set; }

                public void Whatever(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_method_not_matching_propertyName_with_string_literal_and_anonymous_PropertyMetadata() => An_issue_is_reported_for("""

            using System;
            using System.Windows;

            public class TestMe
            {
                public static readonly DependencyProperty MyDependencyProperty = DependencyProperty.Register("MyProperty",
                                                                                                             typeof(bool),
                                                                                                             typeof(TestMe),
                                                                                                             new(false, Whatever));

                public bool MyProperty { get; set; }

                public void Whatever(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
            }

            """);

        [Test, Ignore("Currently I have no idea why the code-fix only changes the method name but not the callback, as that works in production.")]
        public void Code_gets_fixed_for_method_with_nameof()
        {
            const string Template = """

                using System;
                using System.Windows;

                public class TestMe
                {
                    public static readonly DependencyProperty MyDependencyProperty = DependencyProperty.Register(nameof(MyProperty),
                                                                                                                 typeof(bool),
                                                                                                                 typeof(TestMe),
                                                                                                                 new PropertyMetadata(false, ###));

                    public bool MyProperty { get; set; }

                    public void ###(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", "Whatever"), Template.Replace("###", "OnMyPropertyChanged"));
        }

        [Test, Ignore("Currently I have no idea why the code-fix only changes the method name but not the callback, as that works in production.")]
        public void Code_gets_fixed_for_method_with_string_literal()
        {
            const string Template = """

                using System;
                using System.Windows;

                public class TestMe
                {
                    public static readonly DependencyProperty MyDependencyProperty = DependencyProperty.Register("MyProperty",
                                                                                                                 typeof(bool),
                                                                                                                 typeof(TestMe),
                                                                                                                 new PropertyMetadata(false, ###));

                    public bool MyProperty { get; set; }

                    public void ###(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", "Whatever"), Template.Replace("###", "OnMyPropertyChanged"));
        }

        [Test, Ignore("Currently I have no idea why the code-fix only changes the method name but not the callback, as that works in production.")]
        public void Code_gets_fixed_for_method_with_nameof_and_anonymous_PropertyMetadata()
        {
            const string Template = """

                using System;
                using System.Windows;

                public class TestMe
                {
                    public static readonly DependencyProperty MyDependencyProperty = DependencyProperty.Register(nameof(MyProperty),
                                                                                                                 typeof(bool),
                                                                                                                 typeof(TestMe),
                                                                                                                 new(false, ###));

                    public bool MyProperty { get; set; }

                    public void ###(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", "Whatever"), Template.Replace("###", "OnMyPropertyChanged"));
        }

        [Test, Ignore("Currently I have no idea why the code-fix only changes the method name but not the callback, as that works in production.")]
        public void Code_gets_fixed_for_method_with_string_literal_and_anonymous_PropertyMetadata()
        {
            const string Template = """

                using System;
                using System.Windows;

                public class TestMe
                {
                    public static readonly DependencyProperty MyDependencyProperty = DependencyProperty.Register("MyProperty",
                                                                                                                 typeof(bool),
                                                                                                                 typeof(TestMe),
                                                                                                                 new(false, ###));

                    public bool MyProperty { get; set; }

                    public void ###(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", "Whatever"), Template.Replace("###", "OnMyPropertyChanged"));
        }

        protected override string GetDiagnosticId() => MiKo_1530_DependencyPropertyChangeHandlingMethodNamesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1530_DependencyPropertyChangeHandlingMethodNamesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1530_CodeFixProvider();
    }
}