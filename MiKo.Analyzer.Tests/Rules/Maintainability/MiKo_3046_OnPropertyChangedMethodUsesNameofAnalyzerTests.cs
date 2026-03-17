using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3046_OnPropertyChangedMethodUsesNameofAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] MethodNames =
                                                       [
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
                                                       ];

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
            const string OriginalCode = """

                                        using System;

                                        namespace Bla
                                        {
                                            public class TestMe
                                            {
                                                public string MyProperty { get; set; }

                                                private void DoSomething()
                                                {
                                                    ###("MyProperty");
                                                }

                                                private void ###(string name)
                                                { }
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     using System;

                                     namespace Bla
                                     {
                                         public class TestMe
                                         {
                                             public string MyProperty { get; set; }

                                             private void DoSomething()
                                             {
                                                 ###(nameof(MyProperty));
                                             }

                                             private void ###(string name)
                                             { }
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode.Replace("###", methodName), FixedCode.Replace("###", methodName));
        }

        [Test]
        public void Code_gets_fixed_for_empty_string_at_([ValueSource(nameof(MethodNames))] string methodName)
        {
            const string OriginalCode = """

                                        using System;

                                        namespace Bla
                                        {
                                            public class TestMe
                                            {
                                                public string MyProperty { get; set; }

                                                private void DoSomething()
                                                {
                                                    ###("");
                                                }

                                                private void ###(string name)
                                                { }
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     using System;

                                     namespace Bla
                                     {
                                         public class TestMe
                                         {
                                             public string MyProperty { get; set; }

                                             private void DoSomething()
                                             {
                                                 ###(string.Empty);
                                             }

                                             private void ###(string name)
                                             { }
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode.Replace("###", methodName), FixedCode.Replace("###", methodName));
        }

        protected override string GetDiagnosticId() => MiKo_3046_OnPropertyChangedMethodUsesNameofAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3046_OnPropertyChangedMethodUsesNameofAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3046_CodeFixProvider();
    }
}