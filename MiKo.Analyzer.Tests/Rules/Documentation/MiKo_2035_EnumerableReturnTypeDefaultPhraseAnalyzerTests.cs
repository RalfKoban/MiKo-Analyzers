using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] EnumerableOnlyReturnValues =
            {
                "IEnumerable",
                "IEnumerable<int>",
                "IList<int>",
                "ICollection<int>",
                "List<int>",
                "Dictionary<int, int>",
            };

        private static readonly string[] EnumerableTaskReturnValues =
            {
                "Task<int[]>",
                "Task<IEnumerable>",
                "Task<List<int>>",
            };

        private static readonly string[] EnumerableReturnValues = EnumerableOnlyReturnValues.Concat(EnumerableTaskReturnValues).ToArray();

        [Test]
        public void No_issue_is_reported_for_uncommented_method_([ValueSource(nameof(EnumerableReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property_([ValueSource(nameof(EnumerableReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_method_that_returns_a_(
                                                                [Values("returns", "value")] string xmlTag,
                                                                [Values("void", "int", "string", "Task", "Task<int>", "Task<bool>, Task<string>")] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// Something.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => throw new NotSupportedException();
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Array_only_method_(
                                                                                [Values("returns", "value")] string xmlTag,
                                                                                [Values("int", "string")] string returnType,
                                                                                [Values("An array of", "The array of")] string startingPhrase)
            => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + startingPhrase + @" whatever.
    /// </" + xmlTag + @">
    public " + returnType + @"[] DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_Byte_array_only_method()
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A byte array containing whatever.
    /// </returns>
    public byte[] DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Enumerable_only_method_(
                                                                                    [Values("returns", "value")] string xmlTag,
                                                                                    [ValueSource(nameof(EnumerableOnlyReturnValues))] string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A collection of whatever.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_List_method_(
                                                                        [Values("returns", "value")] string xmlTag,
                                                                        [Values("A", "An")] string startingWord) => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + startingWord + @" <see cref=""List{T}"" /> that contains something.
    /// </" + xmlTag + @">
    public List<int> DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Enumerable_Task_method_(
                                                                                    [Values("returns", "value")] string xmlTag,
                                                                                    [Values("", " ")] string space,
                                                                                    [ValueSource(nameof(EnumerableTaskReturnValues))] string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A task that represents the asynchronous operation. The value of the <see cref=""System.Threading.Tasks.Task{TResult}.Result" + space + @"/> parameter contains a collection of whatever.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_wrong_commented_method_(
                                                                [Values("returns", "value")] string xmlTag,
                                                                [Values("A whatever", "An whatever", "The whatever")] string comment,
                                                                [ValueSource(nameof(EnumerableReturnValues))] string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + comment + @"
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void Code_gets_fixed_for_array_type()
        {
            const string OriginalText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// Some integers.
    /// </returns>
    public int[] DoSomething { get; set; }
}
";

            const string FixedText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// An array of some integers.
    /// </returns>
    public int[] DoSomething { get; set; }
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_byte_array_type()
        {
            const string OriginalText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// Some data.
    /// </returns>
    public byte[] DoSomething { get; set; }
}
";

            const string FixedText = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A byte array containing some data.
    /// </returns>
    public byte[] DoSomething { get; set; }
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [TestCase("Some integers.", "A collection of some integers.")]
        [TestCase("An enumerable of some integers.", "A collection of some integers.")]
        [TestCase("A list of some integers.", "A collection of some integers.")]
        public void Code_gets_fixed_for_non_generic_collection_(string originalPhrase, string fixedPhrase)
        {
            const string Template = @"
using System;
using System.Collections;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// ###
    /// </returns>
    public IList DoSomething { get; set; }
}
";

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", fixedPhrase));
        }

        [TestCase("Some integers.", "A collection of some integers.")]
        [TestCase("An enumerable of some integers.", "A collection of some integers.")]
        [TestCase("An enumerable with some integers.", "A collection of some integers.")]
        [TestCase("A list of some integers.", "A collection of some integers.")]
        [TestCase("A list with some integers.", "A collection of some integers.")]
        [TestCase("The enumerable of some integers.", "A collection of some integers.")]
        [TestCase("The enumerable with some integers.", "A collection of some integers.")]
        [TestCase("The list of some integers.", "A collection of some integers.")]
        [TestCase("The list with some integers.", "A collection of some integers.")]
        [TestCase("The collection of some integers.", "A collection of some integers.")]
        [TestCase("The collection with some integers.", "A collection of some integers.")]
        [TestCase("The array of some integers.", "A collection of some integers.")]
        [TestCase("The array with some integers.", "A collection of some integers.")]
        public void Code_gets_fixed_for_generic_collection_(string originalPhrase, string fixedPhrase)
        {
            const string Template = @"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// ###
    /// </returns>
    public IEnumerable<int> DoSomething { get; set; }
}
";

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", fixedPhrase));
        }

        [Test]
        public void Code_gets_fixed_for_Task_with_generic_collection()
        {
            const string OriginalText = @"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// Some integers.
    /// </returns>
    public Task<IList<int>> DoSomething { get; set; }
}
";

            const string FixedText = @"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter contains a collection of some integers.
    /// </returns>
    public Task<IList<int>> DoSomething { get; set; }
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_Task_with_array()
        {
            const string OriginalText = @"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// Some integers.
    /// </returns>
    public Task<int[]> DoSomething { get; set; }
}
";

            const string FixedText = @"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter contains an array of some integers.
    /// </returns>
    public Task<int[]> DoSomething { get; set; }
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_Task_with_byte_array()
        {
            const string OriginalText = @"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// Some data.
    /// </returns>
    public Task<byte[]> DoSomething { get; set; }
}
";

            const string FixedText = @"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter contains a byte array containing some data.
    /// </returns>
    public Task<byte[]> DoSomething { get; set; }
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        protected override string GetDiagnosticId() => MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2035_CodeFixProvider();
    }
}