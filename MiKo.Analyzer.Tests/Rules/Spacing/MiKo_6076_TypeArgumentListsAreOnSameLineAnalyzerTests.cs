using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6076_TypeArgumentListsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_when_type_argument_list_is_on_same_line() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<int>(42);
}
");

        [Test]
        public void No_issue_is_reported_when_type_argument_list_and_nested_type_argument_list_are_on_same_line() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public List<IEnumerable<int>> GetItems() => new List<IEnumerable<int>>(42);
}
");

        [Test]
        public void An_issue_is_reported_when_opening_token_of_type_argument_list_of_creation_is_on_multiple_lines() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<
                                            int>(42);
}
");

        [Test]
        public void An_issue_is_reported_when_closing_token_of_type_argument_list_of_creation_is_on_multiple_lines() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<int
                                               >(42);
}
");

        [Test]
        public void An_issue_is_reported_when_opening_token_and_closing_token_of_type_argument_list_of_creation_is_on_multiple_lines() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<
                                            int
                                               >(42);
}
");

        [Test]
        public void An_issue_is_reported_when_type_arguments_of_type_argument_list_of_creation_are_on_multiple_lines() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, string> GetItems() => new Dictionary<
                                                            int,
                                                            string>(42);
}
");

        [Test]
        public void An_issue_is_reported_when_all_parts_of_type_argument_list_of_creation_are_on_multiple_lines() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, string> GetItems() => new Dictionary
                                                            <
                                                                int
                                                                   ,
                                                                       string
                                                                                >(42);
}
");

        [Test]
        public void Code_gets_fixed_when_opening_token_of_type_argument_list_of_creation_is_on_multiple_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<
                                            int>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<int>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_closing_token_of_type_argument_list_of_creation_is_on_multiple_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<int
                                               >(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<int>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_opening_token_and_closing_token_of_type_argument_list_of_creation_is_on_multiple_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<
                                            int
                                               >(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<int>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_type_arguments_of_type_argument_list_of_creation_are_on_multiple_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, string> GetItems() => new Dictionary<
                                                            int,
                                                            string>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, string> GetItems() => new Dictionary<int, string>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_all_parts_of_type_argument_list_of_creation_are_on_multiple_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, string> GetItems() => new Dictionary
                                                            <
                                                                int
                                                                   ,
                                                                       string
                                                                                >(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, string> GetItems() => new Dictionary<int, string>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_part_of_type_argument_list_is_array_and_all_on_multiple_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, string[]> GetItems() => new Dictionary
                                                            <
                                                                int
                                                                   ,
                                                                       string
                                                                              [
                                                                                ]
                                                                                  >(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, string[]> GetItems() => new Dictionary<int, string[]>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_nested_type_argument_list_is_on_separate_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, List<string>> GetItems() => new Dictionary<
                                                                int,
                                                                List<string>>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, List<string>> GetItems() => new Dictionary<int, List<string>>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_nested_type_argument_list_has_parts_on_separate_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, List<string>> GetItems() => new Dictionary<int, List
                                                                                <
                                                                                    string
                                                                                          >>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, List<string>> GetItems() => new Dictionary<int, List<string>>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_outer_and_nested_type_argument_lists_are_on_separate_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, List<string>> GetItems() => new Dictionary
                                                                <
                                                                    int
                                                                       ,
                                                                           List
                                                                               <
                                                                                string
                                                                                      >
                                                                                       >(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public Dictionary<int, List<string>> GetItems() => new Dictionary<int, List<string>>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_qualified_name_type_argument_is_on_multiple_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<System.String> GetItems() => new List<System.
                                                            String>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<System.String> GetItems() => new List<System.String>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_tuple_type_argument_is_on_multiple_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<(int, string)> GetItems() => new List<(int,
                                                       string)>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<(int, string)> GetItems() => new List<(int, string)>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_nullable_type_argument_is_on_multiple_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int?> GetItems() => new List<int
                                                ?>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int?> GetItems() => new List<int?>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test] // intended behavior to remove comments
        public void Code_gets_fixed_with_losing_comment_inside_type_argument_list()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<
                                            /* comment */ int>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<int>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test] // intended behavior to remove comments
        public void Code_gets_fixed_with_losing_comment_when_type_argument_list_contains_single_line_comment_before_type()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<
                                            // comment
                                            int>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<int>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test] // intended behavior to remove comments
        public void Code_gets_fixed_with_losing_comment_when_type_argument_list_contains_single_line_comment_after_type()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<int // comment
                                            >(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<int> GetItems() => new List<int>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_named_tuple_type_argument_is_on_multiple_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<(int count, string text)> GetItems() => new List<(int count,
                                                                  string text)>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<(int count, string text)> GetItems() => new List<(int count, string text)>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_named_tuple_element_type_and_name_are_on_separate_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<(int count, string text)> GetItems() => new List<(int
                                                                  count, string
                                                                                text)>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<(int count, string text)> GetItems() => new List<(int count, string text)>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_alias_qualified_name_type_argument_is_on_multiple_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<global::System.String> GetItems() => new List<global::
                                                                      System.String>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<global::System.String> GetItems() => new List<global::System.String>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_alias_qualified_name_parts_are_on_separate_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<global::System.String> GetItems() => new List<global
                                                                    ::
                                                                      System.String>(42);
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public List<global::System.String> GetItems() => new List<global::System.String>(42);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_without_breaking_directive_directly_after_type_argument_list()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    private List<
                 int>
#if true
    _field;
#else
    _other;
#endif
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    private List<int>
#if true
    _field;
#else
    _other;
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_is_not_changed_when_type_argument_list_contains_preprocessor_directives()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public object GetItems() => new Dictionary<
#if NETSTANDARD
                                                int,
#else
                                                long,
#endif
                                                string>(42);
}
";

            VerifyCSharpFix(OriginalCode, OriginalCode);
        }

        protected override string GetDiagnosticId() => MiKo_6076_TypeArgumentListsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6076_TypeArgumentListsAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6076_CodeFixProvider();
    }
}