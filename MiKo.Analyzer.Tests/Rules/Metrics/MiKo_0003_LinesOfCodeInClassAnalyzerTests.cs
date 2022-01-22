using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

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
        public void Generated_type_with_length_above_limit_is_not_reported() => No_issue_is_reported_for(@"

    [System.Rutime.CompilerServices.CompilerGeneratedAttribute]
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
        public void Type_with_length_above_limit_is_reported() => An_issue_is_reported_for(@"

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

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_0003_LinesOfCodeInClassAnalyzer { MaxLinesOfCode = 3 };

        protected override string GetDiagnosticId() => MiKo_0003_LinesOfCodeInClassAnalyzer.Id;
    }
}
