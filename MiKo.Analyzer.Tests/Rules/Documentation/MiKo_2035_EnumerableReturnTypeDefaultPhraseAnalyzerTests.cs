using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] EnumerableOnlyReturnValues =
                                                                      [
                                                                          "IEnumerable",
                                                                          "IEnumerable<int>",
                                                                          "IList<int>",
                                                                          "ICollection<int>",
                                                                          "List<int>",
                                                                          "Dictionary<int, int>",
                                                                      ];

        private static readonly string[] EnumerableTaskReturnValues =
                                                                      [
                                                                          "Task<int[]>",
                                                                          "Task<IEnumerable>",
                                                                          "Task<List<int>>",
                                                                      ];

        private static readonly string[] EnumerableReturnValues = [.. EnumerableOnlyReturnValues, .. EnumerableTaskReturnValues];

        private static readonly string[] StartingPhrases = [.. Enumerable.ToHashSet(CreateStartingPhrases().Take(TestLimit))];

        [OneTimeSetUp]
        public void PrepareTestEnvironment() => MiKo_2035_CodeFixProvider.LoadData();

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
            const string OriginalCode = @"
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

            const string FixedCode = @"
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

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("")]
        [TestCase("A array of byte containing")]
        [TestCase("A array of byte that contains")]
        [TestCase("A array of byte which contains")]
        [TestCase("A array of bytes containing")]
        [TestCase("A array of bytes that contains")]
        [TestCase("A array of bytes which contains")]
        [TestCase("An array of byte containing")]
        [TestCase("An array of byte that contains")]
        [TestCase("An array of byte which contains")]
        [TestCase("An array of bytes containing")]
        [TestCase("An array of bytes that contains")]
        [TestCase("An array of bytes which contains")]
        [TestCase("The array of byte containing")]
        [TestCase("The array of byte that contains")]
        [TestCase("The array of byte which contains")]
        [TestCase("The array of bytes containing")]
        [TestCase("The array of bytes that contains")]
        [TestCase("The array of bytes which contains")]
        [TestCase(@"A array of")]
        [TestCase(@"A array with")]
        [TestCase(@"An array of <see cref=""byte""/> containing")]
        [TestCase(@"An array of <see cref=""byte""/> that contains")]
        [TestCase(@"An array of <see cref=""byte""/> which contains")]
        [TestCase(@"An array of <see cref=""byte""/>s containing")]
        [TestCase(@"An array of <see cref=""byte""/>s that contains")]
        [TestCase(@"An array of <see cref=""byte""/>s which contains")]
        [TestCase(@"An array of")]
        [TestCase(@"An array with")]
        [TestCase(@"Array of")]
        [TestCase(@"Array with")]
        [TestCase(@"The array of <see cref=""byte""/> containing")]
        [TestCase(@"The array of <see cref=""byte""/> that contains")]
        [TestCase(@"The array of <see cref=""byte""/> which contains")]
        [TestCase(@"The array of <see cref=""byte""/>s containing")]
        [TestCase(@"The array of <see cref=""byte""/>s that contains")]
        [TestCase(@"The array of <see cref=""byte""/>s which contains")]
        [TestCase(@"The array of")]
        [TestCase(@"The array with")]
        public void Code_gets_fixed_for_byte_array_type_(string text)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// " + text + @" data.
    /// </returns>
    public byte[] DoSomething { get; set; }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A byte array containing data.
    /// </returns>
    public byte[] DoSomething { get; set; }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_non_generic_collection_([ValueSource(nameof(StartingPhrases))] string originalPhrase)
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
    /// ### some integers.
    /// </returns>
    public IEnumerable DoSomething { get; set; }
}
";

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", "A collection of"));
        }

        [TestCase("Some integers.", "A collection of some integers.")]
        [TestCase("The mapping information.", "A collection of the mapping information.")]
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
    public IEnumerable DoSomething { get; set; }
}
";

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", fixedPhrase));
        }

        [Test]
        public void Code_gets_fixed_for_generic_collection_([ValueSource(nameof(StartingPhrases))] string originalPhrase)
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
    /// ### some integers.
    /// </returns>
    public IEnumerable<int> DoSomething { get; set; }
}
";

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", "A collection of"));
        }

        [TestCase("Some integers.", "A collection of some integers.")]
        [TestCase("The mapping information.", "A collection of the mapping information.")]
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

        [TestCase("Some integers.", "some integers.")]
        [TestCase("A task that can be used to await.", "")]
        [TestCase("A task that can be used to await", "")]
        [TestCase("A task to await.", "")]
        [TestCase("A task to await", "")]
        [TestCase("An awaitable task.", "")]
        [TestCase("An awaitable task", "")]
        [TestCase("A task that represents the asynchronous operation. The Result is something", "something")]
        public void Code_gets_fixed_for_Task_with_generic_collection_(string originalText, string fixedText)
        {
            var originalCode = @"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>" + originalText + @"</returns>
    public Task<IList<int>> DoSomething { get; set; }
}
";

            var fixedCode = @"
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
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter contains a collection of " + fixedText + @"
    /// </returns>
    public Task<IList<int>> DoSomething { get; set; }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("Some integers.", "some integers.")]
        [TestCase("A task that represents the asynchronous operation. The Result is something", "something")]
        public void Code_gets_fixed_for_Task_with_array_(string originalText, string fixedText)
        {
            var originalCode = @"
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
    /// " + originalText + @"
    /// </returns>
    public Task<int[]> DoSomething { get; set; }
}
";

            var fixedCode = @"
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
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter contains an array of " + fixedText + @"
    /// </returns>
    public Task<int[]> DoSomething { get; set; }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("Some data.", "some data.")]
        [TestCase("A task that represents the asynchronous operation. The Result is something", "something")]
        public void Code_gets_fixed_for_Task_with_byte_array_(string originalText, string fixedText)
        {
            var originalCode = @"
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
    /// " + originalText + @"
    /// </returns>
    public Task<byte[]> DoSomething { get; set; }
}
";

            var fixedCode = @"
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
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter contains a byte array containing " + fixedText + @"
    /// </returns>
    public Task<byte[]> DoSomething { get; set; }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2035_CodeFixProvider();

//// ncrunch: no coverage start

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> CreateStartingPhrases()
        {
            string[] startingWords = ["a", "an", "the"];
            string[] modifications = ["readonly", "read-only", "read only"];
            string[] collections = ["array", "list", "dictionary", "enumerable", "queue", "stack", "map", "hashset", "hashSet", "hashtable", "hashTable", "hash set", "hashed set", "hash table", "hashed table", "hashing set", "hashing table"];
            string[] prepositions = ["of", "with", "that contains", "which contains", "that holds", "which holds", "containing", "holding"];

            foreach (var collection in collections)
            {
                foreach (var preposition in prepositions)
                {
                    var phrase = string.Concat(collection, " ", preposition);

                    yield return phrase.ToLowerCaseAt(0);
                    yield return phrase.ToUpperCaseAt(0);

                    foreach (var modification in modifications)
                    {
                        var modificationPhrase = string.Concat(modification, " ", phrase);

                        yield return modificationPhrase.ToLowerCaseAt(0);
                        yield return modificationPhrase.ToUpperCaseAt(0);

                        foreach (var startingWord in startingWords)
                        {
                            var shortStartingPhrase = string.Concat(startingWord, " ", collection);
                            var startingPhrase = string.Concat(startingWord, " ", phrase);
                            var modifiedStartingPhrase = string.Concat(startingWord, " ", modificationPhrase);

                            yield return shortStartingPhrase.ToLowerCaseAt(0);
                            yield return shortStartingPhrase.ToUpperCaseAt(0);

                            yield return startingPhrase.ToLowerCaseAt(0);
                            yield return startingPhrase.ToUpperCaseAt(0);

                            yield return modifiedStartingPhrase.ToLowerCaseAt(0);
                            yield return modifiedStartingPhrase.ToUpperCaseAt(0);
                        }
                    }
                }
            }
        }

//// ncrunch: no coverage end
    }
}