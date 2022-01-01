using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

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
        public void DoSomething()
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
        public void DoSomething()
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
        public void DoSomething()
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
        public void DoSomething()
        {
            WeakEventManager<Control, MouseButtonEventArgs>.RemoveHandler(control, ""MouseDoubleClick"", OnMouseDoubleClick);
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }
    }
}");

        protected override string GetDiagnosticId() => MiKo_3043_WeakEventManagerHandlerUsesNameofAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3043_WeakEventManagerHandlerUsesNameofAnalyzer();
    }
}