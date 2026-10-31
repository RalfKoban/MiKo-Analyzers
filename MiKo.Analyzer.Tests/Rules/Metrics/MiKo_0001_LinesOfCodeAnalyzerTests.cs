using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [TestFixture]
    public sealed class MiKo_0001_LinesOfCodeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_file_with_fewer_lines_of_code() => No_issue_is_reported_for(@"

namespace MiKoSolutions.Analyzers.Rules.Metrics.LoCValidTestFiles
{
    public class EmptyType
    {
    }

    public class TypeWithCtor
    {
        public TypeWithCtor()
        {
        }
    }

    public class TypeWithMethod
    {
        public void Method()
        {
        }
    }

    public class TypeWithProperties
    {
        public string SingleLineProperty
        {
            get { return string.Empty; }
            set { }
        }

        public string BracketOnSameLineProperty
        {
            get {
                return string.Empty;
            }
            set {
            }
        }

        public string BracketOnOtherLineProperty
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_local_function_with_long_if_statement() => No_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            void LocalFunction()
            {
                if (true)
                {
                    var x = 0;
                    if (x == 0)
                    {
                        return;
                    }
                }
            }
        }
    }
");

        [Test]
        public void No_issue_is_reported_for_method_with_exactly_matching_ObjectInitializer_statement() => No_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public DTO Create()
        {
            var data = new DTO
                           {
                               NodesToInsert = 4,
                           };
            return data;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_exactly_matching_ObjectInitializer_statement_as_return_statement() => No_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public DTO Create()
        {
            return new DTO
                       {
                           NodesToInsert = 4,
                           TargetNodes = 5,
                       };
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_short_do_while_statement() => No_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            do
            {
            }
            while (true);
        }
    }
");

        [Test]
        public void No_issue_is_reported_for_method_with_short_for_statement() => No_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            for (var i = 0; i < 10; i++)
            {
            }
        }
    }
");

        [Test]
        public void No_issue_is_reported_for_method_with_short_foreach_statement() => No_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method(int[] values)
        {
            foreach (var value in values)
            {
            }
        }
    }
");

        [Test]
        public void No_issue_is_reported_for_method_with_short_foreach_variable_statement() => No_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method((int, int)[] values)
        {
            foreach (var (a, b) in values)
            {
            }
        }
    }
");

        [Test]
        public void No_issue_is_reported_for_method_with_short_while_statement() => No_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            while (true)
            {
            }
        }
    }
");

        [Test]
        public void An_issue_is_reported_for_method_with_long_catch_filter_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            try { }
            catch (Exception ex) when (ex != null)
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            }
        }
    }
");

        [Test]
        public void An_issue_is_reported_for_method_with_long_catch_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            try { }
            catch (Exception ex)
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            }
        }
    }
");

        [Test]
        public void An_issue_is_reported_for_method_with_long_do_while_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            do
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            }
            while (true);
        }
    }
");

        [Test]
        public void An_issue_is_reported_for_method_with_long_finally_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            try { }
            finally
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            }
        }
    }
");

        [Test]
        public void An_issue_is_reported_for_method_with_long_for_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            for (var i = 0; i < 10; i++)
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            }
        }
    }
");

        [Test]
        public void An_issue_is_reported_for_method_with_long_foreach_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method(int[] values)
        {
            foreach (var value in values)
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            }
        }
    }
");

        [Test]
        public void An_issue_is_reported_for_method_with_long_foreach_variable_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method((int, int)[] values)
        {
            foreach (var (a, b) in values)
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            }
        }
    }
");

        [Test]
        public void An_issue_is_reported_for_method_with_long_if_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            if (true)
            {
                var x = 0;
                if (x == 0)
                {
                    return;
                }
            }
        }
    }
");

        [Test]
        public void An_issue_is_reported_for_method_with_long_ObjectInitializer_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public DTO Create()
        {
            var data = new DTO
                           {
                               NodesToInsert = 4,
                               TargetNodes = 5,
                           };
            return data;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_long_switch_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            switch (x)
            {
                case 1:
                    var x = 0;
                    break;
            }
        }
    }
");

        [Test]
        public void An_issue_is_reported_for_method_with_long_try_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            try
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            } finally { }
        }
    }
");

        [Test]
        public void An_issue_is_reported_for_method_with_long_while_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public void Method()
        {
            while (true)
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            }
        }
    }
");

        [Test]
        public void An_issue_is_reported_for_method_with_too_long_returning_ObjectInitializer_statement() => An_issue_is_reported_for(@"

    public class TypeWithMethod
    {
        public DTO Create()
        {
            return new DTO
                       {
                           NodesToInsert = 4,
                           TargetNodes = 5,
                           TargetNodes2 = 56,
                       };
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_0001_LinesOfCodeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_0001_LinesOfCodeAnalyzer { MaxLinesOfCode = 3 };
    }
}
