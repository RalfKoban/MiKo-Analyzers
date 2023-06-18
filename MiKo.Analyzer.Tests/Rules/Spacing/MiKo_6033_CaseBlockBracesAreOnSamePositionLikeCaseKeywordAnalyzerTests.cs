using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6033_CaseBlockBracesAreOnSamePositionLikeCaseKeywordAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_switch_section_with_case_and_return_on_same_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        switch (i)
        {
            case 1: return i;
            case 2: return i;
            default:
                return i;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_section_with_case_and_return_on_same_lines_with_braces() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        switch (i)
        {
            case 1: { return i; }
            case 2: { return i; }
            default:
                return i;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_section_with_case_and_return_on_different_lines_but_no_braces() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        switch (i)
        {
            case 1:
                return i;

            case 2:
                return i;

            default:
                return i;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_section_with_case_and_return_on_different_lines_and_braces_below_case() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        switch (i)
        {
            case 1:
            {
                return i;
            }

            case 2:
            {
                return i;
            }

            default:
            {
                return i;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_section_with_case_and_return_on_different_lines_and_braces_indented_to_case() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        switch (i)
        {
            case 1:
                {
                    return i;
                }

            default:
            {
                return i;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_section_with_case_and_return_on_different_lines_and_braces_indented_to_default() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        switch (i)
        {
            case 1:
            {
                return i;
            }

            default:
                {
                    return i;
                }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_indented_block_below_case()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        switch (i)
        {
            case 1:
                {
                    return i;
                }

            default: return i;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        switch (i)
        {
            case 1:
            {
                return i;
            }

            default: return i;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_indented_block_below_defaul()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        switch (i)
        {
            case 1: return i;

            default:
                {
                    return i;
                }
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        switch (i)
        {
            case 1: return i;

            default:
            {
                return i;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6033_CaseBlockBracesAreOnSamePositionLikeCaseKeywordAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6033_CaseBlockBracesAreOnSamePositionLikeCaseKeywordAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6033_CodeFixProvider();
    }
}