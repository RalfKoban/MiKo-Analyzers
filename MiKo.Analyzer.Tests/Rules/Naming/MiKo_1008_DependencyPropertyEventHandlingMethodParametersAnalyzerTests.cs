using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzerTests : CodeFixVerifier
    {
        [TestCase("")]
        [TestCase("DependencyPropertyChangedEventArgs args")]
        [TestCase("DependencyObject d")]
        [TestCase("DependencyPropertyChangedEventArgs args, DependencyObject d")]
        [TestCase("DependencyObject d, DependencyPropertyChangedEventArgs args, object whatever")]
        [TestCase("object whatever, DependencyObject d, DependencyPropertyChangedEventArgs args")]
        public void No_issue_is_reported_for_non_event_handling_method_(string parameters) => No_issue_is_reported_for(@"
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

        [Test]
        public void No_issue_is_reported_for_correctly_named_method() => No_issue_is_reported_for(@"
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
        public void OnWhatever(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_DependencyObject() => An_issue_is_reported_for(@"
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
        public void OnWhatever(DependencyObject s, DependencyPropertyChangedEventArgs e) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_DependencyObject_on_overridden_method() => An_issue_is_reported_for(@"
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
        public override void OnWhatever(DependencyObject s, DependencyPropertyChangedEventArgs e) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_DependencyPropertyChangedEventArgs() => An_issue_is_reported_for(@"
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
        public void OnWhatever(DependencyObject d, DependencyPropertyChangedEventArgs args) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_DependencyPropertyChangedEventArgs_on_overridden_method() => An_issue_is_reported_for(@"
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
        public override void OnWhatever(DependencyObject d, DependencyPropertyChangedEventArgs args) { }
    }
}");

        [Test]
        public void Code_gets_fixed()
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

    public class TestMe
    {
        public override void OnWhatever(DependencyObject #1, DependencyPropertyChangedEventArgs #2) { }
    }
}";

            var originalCode = Template.Replace("#1", "obj").Replace("#2", "args");
            var fixedCode = Template.Replace("#1", "d").Replace("#2", "e");

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1008_CodeFixProvider();
    }
}