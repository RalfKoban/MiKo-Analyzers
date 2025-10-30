using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2016_AsyncMethodDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public async void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_non_async_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>Does something.</summary>
    public void DoSomething() { }
}
");

        [TestCase("void")]
        [TestCase("Task")]
        [TestCase("Task<int>")]
        public void No_issue_is_reported_for_correctly_documented_async_method_(string returnType) => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Asynchronously does something.</summary>
    public async " + returnType + @" DoSomething() { }
}
");

        [TestCase("Task")]
        [TestCase("Task<int>")]
        public void No_issue_is_reported_for_correctly_documented_non_async_Task_method_(string returnType) => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Asynchronously does something.</summary>
    public " + returnType + @" DoSomething() { }
}
");

        [TestCase("void")]
        [TestCase("Task")]
        [TestCase("Task<int>")]
        public void An_issue_is_reported_for_incorrectly_documented_async_method_(string returnType) => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    public async " + returnType + @" DoSomething() { }
}
");

        [TestCase("Task")]
        [TestCase("Task<int>")]
        public void An_issue_is_reported_for_incorrectly_documented_Task_method_(string returnType) => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    public " + returnType + @" DoSomething() { }
}
");

        [TestCase("Task")]
        [TestCase("Task<int>")]
        public void An_issue_is_reported_for_comment_with_see_cref_only_(string returnType) => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// <see cref=""DoSomething""/>
    /// </summary>
    public " + returnType + @" DoSomething() { }
}

");

        [Test]
        public void Code_gets_fixed_and_upper_case_text_adjusted()
        {
            const string OriginalCode = """

                                        public class TestMe
                                        {
                                            /// <summary>Does something.</summary>
                                            public async void DoSomething() { }
                                        }
                                        """;

            const string FixedCode = """

                                     public class TestMe
                                     {
                                         /// <summary>
                                         /// Asynchronously does something.
                                         /// </summary>
                                         public async void DoSomething() { }
                                     }
                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_and_intended_upper_case_text_adjusted()
        {
            const string OriginalCode = """

                                        public class TestMe
                                        {
                                            /// <summary>   Does something.</summary>
                                            public async void DoSomething() { }
                                        }
                                        """;

            const string FixedCode = """

                                     public class TestMe
                                     {
                                         /// <summary>
                                         /// Asynchronously does something.
                                         /// </summary>
                                         public async void DoSomething() { }
                                     }
                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_and_lower_case_text_kept()
        {
            const string OriginalCode = """

                                        public class TestMe
                                        {
                                            /// <summary>does something.</summary>
                                            public async void DoSomething() { }
                                        }
                                        """;

            const string FixedCode = """

                                     public class TestMe
                                     {
                                         /// <summary>
                                         /// Asynchronously does something.
                                         /// </summary>
                                         public async void DoSomething() { }
                                     }
                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_and_seeCref_element_moved()
        {
            const string OriginalCode = """

                                        public class TestMe
                                        {
                                            /// <summary><see cref="string"/></summary>
                                            public async void DoSomething() { }
                                        }
                                        """;

            const string FixedCode = """

                                     public class TestMe
                                     {
                                         /// <summary>
                                         /// Asynchronously <see cref="string"/>
                                         /// </summary>
                                         public async void DoSomething() { }
                                     }
                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_and_upper_case_text_adjusted_when_on_different_lines()
        {
            const string OriginalCode = """

                                        public class TestMe
                                        {
                                            /// <summary>
                                            /// Does something.
                                            /// </summary>
                                            public async void DoSomething() { }
                                        }
                                        """;

            const string FixedCode = """

                                     public class TestMe
                                     {
                                         /// <summary>
                                         /// Asynchronously does something.
                                         /// </summary>
                                         public async void DoSomething() { }
                                     }
                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_and_seeCref_element_moved_when_on_different_lines()
        {
            const string OriginalCode = """

                                        public class TestMe
                                        {
                                            /// <summary>
                                            /// <see cref="string"/>
                                            /// </summary>
                                            public async void DoSomething() { }
                                        }
                                        """;

            const string FixedCode = """

                                     public class TestMe
                                     {
                                         /// <summary>
                                         /// Asynchronously <see cref="string"/>
                                         /// </summary>
                                         public async void DoSomething() { }
                                     }
                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_and_upper_case_text_adjusted_when_infinite_and_on_different_lines()
        {
            const string OriginalCode = """

                                        public class TestMe
                                        {
                                            /// <summary>
                                            /// Do something.
                                            /// </summary>
                                            public async void DoSomething() { }
                                        }
                                        """;

            const string FixedCode = """

                                     public class TestMe
                                     {
                                         /// <summary>
                                         /// Asynchronously does something.
                                         /// </summary>
                                         public async void DoSomething() { }
                                     }
                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("This method is called", "Asynchronously invoked")]
        [TestCase("The method is called", "Asynchronously invoked")]
        [TestCase("The method that is called", "Asynchronously invoked")]
        [TestCase("The method which is called", "Asynchronously invoked")]
        [TestCase("Method that is called", "Asynchronously invoked")]
        [TestCase("Method which is called", "Asynchronously invoked")]
        [TestCase("This method gets called", "Asynchronously invoked")]
        [TestCase("The method gets called", "Asynchronously invoked")]
        [TestCase("The method that gets called", "Asynchronously invoked")]
        [TestCase("The method which gets called", "Asynchronously invoked")]
        [TestCase("Method that gets called", "Asynchronously invoked")]
        [TestCase("Method which gets called", "Asynchronously invoked")]
        [TestCase("Callback that is called", "Asynchronously invoked")]
        [TestCase("Callback which is called", "Asynchronously invoked")]
        [TestCase("A callback that is called", "Asynchronously invoked")]
        [TestCase("A callback which is called", "Asynchronously invoked")]
        [TestCase("The callback that is called", "Asynchronously invoked")]
        [TestCase("The callback which is called", "Asynchronously invoked")]
        public void Code_gets_fixed_for_(string originalText, string fixedText)
        {
            const string Template = """

                                    public class TestMe
                                    {
                                        /// <summary>
                                        /// ### to do something.
                                        /// </summary>
                                        public async Task DoSomethingAsync() { }
                                    }
                                    """;

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", fixedText));
        }

        protected override string GetDiagnosticId() => MiKo_2016_AsyncMethodDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2016_AsyncMethodDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2016_CodeFixProvider();
    }
}