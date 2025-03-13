using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6062_ComplexInitializerElementExpressionsAreIndentedBesideOpenBraceAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_complex_initializer_element_expression_on_same_line() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private Dictionary<int, int> Values = new Dictionary<int, int>
                                              {
                                                { 42, 11 },
                                              };
}
");

        [Test]
        public void No_issue_is_reported_for_complex_initializer_element_expression_on_other_line_but_indented() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private Dictionary<int, int> Values = new Dictionary<int, int>
                                              {
                                                {
                                                  42, 11 },
                                              };
}
");

        [Test]
        public void An_issue_is_reported_for_complex_initializer_element_expression_on_other_line_but_more_indented() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private Dictionary<int, int> Values = new Dictionary<int, int>
                                              {
                                                {
                                                   42, 11 },
                                              };
}
");

        [Test]
        public void An_issue_is_reported_for_complex_initializer_element_expression_on_other_line_but_outdented() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private Dictionary<int, int> Values = new Dictionary<int, int>
                                              {
                                                {
                      42, 11 },
                                              };
}
");

        [Test]
        public void Code_gets_fixed_for_complex_initializer_element_expression_on_other_line_but_outdented()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private Dictionary<int, int> Values = new Dictionary<int, int>
                                              {
                                                {
                      42, 11 },
                                              };
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private Dictionary<int, int> Values = new Dictionary<int, int>
                                              {
                                                {
                                                  42, 11 },
                                              };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_initializer_element_expression_on_other_line_but_on_same_position_as_open_brace()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private Dictionary<int, int> Values = new Dictionary<int, int>
                                              {
                                                {
                                                42, 11 },
                                              };
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private Dictionary<int, int> Values = new Dictionary<int, int>
                                              {
                                                {
                                                  42, 11 },
                                              };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_initializer_element_expression_on_other_line_but_on_position_1_character_behind_open_brace()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private Dictionary<int, int> Values = new Dictionary<int, int>
                                              {
                                                {
                                                 42, 11 },
                                              };
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private Dictionary<int, int> Values = new Dictionary<int, int>
                                              {
                                                {
                                                  42, 11 },
                                              };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6062_ComplexInitializerElementExpressionsAreIndentedBesideOpenBraceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6062_ComplexInitializerElementExpressionsAreIndentedBesideOpenBraceAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6062_CodeFixProvider();
    }
}