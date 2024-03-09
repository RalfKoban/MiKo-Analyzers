using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1001_EventArgsParameterAnalyzerTests : CodeFixVerifier
    {
        [TestCase("object s, EventArgs args")]
        [TestCase("DependencyObject d, DependencyPropertyChangedEventArgs args")]
        public void No_issue_is_reported_for_parameters_on_event_handling_method_(string parameters) => No_issue_is_reported_for(@"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void DoSomething(" + parameters + @") { }
    }
}
");

        [TestCase("")]
        [TestCase("int args")]
        [TestCase("object s")]
        [TestCase("object s, int args, object whatever")]
        [TestCase("object whatever, object s, int args")]
        public void No_issue_is_reported_for_non_matching_parameters_on_method_(string parameters) => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void DoSomething(" + parameters + @") { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [TestCase("EventArgs args", 1)]
        [TestCase("EventArgs args, object s", 1)]
        [TestCase("EventArgs e, EventArgs a", 2)]
        [TestCase("object s, EventArgs args, object whatever", 1)]
        [TestCase("object whatever, object s, EventArgs args", 1)]
        [TestCase("DependencyPropertyChangedEventArgs args", 1)]
        [TestCase("DependencyPropertyChangedEventArgs args, object s", 1)]
        [TestCase("DependencyPropertyChangedEventArgs e, DependencyPropertyChangedEventArgs a", 2)]
        [TestCase("object s, DependencyPropertyChangedEventArgs args, object whatever", 1)]
        [TestCase("object whatever, object s, DependencyPropertyChangedEventArgs args", 1)]
        public void An_issue_is_reported_for_matching_parameters_on_method_(string parameters, int violations) => An_issue_is_reported_for(violations, @"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void DoSomething(" + parameters + @") { }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [TestCase("EventArgs args", 1)]
        [TestCase("EventArgs args, object s", 1)]
        [TestCase("EventArgs e, EventArgs a", 2)]
        [TestCase("object s, EventArgs args, object whatever", 1)]
        [TestCase("object whatever, object s, EventArgs args", 1)]
        [TestCase("DependencyPropertyChangedEventArgs args", 1)]
        [TestCase("DependencyPropertyChangedEventArgs args, object s", 1)]
        [TestCase("DependencyPropertyChangedEventArgs e, DependencyPropertyChangedEventArgs a", 2)]
        [TestCase("object s, DependencyPropertyChangedEventArgs args, object whatever", 1)]
        [TestCase("object whatever, object s, DependencyPropertyChangedEventArgs args", 1)]
        public void An_issue_is_reported_for_matching_parameters_on_local_function_(string parameters, int violations) => An_issue_is_reported_for(violations, @"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction(" + parameters + @")
            {
            }
        }
    }
}
");

        [TestCase("EventArgs e")]
        [TestCase("EventArgs e, string a")]
        [TestCase("EventArgs e0, EventArgs e1")]
        [TestCase("DependencyPropertyChangedEventArgs e")]
        [TestCase("DependencyPropertyChangedEventArgs e, string a")]
        [TestCase("DependencyPropertyChangedEventArgs e0, DependencyPropertyChangedEventArgs e1")]
        public void No_issue_is_reported_for_correctly_named_parameters_on_method_(string parameters) => No_issue_is_reported_for(@"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void DoSomething(" + parameters + @") { }
    }
}
");

        [TestCase("EventArgs e")]
        [TestCase("EventArgs e, string a")]
        [TestCase("EventArgs e0, EventArgs e1")]
        [TestCase("DependencyPropertyChangedEventArgs e")]
        [TestCase("DependencyPropertyChangedEventArgs e, string a")]
        [TestCase("DependencyPropertyChangedEventArgs e0, DependencyPropertyChangedEventArgs e1")]
        public void No_issue_is_reported_for_correctly_named_parameters_on_local_function_(string parameters) => No_issue_is_reported_for(@"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction(" + parameters + @")
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_setter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public EventArgs Property { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_local_function_if_surrounding_method_contains_parameter_names() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void OnWhatever(object sender, EventArgs e)
    {
        void LocalFunction(object s, EventArgs args)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_local_function_if_surrounding_method_is_event_handling_method() => No_issue_is_reported_for(@"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void OnWhatever(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            void SomeLocal(DependencyObject dep, DependencyPropertyChangedEventArgs args) { }
        }
    }
}
");

        [TestCase("EventArgs args", "EventArgs e")]
        [TestCase("EventArgs args, EventArgs eventArgs", "EventArgs e0, EventArgs e1")]
        [TestCase("EventArgs args, int i", "EventArgs e, int i")]
        [TestCase("EventArgs args, int i, EventArgs eventArgs", "EventArgs e0, int i, EventArgs e1")]
        [TestCase("DependencyPropertyChangedEventArgs args", "DependencyPropertyChangedEventArgs e")]
        public void Code_gets_fixed_for_method_(string expected, string wanted)
        {
            const string Template = @"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    class TestMe
    {
        void DoSomething(###)
        {
        }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", expected), Template.Replace("###", wanted));
        }

        [TestCase("EventArgs args", "EventArgs e")]
        [TestCase("EventArgs args, EventArgs eventArgs", "EventArgs e0, EventArgs e1")]
        [TestCase("EventArgs args, int i", "EventArgs e, int i")]
        [TestCase("EventArgs args, int i, EventArgs eventArgs", "EventArgs e0, int i, EventArgs e1")]
        [TestCase("DependencyPropertyChangedEventArgs args", "DependencyPropertyChangedEventArgs e")]
        public void Code_gets_fixed_for_local_function_(string expected, string wanted)
        {
            const string Template = @"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction(###)
            {
            }
        }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", expected), Template.Replace("###", wanted));
        }

        protected override string GetDiagnosticId() => MiKo_1001_EventArgsParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1001_EventArgsParameterAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1001_CodeFixProvider();
    }
}