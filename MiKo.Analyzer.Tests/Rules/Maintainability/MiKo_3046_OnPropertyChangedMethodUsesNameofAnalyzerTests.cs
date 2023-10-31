using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3046_OnPropertyChangedMethodUsesNameofAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] MethodNames =
                                                       {
                                                           "NotifyPropertyChanged",
                                                           "NotifyPropertyChanging",
                                                           "OnNotifyPropertyChanged",
                                                           "OnNotifyPropertyChanging",
                                                           "OnPropertyChanged",
                                                           "OnPropertyChanging",
                                                           "OnRaisePropertyChanged",
                                                           "OnRaisePropertyChanging",
                                                           "OnTriggerPropertyChanged",
                                                           "OnTriggerPropertyChanging",
                                                           "RaisePropertyChanged",
                                                           "RaisePropertyChanging",
                                                           "TriggerPropertyChanged",
                                                           "TriggerPropertyChanging",
                                                       };

        [Test]
        public void No_issue_is_reported_for_method_with_nameof_applied_([ValueSource(nameof(MethodNames))] string methodName) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public string MyProperty { get; set; }

        private void DoSomething()
        {
            " + methodName + @"(nameof(MyProperty));
        }

        private void " + methodName + @"(string name)
        { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_method_with_out_nameof_applied_([ValueSource(nameof(MethodNames))] string methodName) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public string MyProperty { get; set; }

        private void DoSomething()
        {
            " + methodName + @"(""MyProperty"");
        }

        private void " + methodName + @"(string name)
        { }
    }
}");

        [Test]
        public void Code_gets_fixed_for_([ValueSource(nameof(MethodNames))] string methodName)
        {
            var originalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public string MyProperty { get; set; }

        private void DoSomething()
        {
            " + methodName + @"(""MyProperty"");
        }

        private void " + methodName + @"(string name)
        { }
    }
}
";

            var fixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public string MyProperty { get; set; }

        private void DoSomething()
        {
            " + methodName + @"(nameof(MyProperty));
        }

        private void " + methodName + @"(string name)
        { }
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3046_OnPropertyChangedMethodUsesNameofAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3046_OnPropertyChangedMethodUsesNameofAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3046_CodeFixProvider();
    }
}