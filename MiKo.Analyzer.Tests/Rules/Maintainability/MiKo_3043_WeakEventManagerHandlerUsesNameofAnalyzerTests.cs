using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3043_WeakEventManagerHandlerUsesNameofAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_AddHandler_with_nameof_applied() => No_issue_is_reported_for(@"
using System;
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(Control control)
        {
            WeakEventManager<Control, MouseButtonEventArgs>.AddHandler(control, nameof(MouseDoubleClick), OnMouseDoubleClick);
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_AddHandler_without_nameof_applied() => An_issue_is_reported_for(@"
using System;
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(Control control)
        {
            WeakEventManager<Control, MouseButtonEventArgs>.AddHandler(control, ""MouseDoubleClick"", OnMouseDoubleClick);
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_RemoveHandler_with_nameof_applied() => No_issue_is_reported_for(@"
using System;
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(Control control)
        {
            WeakEventManager<Control, MouseButtonEventArgs>.RemoveHandler(control, nameof(MouseDoubleClick), OnMouseDoubleClick);
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_RemoveHandler_without_nameof_applied() => An_issue_is_reported_for(@"
using System;
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(Control control)
        {
            WeakEventManager<Control, MouseButtonEventArgs>.RemoveHandler(control, ""MouseDoubleClick"", OnMouseDoubleClick);
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }
    }
}");

        [TestCase("AddHandler", "\"MouseDoubleClick\"", "nameof(Control.MouseDoubleClick)")]
        [TestCase("RemoveHandler", "\"MouseDoubleClick\"", "nameof(Control.MouseDoubleClick)")]
        public void Code_gets_fixed_for_(string methodName, string originalArgument, string fixedArgument)
        {
            const string Template = @"
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace System.Windows.Controls
{
    public class Control
    {
        public event System.Windows.Input.MouseButtonEventHandler MouseDoubleClick;
    }
}

namespace System.Windows.Input
{
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);

    public class MouseButtonEventArgs : EventArgs
    {}
}

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(Control control)
        {
            WeakEventManager<Control, MouseButtonEventArgs>.#1#(control, #2#, OnMouseDoubleClick);
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }
    }
}";

            var originalCode = Template.Replace("#1#", methodName).Replace("#2#", originalArgument);
            var fixedCode = Template.Replace("#1#", methodName).Replace("#2#", fixedArgument);

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3043_WeakEventManagerHandlerUsesNameofAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3043_WeakEventManagerHandlerUsesNameofAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3043_CodeFixProvider();
    }
}