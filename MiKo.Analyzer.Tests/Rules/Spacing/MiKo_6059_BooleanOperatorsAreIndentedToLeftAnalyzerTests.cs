using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6059_BooleanOperatorsAreIndentedToLeftAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Code_gets_fixed_when_operator_is_left_of_left_operand_for_where_clause()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public void DoSomething(IEnumerable<int> numbers)
    {
        var result = from number in numbers
                     where number > 0
                  && number != 5
                  && number < 10
                     select number;
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public void DoSomething(IEnumerable<int> numbers)
    {
        var result = from number in numbers
                     where number > 0
                        && number != 5
                        && number < 10
                     select number;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6059_BooleanOperatorsAreIndentedToLeftAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6059_BooleanOperatorsAreIndentedToLeftAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6059_CodeFixProvider();
    }
}