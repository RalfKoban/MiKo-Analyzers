using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6072_BaseExpressionSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_non_base_statements_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomething();
            DoSomething();
            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_base_statement_as_only_statement_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMeBase
    {
        public virtual void DoSomething()
        {
        }
    }

    public class TestMe : TestMeBase
    {
        public override void DoSomething()
        {
            base.DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_base_statement_as_only_statement_in_arrow_clause_method_expression_body() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMeBase
    {
        public virtual void DoSomething()
        {
        }
    }

    public class TestMe : TestMeBase
    {
        public override void DoSomething() => base.DoSomething();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_base_statement_as_only_statements_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMeBase
    {
        public virtual void DoSomething()
        {
        }
    }

    public class TestMe : TestMeBase
    {
        public override void DoSomething()
        {
            base.DoSomething();
            base.DoSomething();
            base.DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_base_statement_with_blank_line_before() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMeBase
    {
        public virtual void DoSomething()
        {
        }
    }

    public class TestMe : TestMeBase
    {
        public override void DoSomething()
        {
            DoSomethingElse();

            base.DoSomething();
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_base_statement_with_blank_line_after() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMeBase
    {
        public virtual void DoSomething()
        {
        }
    }

    public class TestMe : TestMeBase
    {
        public override void DoSomething()
        {
            base.DoSomething();

            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_base_statement_with_blank_line_before_and_after() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMeBase
    {
        public virtual void DoSomething()
        {
        }
    }

    public class TestMe : TestMeBase
    {
        public void DoSomething()
        {
            DoSomethingElse();

            base.DoSomething();

            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_base_statement_without_blank_line_before() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMeBase
    {
        public virtual void DoSomething()
        {
        }
    }

    public class TestMe : TestMeBase
    {
        public void DoSomething()
        {
            DoSomethingElse();
            base.DoSomething();

            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_base_statement_without_blank_line_after() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMeBase
    {
        public virtual void DoSomething()
        {
        }
    }

    public class TestMe : TestMeBase
    {
        public void DoSomething()
        {
            DoSomethingElse();

            base.DoSomething();
            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_base_statement_without_blank_line_before_and_after() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMeBase
    {
        public virtual void DoSomething()
        {
        }
    }

    public class TestMe : TestMeBase
    {
        public void DoSomething()
        {
            DoSomethingElse();
            base.DoSomething();
            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_base_statement_without_blank_line_before()
        {
            const string OriginalCode = """

                                        namespace Bla
                                        {
                                            public class TestMeBase
                                            {
                                                public virtual void DoSomething()
                                                {
                                                }
                                            }

                                            public class TestMe : TestMeBase
                                            {
                                                public void DoSomething()
                                                {
                                                    DoSomethingElse();
                                                    base.DoSomething();

                                                    DoSomethingElse();
                                                }

                                                private void DoSomethingElse()
                                                {
                                                }
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                        namespace Bla
                                        {
                                            public class TestMeBase
                                            {
                                                public virtual void DoSomething()
                                                {
                                                }
                                            }

                                            public class TestMe : TestMeBase
                                            {
                                                public void DoSomething()
                                                {
                                                    DoSomethingElse();

                                                    base.DoSomething();

                                                    DoSomethingElse();
                                                }

                                                private void DoSomethingElse()
                                                {
                                                }
                                            }
                                        }

                                        """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_base_statement_without_blank_line_after()
        {
            const string OriginalCode = """

                                        namespace Bla
                                        {
                                            public class TestMeBase
                                            {
                                                public virtual void DoSomething()
                                                {
                                                }
                                            }

                                            public class TestMe : TestMeBase
                                            {
                                                public void DoSomething()
                                                {
                                                    DoSomethingElse();

                                                    base.DoSomething();
                                                    DoSomethingElse();
                                                }

                                                private void DoSomethingElse()
                                                {
                                                }
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     namespace Bla
                                     {
                                         public class TestMeBase
                                         {
                                             public virtual void DoSomething()
                                             {
                                             }
                                         }

                                         public class TestMe : TestMeBase
                                         {
                                             public void DoSomething()
                                             {
                                                 DoSomethingElse();

                                                 base.DoSomething();

                                                 DoSomethingElse();
                                             }

                                             private void DoSomethingElse()
                                             {
                                             }
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_base_statement_without_blank_line_before_or_after()
        {
            const string OriginalCode = """

                                        namespace Bla
                                        {
                                            public class TestMeBase
                                            {
                                                public virtual void DoSomething()
                                                {
                                                }
                                            }

                                            public class TestMe : TestMeBase
                                            {
                                                public void DoSomething()
                                                {
                                                    DoSomethingElse();
                                                    base.DoSomething();
                                                    DoSomethingElse();
                                                }

                                                private void DoSomethingElse()
                                                {
                                                }
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     namespace Bla
                                     {
                                         public class TestMeBase
                                         {
                                             public virtual void DoSomething()
                                             {
                                             }
                                         }

                                         public class TestMe : TestMeBase
                                         {
                                             public void DoSomething()
                                             {
                                                 DoSomethingElse();

                                                 base.DoSomething();

                                                 DoSomethingElse();
                                             }

                                             private void DoSomethingElse()
                                             {
                                             }
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6072_BaseExpressionSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6072_BaseExpressionSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6072_CodeFixProvider();
    }
}