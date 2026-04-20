using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6073_LinqQueriesAreVerticallyAlignedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_vertically_aligned_Linq_query() => No_issue_is_reported_for("""

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from item in new List<string>()
                                 where item.Length > 0
                                 select item;
                }
            }

            """);

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_Linq_query_with_query_continuation_on_same_line_as_group() => No_issue_is_reported_for("""

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public static IEnumerable<(string Key, int Count)> GetGroupCounts(IEnumerable<string> words)
                {
                    var result = from word in words
                                 group word by word.ToLower() into g   // <-- QueryContinuationSyntax starts here
                                 where g.Count() > 1                   //     QueryBodySyntax inside the continuation
                                 select (Key: g.Key, Count: g.Count());

                    return result;
                }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_select_clause_is_indented() => An_issue_is_reported_for("""

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from item in new List<string>()
                                 where item.Length > 0
                                  select item;
                }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_select_clause_is_outdented() => An_issue_is_reported_for("""

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from item in new List<string>()
                                 where item.Length > 0
                                select item;
                }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_where_clause_is_indented() => An_issue_is_reported_for("""

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from item in new List<string>()
                                  where item.Length > 0
                                 select item;
                }
            }

            """);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_where_clause_is_indented_and_select_clause_is_outdented() => An_issue_is_reported_for(2, """

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from item in new List<string>()
                                  where item.Length > 0
                                select item;
                }
            }

            """);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_where_clause_is_indented_and_select_clause_is_indented() => An_issue_is_reported_for(2, """

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from item in new List<string>()
                                  where item.Length > 0
                                  select item;
                }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_where_clause_is_outdented() => An_issue_is_reported_for("""

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from item in new List<string>()
                                where item.Length > 0
                                 select item;
                }
            }

            """);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_where_clause_is_outdented_and_select_clause_is_outdented() => An_issue_is_reported_for(2, """

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from item in new List<string>()
                                where item.Length > 0
                                select item;
                }
            }

            """);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_where_clause_is_outdented_and_select_clause_is_indented() => An_issue_is_reported_for(2, """

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from item in new List<string>()
                                where item.Length > 0
                                  select item;
                }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_from_clause_is_indented() => An_issue_is_reported_for("""

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from x in new List<string>()
                                  from y in new List<string>()
                                 where x.Length != y.Length
                                 select x;
                }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_from_clause_is_outdented() => An_issue_is_reported_for("""

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from x in new List<string>()
                                from y in new List<string>()
                                 where x.Length != y.Length
                                 select x;
                }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_let_clause_is_indented() => An_issue_is_reported_for("""

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from item in new List<string>()
                                  let count = item.Length
                                 where count > 0
                                 select item;
                }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_let_clause_is_outdented() => An_issue_is_reported_for("""

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public void DoSomething()
                {
                    var result = from item in new List<string>()
                                let count = item.Length
                                 where count > 0
                                 select item;
                }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_continuation_clause_is_indented() => An_issue_is_reported_for("""

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public static IEnumerable<(string Key, int Count)> GetGroupCounts(IEnumerable<string> words)
                {
                    var result = from word in words
                                 group word by word.ToLower()
                                  into g   // <-- QueryContinuationSyntax starts here
                                 where g.Count() > 1
                                 select (Key: g.Key, Count: g.Count());

                    return result;
                }
            }

            """);

        [Test]
        public void An_issue_is_reported_for_vertically_aligned_Linq_query_if_continuation_clause_is_outdented() => An_issue_is_reported_for("""

            using System;
            using System.Collections.Generic;
            using System.Linq;

            public class TestMe
            {
                public static IEnumerable<(string Key, int Count)> GetGroupCounts(IEnumerable<string> words)
                {
                    var result = from word in words
                                 group word by word.ToLower()
                                into g   // <-- QueryContinuationSyntax starts here
                                 where g.Count() > 1
                                 select (Key: g.Key, Count: g.Count());

                    return result;
                }
            }

            """);

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_select_clause_is_indented()
        {
            const string OriginalCode = """

                                        using System;
                                        using System.Collections.Generic;
                                        using System.Linq;

                                        public class TestMe
                                        {
                                            public void DoSomething()
                                            {
                                                var result = from item in new List<string>()
                                                             where item.Length > 0
                                                              select item;
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     using System;
                                     using System.Collections.Generic;
                                     using System.Linq;

                                     public class TestMe
                                     {
                                         public void DoSomething()
                                         {
                                             var result = from item in new List<string>()
                                                          where item.Length > 0
                                                          select item;
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_select_clause_is_outdented()
        {
            const string OriginalCode = """

                                        using System;
                                        using System.Collections.Generic;
                                        using System.Linq;

                                        public class TestMe
                                        {
                                            public void DoSomething()
                                            {
                                                var result = from item in new List<string>()
                                                             where item.Length > 0
                                                            select item;
                                            }
                                        }

                                        """;
            const string FixedCode = """

                                     using System;
                                     using System.Collections.Generic;
                                     using System.Linq;

                                     public class TestMe
                                     {
                                         public void DoSomething()
                                         {
                                             var result = from item in new List<string>()
                                                          where item.Length > 0
                                                          select item;
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_where_clause_is_indented()
        {
            const string OriginalCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                      where item.Length > 0
                                     select item;
                    }
                }

                """;

            const string FixedCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                     where item.Length > 0
                                     select item;
                    }
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_where_clause_is_indented_and_select_clause_is_outdented()
        {
            const string OriginalCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                      where item.Length > 0
                                    select item;
                    }
                }

                """;

            const string FixedCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                     where item.Length > 0
                                     select item;
                    }
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_where_clause_is_indented_and_select_clause_is_indented()
        {
            const string OriginalCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                      where item.Length > 0
                                      select item;
                    }
                }

                """;

            const string FixedCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                     where item.Length > 0
                                     select item;
                    }
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_where_clause_is_outdented()
        {
            const string OriginalCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                    where item.Length > 0
                                     select item;
                    }
                }

                """;

            const string FixedCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                     where item.Length > 0
                                     select item;
                    }
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_where_clause_is_outdented_and_select_clause_is_outdented()
        {
            const string OriginalCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                    where item.Length > 0
                                    select item;
                    }
                }

                """;

            const string FixedCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                     where item.Length > 0
                                     select item;
                    }
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_where_clause_is_outdented_and_select_clause_is_indented()
        {
            const string OriginalCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                    where item.Length > 0
                                      select item;
                    }
                }

                """;

            const string FixedCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                     where item.Length > 0
                                     select item;
                    }
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_from_clause_is_indented()
        {
            const string OriginalCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from x in new List<string>()
                                      from y in new List<string>()
                                     where x.Length != y.Length
                                     select x;
                    }
                }

                """;

            const string FixedCode = """

                                     using System;
                                     using System.Collections.Generic;
                                     using System.Linq;

                                     public class TestMe
                                     {
                                         public void DoSomething()
                                         {
                                             var result = from x in new List<string>()
                                                          from y in new List<string>()
                                                          where x.Length != y.Length
                                                          select x;
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_from_clause_is_outdented()
        {
            const string OriginalCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from x in new List<string>()
                                    from y in new List<string>()
                                     where x.Length != y.Length
                                     select x;
                    }
                }

                """;

            const string FixedCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from x in new List<string>()
                                     from y in new List<string>()
                                     where x.Length != y.Length
                                     select x;
                    }
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_let_clause_is_indented()
        {
            const string OriginalCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                      let count = item.Length
                                     where count > 0
                                     select item;
                    }
                }

                """;

            const string FixedCode = """

                                     using System;
                                     using System.Collections.Generic;
                                     using System.Linq;

                                     public class TestMe
                                     {
                                         public void DoSomething()
                                         {
                                             var result = from item in new List<string>()
                                                          let count = item.Length
                                                          where count > 0
                                                          select item;
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_let_clause_is_outdented()
        {
            const string OriginalCode = """

                using System;
                using System.Collections.Generic;
                using System.Linq;

                public class TestMe
                {
                    public void DoSomething()
                    {
                        var result = from item in new List<string>()
                                    let count = item.Length
                                     where count > 0
                                     select item;
                    }
                }

                """;

            const string FixedCode = """

                                     using System;
                                     using System.Collections.Generic;
                                     using System.Linq;

                                     public class TestMe
                                     {
                                         public void DoSomething()
                                         {
                                             var result = from item in new List<string>()
                                                          let count = item.Length
                                                          where count > 0
                                                          select item;
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_continuation_clause_is_indented()
        {
            const string OriginalCode = """

                                        using System;
                                        using System.Collections.Generic;
                                        using System.Linq;

                                        public class TestMe
                                        {
                                            public static IEnumerable<(string Key, int Count)> GetGroupCounts(IEnumerable<string> words)
                                            {
                                                var result = from word in words
                                                             group word by word.ToLower()
                                                              into g   // <-- QueryContinuationSyntax starts here
                                                             where g.Count() > 1
                                                             select (Key: g.Key, Count: g.Count());

                                                return result;
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     using System;
                                     using System.Collections.Generic;
                                     using System.Linq;

                                     public class TestMe
                                     {
                                         public static IEnumerable<(string Key, int Count)> GetGroupCounts(IEnumerable<string> words)
                                         {
                                             var result = from word in words
                                                          group word by word.ToLower()
                                                          into g   // <-- QueryContinuationSyntax starts here
                                                          where g.Count() > 1
                                                          select (Key: g.Key, Count: g.Count());

                                             return result;
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_vertically_aligned_Linq_query_if_continuation_clause_is_outdented()
        {
            const string OriginalCode = """

                                        using System;
                                        using System.Collections.Generic;
                                        using System.Linq;

                                        public class TestMe
                                        {
                                            public static IEnumerable<(string Key, int Count)> GetGroupCounts(IEnumerable<string> words)
                                            {
                                                var result = from word in words
                                                             group word by word.ToLower()
                                                            into g   // <-- QueryContinuationSyntax starts here
                                                             where g.Count() > 1
                                                             select (Key: g.Key, Count: g.Count());

                                                return result;
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     using System;
                                     using System.Collections.Generic;
                                     using System.Linq;

                                     public class TestMe
                                     {
                                         public static IEnumerable<(string Key, int Count)> GetGroupCounts(IEnumerable<string> words)
                                         {
                                             var result = from word in words
                                                          group word by word.ToLower()
                                                          into g   // <-- QueryContinuationSyntax starts here
                                                          where g.Count() > 1
                                                          select (Key: g.Key, Count: g.Count());

                                             return result;
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6073_LinqQueriesAreVerticallyAlignedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6073_LinqQueriesAreVerticallyAlignedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6073_CodeFixProvider();
    }
}