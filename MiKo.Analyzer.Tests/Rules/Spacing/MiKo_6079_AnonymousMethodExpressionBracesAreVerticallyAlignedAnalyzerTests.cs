using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6079_AnonymousMethodExpressionBracesAreVerticallyAlignedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_anonymous_method_vertically_aligned_to_delegate_keyword() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, object> x = delegate (int i)
                              {
                                  return new
                                             {
                                                 A = i,
                                                 B = i,
                                                 C = i,
                                                 D = i,
                                                 E = i,
                                                 F = i,
                                             };
                              };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_block_of_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_anonymous_method_vertically_aligned_to_delegate_keyword_but_indented_to_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, object> x = delegate (int i)
                          {
                              return new
                                         {
                                             A = i,
                                             B = i,
                                             C = i,
                                             D = i,
                                             E = i,
                                             F = i,
                                         };
                          };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_anonymous_method_vertically_aligned_to_delegate_keyword_but_indented_to_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, object> x = delegate (int i)
                                  {
                                      return new
                                                 {
                                                     A = i,
                                                     B = i,
                                                     C = i,
                                                     D = i,
                                                     E = i,
                                                     F = i,
                                                 };
                                  };
    }
}
");

        [Test]
        public void Code_gets_fixed_for_anonymous_method_vertically_aligned_to_delegate_keyword_but_indented_to_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, object> x = delegate (int i)
                          {
                              return new
                                         {
                                             A = i,
                                             B = i,
                                             C = i,
                                             D = i,
                                             E = i,
                                             F = i,
                                         };
                          };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, object> x = delegate (int i)
                              {
                                  return new
                                             {
                                                 A = i,
                                                 B = i,
                                                 C = i,
                                                 D = i,
                                                 E = i,
                                                 F = i,
                                             };
                              };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_anonymous_method_vertically_aligned_to_delegate_keyword_but_indented_to_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, object> x = delegate (int i)
                                  {
                                      return new
                                                 {
                                                     A = i,
                                                     B = i,
                                                     C = i,
                                                     D = i,
                                                     E = i,
                                                     F = i,
                                                 };
                                  };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, object> x = delegate (int i)
                              {
                                  return new
                                             {
                                                 A = i,
                                                 B = i,
                                                 C = i,
                                                 D = i,
                                                 E = i,
                                                 F = i,
                                             };
                              };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiline_delegate_parameters_with_opening_brace_on_closing_parameter_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, int, int> x = delegate (int i,
                                          int value) {
                                    return i;
                                };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, int, int> x = delegate (int i,
                                          int value)
                                {
                                    return i;
                                };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiline_delegate_parameters_with_opening_brace_on_closing_parameter_line_and_trailing_comment_before_opening_brace()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, int, int> x = delegate (int i,
                                          int value) /* keep */ {
                                    return i;
                                };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, int, int> x = delegate (int i,
                                          int value) /* keep */ 
                                {
                                    return i;
                                };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_block_preserving_outer_braces_and_region_directives()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, int> x = delegate (int i)
                          {
                              #region Test
                              {
                                  return i;
                              }
                              #endregion
                          };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, int> x = delegate (int i)
                           {
                               #region Test
                               {
                                   return i;
                               }
                               #endregion
                           };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6079_AnonymousMethodExpressionBracesAreVerticallyAlignedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6079_AnonymousMethodExpressionBracesAreVerticallyAlignedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6079_CodeFixProvider();
    }
}