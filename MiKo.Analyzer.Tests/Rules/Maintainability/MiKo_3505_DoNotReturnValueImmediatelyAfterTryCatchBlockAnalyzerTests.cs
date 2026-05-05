using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3505_DoNotReturnValueImmediatelyAfterTryCatchBlockAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_when_returning_a_value_without_any_try_catch_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_without_any_try_catch_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_inside_try_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return 42;
        }
        catch
        {
           throw;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_inside_try_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return StringComparison.Ordinal;
        }
        catch
        {
           throw;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_and_the_catch_block_swallows() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_and_the_catch_block_swallows() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch
        {
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_and_the_catch_block_partly_falls_through() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch
        {
            if (o == null)
            {
                return -1;
            }
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_and_the_catch_block_partly_falls_through() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch
        {
            if (o == null)
            {
                return StringComparison.OrdinalIgnoreCase;
            }
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_all_catch_blocks_swallow() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
        }
        catch
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_all_catch_blocks_swallow() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
        }
        catch
        {
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_the_catch_blocks_partly_fall_through() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            if (o == null)
            {
                return -1;
            }
        }
        catch
        {
            if (o == null)
            {
                return -2;
            }
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_the_catch_blocks_partly_fall_through() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            if (o == null)
            {
                return StringComparison.OrdinalIgnoreCase;
            }
        }
        catch
        {
            if (o == null)
            {
                return StringComparison.CurrentCulture;
            }
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_a_single_catch_block_partly_falls_through() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            if (o == null)
            {
                return -1;
            }
        }
        catch
        {
            return -2;
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_a_single_catch_block_partly_falls_through() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            if (o == null)
            {
                return StringComparison.OrdinalIgnoreCase;
            }
        }
        catch
        {
            return StringComparison.CurrentCulture;
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_a_single_catch_block_throws() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_a_single_catch_block_throws() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch
        {
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_a_single_catch_block_returns() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            return -1;
        }
        catch
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_a_single_catch_block_returns() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            return StringComparison.OrdinalIgnoreCase;
        }
        catch
        {
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_and_the_catch_block_with_when_filter_swallows() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_and_the_catch_block_with_when_filter_swallows() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_and_the_catch_block_with_when_filter_partly_falls_through() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            if (o == null)
            {
                return -1;
            }
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_and_the_catch_block_with_when_filter_partly_falls_through() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            if (o == null)
            {
                return StringComparison.OrdinalIgnoreCase;
            }
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_all_catch_blocks_swallow() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_all_catch_blocks_swallow() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_the_catch_blocks_partly_fall_through() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            if (o == null)
            {
                return -1;
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            if (o == null)
            {
                return -2;
            }
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_the_catch_blocks_partly_fall_through() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            if (o == null)
            {
                return StringComparison.OrdinalIgnoreCase;
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            if (o == null)
            {
                return StringComparison.CurrentCulture;
            }
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_a_single_catch_block_partly_falls_through() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            if (o == null)
            {
                return -1;
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return -2;
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_a_single_catch_block_partly_falls_through() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            if (o == null)
            {
                return StringComparison.OrdinalIgnoreCase;
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return StringComparison.CurrentCulture;
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_a_single_catch_block_throws() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            throw;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_a_single_catch_block_throws() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            throw;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_a_single_catch_block_returns() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            return -1;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_a_single_catch_block_returns() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            return StringComparison.OrdinalIgnoreCase;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_value_inside_try_block_of_try_finally_block_without_catch_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return 42;
        }
        finally
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_an_enum_value_inside_try_block_of_try_finally_block_without_catch_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return StringComparison.Ordinal;
        }
        finally
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_and_the_catch_block_throws() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch
        {
            throw;
        }

        return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_and_the_catch_block_throws() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch
        {
            throw;
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_and_the_catch_block_returns() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch
        {
            return StringComparison.OrdinalIgnoreCase;
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_and_the_catch_block_returns() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch
        {
            return -1;
        }

        return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_all_catch_blocks_throw() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch
        {
            throw;
        }

        return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_all_catch_blocks_throw() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch
        {
            throw;
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_all_catch_blocks_return() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            return -1;
        }
        catch
        {
            return -2;
        }

        return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_all_catch_blocks_return() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            return StringComparison.OrdinalIgnoreCase;
        }
        catch
        {
            return StringComparison.CurrentCulture;
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_and_the_catch_block_with_when_filter_throws() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            throw;
        }

        return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_and_the_catch_block_with_when_filter_throws() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            throw;
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_and_the_catch_block_with_when_filter_returns() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            return -1;
        }

        return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_and_the_catch_block_with_when_filter_returns() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            return StringComparison.OrdinalIgnoreCase;
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_all_catch_blocks_throw() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            throw;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            throw;
        }

        return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_all_catch_blocks_throw() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            throw;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            throw;
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_all_catch_blocks_return() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            return -1;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return -2;
        }

        return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_all_catch_blocks_return() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            return StringComparison.OrdinalIgnoreCase;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return StringComparison.CurrentCulture;
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_a_value_directly_after_try_finally_block_without_catch_block() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        finally
        {
        }

        return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_an_enum_value_directly_after_try_finally_block_without_catch_block() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        finally
        {
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_a_value_directly_after_try_finally_block_without_catch_block_and_finally_block_has_statements() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        finally
        {
            DoSomething(null);
        }

        return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_an_enum_value_directly_after_try_finally_block_without_catch_block_and_finally_block_has_statements() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        finally
        {
            DoSomething(null);
        }

        return StringComparison.Ordinal;
    }
}
");

        [Test]
        public void Code_gets_fixed_when_returning_a_value_directly_after_try_catch_block_and_the_catch_block_throws()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch
        {
            throw;
        }

        return 42;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return 42;
        }
        catch
        {
            throw;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_an_enum_value_directly_after_try_catch_block_and_the_catch_block_throws()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch
        {
            throw;
        }

        return StringComparison.Ordinal;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return StringComparison.Ordinal;
        }
        catch
        {
            throw;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_a_value_directly_after_try_catch_block_and_the_catch_block_returns()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch
        {
            return -1;
        }

        return 42;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return 42;
        }
        catch
        {
            return -1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_an_enum_value_directly_after_try_catch_block_and_the_catch_block_returns()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch
        {
            return StringComparison.OrdinalIgnoreCase;
        }

        return StringComparison.Ordinal;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return StringComparison.Ordinal;
        }
        catch
        {
            return StringComparison.OrdinalIgnoreCase;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_all_catch_blocks_throw()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch
        {
            throw;
        }

        return 42;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return 42;
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch
        {
            throw;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_all_catch_blocks_throw()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch
        {
            throw;
        }

        return StringComparison.Ordinal;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return StringComparison.Ordinal;
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch
        {
            throw;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_all_catch_blocks_return()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            return -1;
        }
        catch
        {
            return -2;
        }

        return 42;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return 42;
        }
        catch (ArgumentNullException)
        {
            return -1;
        }
        catch
        {
            return -2;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_and_all_catch_blocks_return()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException)
        {
            return StringComparison.OrdinalIgnoreCase;
        }
        catch
        {
            return StringComparison.CurrentCulture;
        }

        return StringComparison.Ordinal;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return StringComparison.Ordinal;
        }
        catch (ArgumentNullException)
        {
            return StringComparison.OrdinalIgnoreCase;
        }
        catch
        {
            return StringComparison.CurrentCulture;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_a_value_directly_after_try_catch_block_and_the_catch_block_with_when_filter_throws()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            throw;
        }

        return 42;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return 42;
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            throw;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_an_enum_value_directly_after_try_catch_block_and_the_catch_block_with_when_filter_throws()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            throw;
        }

        return StringComparison.Ordinal;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return StringComparison.Ordinal;
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            throw;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_a_value_directly_after_try_catch_block_and_the_catch_block_with_when_filter_returns()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            return -1;
        }

        return 42;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return 42;
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            return -1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_an_enum_value_directly_after_try_catch_block_and_the_catch_block_with_when_filter_returns()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            return StringComparison.OrdinalIgnoreCase;
        }

        return StringComparison.Ordinal;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return StringComparison.Ordinal;
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            return StringComparison.OrdinalIgnoreCase;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_all_catch_blocks_throw()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            throw;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            throw;
        }

        return 42;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return 42;
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            throw;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            throw;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_all_catch_blocks_throw()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            throw;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            throw;
        }

        return StringComparison.Ordinal;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return StringComparison.Ordinal;
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            throw;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            throw;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_a_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_all_catch_blocks_return()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            return -1;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return -2;
        }

        return 42;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return 42;
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            return -1;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return -2;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_an_enum_value_directly_after_try_catch_block_with_multiple_catch_blocks_with_when_filters_and_all_catch_blocks_return()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            return StringComparison.OrdinalIgnoreCase;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return StringComparison.CurrentCulture;
        }

        return StringComparison.Ordinal;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return StringComparison.Ordinal;
        }
        catch (ArgumentNullException ex) when (ex.Message != null)
        {
            return StringComparison.OrdinalIgnoreCase;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return StringComparison.CurrentCulture;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_a_value_directly_after_try_finally_block_without_catch_block()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        finally
        {
        }

        return 42;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return 42;
        }
        finally
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_an_enum_value_directly_after_try_finally_block_without_catch_block()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        finally
        {
        }

        return StringComparison.Ordinal;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return StringComparison.Ordinal;
        }
        finally
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_a_value_directly_after_try_finally_block_without_catch_block_and_finally_block_has_statements()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        finally
        {
            DoSomething(null);
        }

        return 42;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return 42;
        }
        finally
        {
            DoSomething(null);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_an_enum_value_directly_after_try_finally_block_without_catch_block_and_finally_block_has_statements()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);
        }
        finally
        {
            DoSomething(null);
        }

        return StringComparison.Ordinal;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public StringComparison DoSomething(object o)
    {
        try
        {
            DoSomething(null);

            return StringComparison.Ordinal;
        }
        finally
        {
            DoSomething(null);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3505_DoNotReturnValueImmediatelyAfterTryCatchBlockAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3505_DoNotReturnValueImmediatelyAfterTryCatchBlockAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3505_CodeFixProvider();
    }
}