using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [TestFixture]
    public sealed class MiKo_0003_LinesOfCodeInClassAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Type_with_length_below_limit_is_not_reported() => No_issue_is_reported_for(@"

    public class MyType
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
");

        [Test]
        public void Fully_qualified_generated_type_with_length_above_limit_is_not_reported() => No_issue_is_reported_for(@"

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public class MyType
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
        public void Generated_type_with_length_above_limit_is_not_reported() => No_issue_is_reported_for(@"
using System.Runtime.CompilerServices; 

    [CompilerGenerated]
    public class MyType
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
        public void Fully_qualified_test_type_with_length_above_limit_is_not_reported() => No_issue_is_reported_for(@"

    [NUnit.Framework.TestFixtureAttribute]
    public class MyType
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
        public void Test_type_with_length_above_limit_is_not_reported() => No_issue_is_reported_for(@"
using NUnit.Framework;

    [TestFixture]
    public class MyType
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
        public void Type_with_ctor_length_above_limit_is_reported() => An_issue_is_reported_for(@"

    public class MyType
    {
        public MyType()
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
        public void Type_with_method_length_above_limit_is_reported() => An_issue_is_reported_for(@"

    public class MyType
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
        public void Type_with_property_length_above_limit_is_reported() => An_issue_is_reported_for(@"

    public class MyType
    {
        public int Property
        {
            get
            {
                if (true)
                {
                    var x = 0;
                    if (x == 0)
                    {
                        return x;
                    }
                }
            }
        }
    }
");

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_0003_LinesOfCodeInClassAnalyzer { MaxLinesOfCode = 3 };

        protected override string GetDiagnosticId() => MiKo_0003_LinesOfCodeInClassAnalyzer.Id;
    }
}
