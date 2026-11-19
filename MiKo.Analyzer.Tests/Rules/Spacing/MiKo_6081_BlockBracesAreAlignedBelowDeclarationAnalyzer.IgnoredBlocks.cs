using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_nested_block_with_open_brace_indented_more_to_left() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        {
        }

      {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nested_block_with_open_brace_indented_more_to_right() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        {
        }

          {
        }
    }
}
");

        // gets reported by MiKo_6063
        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_block_with_open_brace_indented_more_to_left() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Action action = () =>
       {
        };
    }
}
");

        // gets reported by MiKo_6063
        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_block_with_open_brace_indented_more_to_right() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Action action = () =>
         {
        };
    }
}
");

        // gets reported by MiKo_6063
        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_block_with_open_brace_on_same_line_as_declaration() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Action action = () => {
        };
    }
}
");

        // gets reported by MiKo_6063
        [Test]
        public void No_issue_is_reported_for_simple_lambda_block_with_open_brace_indented_more_to_left() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Action<int> action = i =>
       {
        };
    }
}
");

        // gets reported by MiKo_6063
        [Test]
        public void No_issue_is_reported_for_simple_lambda_block_with_open_brace_indented_more_to_right() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Action<int> action = i =>
         {
        };
    }
}
");

        // gets reported by MiKo_6033
        [Test]
        public void No_issue_is_reported_for_switch_section_block_with_open_brace_indented_more_to_left() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i)
        {
            case 1:
          {
                break;
            }
        }
    }
}
");

        // gets reported by MiKo_6033
        [Test]
        public void No_issue_is_reported_for_switch_section_block_with_open_brace_indented_more_to_right() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i)
        {
            case 1:
               {
                break;
            }
        }
    }
}
");

        // gets reported by MiKo_6033
        [Test]
        public void No_issue_is_reported_for_switch_section_block_with_open_brace_on_same_line_as_declaration() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i)
        {
            case 1: {
                break;
            }
        }
    }
}
");

        // gets reported by MiKo_6079
        [Test]
        public void No_issue_is_reported_for_anonymous_method_block_with_open_brace_indented_more_to_left() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Action action = delegate()
       {
        };
    }
}
");

        // gets reported by MiKo_6079
        [Test]
        public void No_issue_is_reported_for_anonymous_method_block_with_open_brace_indented_more_to_right() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Action action = delegate()
         {
        };
    }
}
");

        // gets reported by MiKo_6079
        [Test]
        public void No_issue_is_reported_for_anonymous_method_block_with_open_brace_on_same_line_as_declaration() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Action action = delegate() {
        };
    }
}
");
    }
}
