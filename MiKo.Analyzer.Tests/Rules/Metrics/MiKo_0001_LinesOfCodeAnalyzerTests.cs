using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [TestFixture]
    public sealed class MiKo_0001_LinesOfCodeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Valid_files_are_not_reported_as_warnings() => No_issue_is_reported_for(@"

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
        public void Method_with_long_if_statement_is_reported() => An_issue_is_reported_for(@"

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
        public void Method_with_long_switch_statement_is_reported() => An_issue_is_reported_for(@"
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
        public void Method_with_long_try_statement_is_reported() => An_issue_is_reported_for(@"
    public class TypeWithMethod
    {
        public void Method()
        {
            try
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            }
        }
    }
");

        [Test]
        public void Method_with_long_catch_statement_is_reported() => An_issue_is_reported_for(@"
    public class TypeWithMethod
    {
        public void Method()
        {
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
        public void Method_with_long_catch_filter_statement_is_reported() => An_issue_is_reported_for(@"
    public class TypeWithMethod
    {
        public void Method()
        {
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
        public void Method_with_long_finally_statement_is_reported() => An_issue_is_reported_for(@"
    public class TypeWithMethod
    {
        public void Method()
        {
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
        public void Method_with_too_long_returning_ObjectInitializer_statement_is_reported() => An_issue_is_reported_for(@"
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

        [Test]
        public void Method_with_exaclty_matching_ObjectInitializer_statement_as_return_statement_is_not_reported() => No_issue_is_reported_for(@"
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
        public void Method_with_long_ObjectInitializer_statement_is_reported() => An_issue_is_reported_for(@"
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
        public void Method_with_exaclty_matching_ObjectInitializer_statement_is_not_reported() => No_issue_is_reported_for(@"
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

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_0001_LinesOfCodeAnalyzer { MaxLinesOfCode = 3 };

        protected override string GetDiagnosticId() => MiKo_0001_LinesOfCodeAnalyzer.Id;
    }
}
