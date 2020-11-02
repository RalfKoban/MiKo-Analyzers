using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2091_InequalityOperatorAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_normal_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething();
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_inequality_operator() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public static bool operator !=(TestMe left, TestMe right) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_equality_operator() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public static bool operator ==(TestMe left, TestMe right) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_documented_equality_operator() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Returns true if items are different.</summary>
    /// <param name=""left"">Left value.</param>
    /// <param name=""right"">Right value.</param>
    /// <returns>If items are different.</returns>
    public static bool operator ==(TestMe left, TestMe right) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_inequality_operator() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Determines whether the specified <see cref=""TestMe""/> instances are considered not equal.</summary>
    /// <param name=""left"">The first value to compare.</param>
    /// <param name=""right"">The second value to compare.</param>
    /// <returns><see langword=""true""/> if both instances are considered not equal; otherwise, <see langword=""false""/>.</returns>
    public static bool operator !=(TestMe left, TestMe right) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_inequality_operator_with_full_qualified_name() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Determines whether the specified <see cref=""Bla.TestMe""/> instances are considered not equal.</summary>
        /// <param name=""left"">The first value to compare.</param>
        /// <param name=""right"">The second value to compare.</param>
        /// <returns><see langword=""true""/> if both instances are considered not equal; otherwise, <see langword=""false""/>.</returns>
        public static bool operator !=(TestMe left, TestMe right) => false;
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Would look strange otherwise.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_inequality_operator_summary() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Returns true if items are not equal.</summary>
    /// <param name=""left"">The first value to compare.</param>
    /// <param name=""right"">The second value to compare.</param>
    /// <returns><see langword=""true""/> if both instances are considered not equal; otherwise, <see langword=""false""/>.</returns>
    public static bool operator !=(TestMe left, TestMe right) => false;
}
", 1);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Would look strange otherwise.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_inequality_operator_returnValue() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Determines whether the specified <see cref=""TestMe""/> instances are considered not equal.</summary>
    /// <param name=""left"">The first value to compare.</param>
    /// <param name=""right"">The second value to compare.</param>
    /// <returns>true if both are not equal.</returns>
    public static bool operator !=(TestMe left, TestMe right) => false;
}
", 1);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Would look strange otherwise.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_inequality_operator_parameter() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Determines whether the specified <see cref=""TestMe""/> instances are considered not equal.</summary>
    /// <param name=""left"">The left value.</param>
    /// <param name=""right"">The right one.</param>
    /// <returns><see langword=""true""/> if both instances are considered not equal; otherwise, <see langword=""false""/>.</returns>
    public static bool operator !=(TestMe left, TestMe right) => false;
}
", 2);

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>Whatever</summary>
    /// <param name=""left"">A.</param>
    /// <param name=""right"">B.</param>
    /// <returns>Result.</returns>
    public static bool operator !=(TestMe left, TestMe right) => false;
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Determines whether the specified <see cref=""TestMe""/> instances are considered not equal.
    /// </summary>
    /// <param name=""left"">
    /// The first value to compare.
    /// </param>
    /// <param name=""right"">
    /// The second value to compare.
    /// </param>
    /// <returns>
    /// <see langword=""true""/> if both instances are considered not equal; otherwise, <see langword=""false""/>.
    /// </returns>
    public static bool operator !=(TestMe left, TestMe right) => false;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2091_InequalityOperatorAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2091_InequalityOperatorAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2091_CodeFixProvider();
    }
}