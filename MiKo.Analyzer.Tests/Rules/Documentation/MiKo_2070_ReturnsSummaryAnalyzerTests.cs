using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2070_ReturnsSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_items() => No_issue_is_reported_for(@"
public class TestMe
{
    public int SomethingProperty { get; set; }

    public int DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_items() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public int SomethingProperty { get; set; }

    /// <summary>
    /// Does something.
    /// </summary>
    public int DoSomething()
    {
    }

    /// <summary>
    /// Gets something.
    /// </summary>
    public int DoSomethingElse()
    {
    }

    /// <summary>
    /// Determines something.
    /// </summary>
    public bool DoSomethingDifferent()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_([Values("Return", "Returns", "return", "returns")] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @" something.
    /// </summary>
    public int DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_property_([Values("Return", "Returns", "return", "returns")] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @" something.
    /// </summary>
    public int SomethingProperty { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_ToString() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString()
    {
        return ""Bla"";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_GetEnumerator() => No_issue_is_reported_for(@"
using System.Collections;

public class TestMe : IEnumerable
{
    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    public IEnumerator GetEnumerator()
    {
        return null;
    }
}
");

        [Test]
        public void Code_gets_fixed_for_non_boolean_property_summary()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Returns something.
    /// </summary>
    public object DoSomething => new object();
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Gets something.
    /// </summary>
    public object DoSomething => new object();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("Return")]
        [TestCase("Returns")]
        [TestCase("Returns true if")]
        [TestCase(@"Returns <see langword=""true""/> if")]
        [TestCase(@"Returns <see langword=""true""/> whether")]
        [TestCase(@"Returns <see langref=""true""/> if")]
        [TestCase(@"Returns <see langref=""true""/> whether")]
        public void Code_gets_fixed_for_boolean_property_summary_(string startPhrase)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// " + startPhrase + @" something.
    /// </summary>
    public bool DoSomething => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Gets a value indicating whether something.
    /// </summary>
    public bool DoSomething => true;
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase("Return")]
        [TestCase("Returns")]
        [TestCase(@"Returns true if")]
        [TestCase(@"Returns true whether")]
        [TestCase(@"Returns <b>true</b> if")]
        [TestCase(@"Returns <c>true</c> if")]
        [TestCase(@"Returns <value>true</value> if")]
        [TestCase(@"Returns <see langword=""true""/> if")]
        [TestCase(@"Returns <see langword=""true""/> whether")]
        [TestCase(@"Returns <see langref=""true""/> if")]
        [TestCase(@"Returns <see langref=""true""/> whether")]
        public void Code_gets_fixed_for_boolean_method_summary_(string startPhrase)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// " + startPhrase + @" something.
    /// </summary>
    public bool DoSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether something.
    /// </summary>
    public bool DoSomething() => true;
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase(@"Returns false if")]
        [TestCase(@"Returns false whether")]
        [TestCase(@"Returns <see langword=""false""/> if")]
        [TestCase(@"Returns <see langword=""false""/> whether")]
        [TestCase(@"Returns <see langref=""false""/> if")]
        [TestCase(@"Returns <see langref=""false""/> whether")]
        public void Code_gets_not_fixed_for_boolean_method_summary_starting_with_false_(string startPhrase)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// " + startPhrase + @" something.
    /// </summary>
    public bool DoSomething() => true;
}
";

            VerifyCSharpFix(originalCode, originalCode);
        }

        [TestCase("Asynchronously returns")]
        [TestCase("Asynchronously returns true if")]
        [TestCase("Asynchronously returns true whether")]
        [TestCase(@"Asynchronously returns <see langword=""true""/> if")]
        [TestCase(@"Asynchronously returns <see langword=""true""/> whether")]
        [TestCase(@"Asynchronously returns <see langref=""true""/> if")]
        [TestCase(@"Asynchronously returns <see langref=""true""/> whether")]
        [TestCase(@"Asynchronously returns <b>true</b> whether")]
        [TestCase(@"Asynchronously returns <c>true</c> whether")]
        [TestCase(@"Asynchronously returns <value>true</value> whether")]
        [TestCase("Returns")]
        [TestCase(@"Returns true if")]
        [TestCase(@"Returns true whether")]
        [TestCase(@"Returns <see langword=""true""/> if")]
        [TestCase(@"Returns <see langword=""true""/> whether")]
        [TestCase(@"Returns <see langref=""true""/> if")]
        [TestCase(@"Returns <see langref=""true""/> whether")]
        public void Code_gets_fixed_for_boolean_async_method_summary_(string startPhrase)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// " + startPhrase + @" something.
    /// </summary>
    public async bool DoSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Asynchronously determines whether something.
    /// </summary>
    public async bool DoSomething() => true;
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_boolean_Task_method_summary()
        {
            const string OriginalCode = @"
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Returns something.
    /// </summary>
    public Task<bool> DoSomething() => Task.FromResult(true);
}
";

            const string FixedCode = @"
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Asynchronously determines whether something.
    /// </summary>
    public Task<bool> DoSomething() => Task.FromResult(true);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_non_boolean_method_summary()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Returns something.
    /// </summary>
    public object DoSomething() => new object();
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Gets something.
    /// </summary>
    public object DoSomething() => new object();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_non_boolean_async_method_summary()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Asynchronously returns something.
    /// </summary>
    public async object DoSomething() => new object();
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Asynchronously gets something.
    /// </summary>
    public async object DoSomething() => new object();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_non_boolean_Task_method_summary()
        {
            const string OriginalCode = @"
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Returns something.
    /// </summary>
    public Task<object> DoSomething() => new object();
}
";

            const string FixedCode = @"
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Asynchronously gets something.
    /// </summary>
    public Task<object> DoSomething() => new object();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_and_space_before_link_is_kept()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Returns something is based on <see cref=""TestMe""/> to see if it is working.
    /// </summary>
    public bool DoSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether something is based on <see cref=""TestMe""/> to see if it is working.
    /// </summary>
    public bool DoSomething() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("; false otherwise")]
        [TestCase("; otherwise false")]
        [TestCase("; otherwise; false")]
        [TestCase("; otherwise, false")]
        [TestCase(", false otherwise")]
        [TestCase(", otherwise false")]
        [TestCase(", otherwise, false")]
        [TestCase(", otherwise; false")]
        [TestCase(@"; <see langword=""false""/> otherwise")]
        [TestCase(@", <see langword=""false""/> otherwise")]
        [TestCase(@"; otherwise; <see langword=""false""/>")]
        [TestCase(@"; otherwise, <see langword=""false""/>")]
        [TestCase(@"; otherwise <see langword=""false""/>")]
        [TestCase(@", otherwise; <see langword=""false""/>")]
        [TestCase(@", otherwise, <see langword=""false""/>")]
        [TestCase(@", otherwise <see langword=""false""/>")]
        [TestCase(@"; otherwise; <see langref=""false""/>")]
        [TestCase(@"; otherwise, <see langref=""false""/>")]
        [TestCase(@"; otherwise <see langref=""false""/>")]
        [TestCase(@", otherwise; <see langref=""false""/>")]
        [TestCase(@", otherwise, <see langref=""false""/>")]
        [TestCase(@", otherwise <see langref=""false""/>")]
        public void Code_gets_fixed_for_ending_phrase_(string phrase)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// Returns something" + phrase + @".
    /// </summary>
    public bool DoSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether something.
    /// </summary>
    public bool DoSomething() => true;
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2070_ReturnsSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2070_ReturnsSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2070_CodeFixProvider();
    }
}