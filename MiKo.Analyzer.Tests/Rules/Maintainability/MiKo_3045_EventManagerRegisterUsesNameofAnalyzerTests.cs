using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3045_EventManagerRegisterUsesNameofAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_RegisterRoutedEvent_with_nameof_applied() => No_issue_is_reported_for(@"
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bla
{
    public class TestMeUserControl : UserControl
    {
        public static readonly RoutedEvent MyDataEvent = EventManager.RegisterRoutedEvent(nameof(MyData), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TestMeUserControl));

        public event RoutedEventHandler MyData;
    }
}");

        [Test]
        public void An_issue_is_reported_for_RegisterRoutedEvent_without_nameof_applied() => An_issue_is_reported_for(@"
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bla
{
    public class TestMeUserControl : UserControl
    {
        public static readonly RoutedEvent MyDataEvent = EventManager.RegisterRoutedEvent(""MyData"", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TestMeUserControl));

        public event RoutedEventHandler MyData;
    }
}");

        [Test]
        public void Code_gets_fixed()
        {
            const string Template = @"
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bla
{
    public class TestMeUserControl : UserControl
    {
        public static readonly RoutedEvent MyDataEvent = EventManager.RegisterRoutedEvent(#1#, RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TestMeUserControl));

        public event RoutedEventHandler MyData;
    }
}";

            var originalCode = Template.Replace("#1#", "\"MyData\"");
            var fixedCode = Template.Replace("#1#", "nameof(MyData)");

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3045_EventManagerRegisterUsesNameofAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3045_EventManagerRegisterUsesNameofAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3045_CodeFixProvider();
    }
}