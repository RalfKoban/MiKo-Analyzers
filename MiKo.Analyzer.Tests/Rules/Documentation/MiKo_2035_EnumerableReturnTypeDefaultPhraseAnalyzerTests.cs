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
        private static readonly string[] NonTaskReturnTypes =
                                                              [
                                                                  "IEnumerable",
                                                                  "IEnumerable<int>",
                                                                  "IList<int>",
                                                                  "ICollection<int>",
                                                                  "List<int>",
                                                                  "Dictionary<int, int>",
                                                              ];

        private static readonly string[] TaskReturnTypes =
                                                           [
                                                               "Task<int[]>",
                                                               "Task<IEnumerable>",
                                                               "Task<List<int>>",
                                                           ];

        private static readonly string[] ReturnTypes = [.. NonTaskReturnTypes, .. TaskReturnTypes];

        private static readonly string[] StartingPhrases = [.. Enumerable.ToHashSet(CreateStartingPhrases().Take(TestLimit))];

        private static readonly string[] OfStartingPhrases = [.. StartingPhrases.Where(_ => _.EndsWith(" of", StringComparison.OrdinalIgnoreCase))];

        [OneTimeSetUp]
        public static void PrepareTestEnvironment() => MiKo_2035_CodeFixProvider.LoadData();

        [Test]
        public void No_issue_is_reported_for_undocumented_method_([ValueSource(nameof(ReturnTypes))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_property_([ValueSource(nameof(ReturnTypes))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_commented_method_returning_XmlNode_type_([Values("XmlNode", "XmlElement", "XmlDocument")] string returnType) => No_issue_is_reported_for(@"
using System.Xml;

public class TestMe
{
    /// <returns>
    /// The item
    /// </returns>
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_method_returning_non_enumerable_type_(
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
        public void No_issue_is_reported_for_array_return_with_standard_phrase_(
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
        public void No_issue_is_reported_for_byte_array_return_with_standard_phrase() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_IEnumerable_return_with_sequence_phrase_(
                                                                                  [Values("returns", "value")] string xmlTag,
                                                                                  [Values("A sequence that contains", "A sequence of")] string startPhrase)
            => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + startPhrase + @" whatever.
    /// </" + xmlTag + @">
    public IEnumerable<int> DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_List_return_with_see_cref_and_standard_phrase_(
                                                                                        [Values("returns", "value")] string xmlTag,
                                                                                        [Values("A", "An")] string startingWord,
                                                                                        [Values("that contains", "containing")] string continuation)
            => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + startingWord + @" <see cref=""List{T}"" /> " + continuation + @" something.
    /// </" + xmlTag + @">
    public List<int> DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_List_of_return_with_see_cref_and_standard_phrase_(
                                                                                           [Values("returns", "value")] string xmlTag,
                                                                                           [Values("A", "An")] string startingWord,
                                                                                           [Values("that contains", "containing")] string continuation)
            => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + startingWord + @" <see cref=""List{T}"" /> of <see cref=""int""/> " + continuation + @" something.
    /// </" + xmlTag + @">
    public List<int> DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_IEnumerable_return_with_sequence_of_phrase_([Values("returns", "value")] string xmlTag)
            => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A sequence of whatever.
    /// </" + xmlTag + @">
    public IEnumerable<int> DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_IEnumerable_return_with_see_cref_and_standard_phrase_(
                                                                                               [Values("returns", "value")] string xmlTag,
                                                                                               [Values("A", "An")] string startingWord,
                                                                                               [Values("that contains", "containing")] string continuation)
            => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + startingWord + @" <see cref=""IEnumerable{T}"" /> " + continuation + @" something.
    /// </" + xmlTag + @">
    public IEnumerable<int> DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_IEnumerable_of_return_with_see_cref_and_standard_phrase_(
                                                                                                  [Values("returns", "value")] string xmlTag,
                                                                                                  [Values("A", "An")] string startingWord,
                                                                                                  [Values("that contains", "containing")] string continuation)
            => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + startingWord + @" <see cref=""IEnumerable{T}"" /> of <see cref=""int""/> " + continuation + @" something.
    /// </" + xmlTag + @">
    public IEnumerable<int> DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_Task_enumerable_return_with_standard_phrase_(
                                                                                      [Values("returns", "value")] string xmlTag,
                                                                                      [Values("", " ")] string space,
                                                                                      [ValueSource(nameof(TaskReturnTypes))] string returnType)
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
    /// A task that represents the asynchronous operation. The value of the <see cref=""System.Threading.Tasks.Task{TResult}.Result" + space + @"/> parameter contains a collection of whatever.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_Select_like_method_with_standard_phrase_([Values("returns", "value")] string xmlTag) => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// An <see cref=""IEnumerable{T}""/> whose elements are the result of something.
    /// </" + xmlTag + @">
    public IEnumerable<T> Select<T>(object o) => null;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_enumerable_return_with_non_standard_phrase_(
                                                                                     [Values("returns", "value")] string xmlTag,
                                                                                     [Values("A whatever", "An whatever", "The whatever")] string comment,
                                                                                     [ValueSource(nameof(ReturnTypes))] string returnType)
            => An_issue_is_reported_for(@"
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
        public void Code_gets_fixed_by_replacing_with_array_phrase_for_array_type()
        {
            const string OriginalCode = """

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

                """;

            const string FixedCode = """

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

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("The adjusted trivia array with proper whitespace indentation for comments", "An array of the adjusted trivia with proper whitespace indentation for comments")]
        public void Code_gets_fixed_by_normalizing_array_phrases_(string originalText, string fixedText)
        {
            const string Template = """

                using Microsoft.CodeAnalysis;

                public class TestMe
                {
                    /// <summary>
                    /// Does something.
                    /// </summary>
                    /// <returns>
                    /// ###.
                    /// </returns>
                    public SyntaxTrivia[] DoSomething { get; set; }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", fixedText));
        }

        [TestCase("The modified set after something", "A collection of elements from the original set after something")]
        public void Code_gets_fixed_by_replacing_with_collection_phrase_for_hashset_(string originalText, string fixedText)
        {
            const string Template = """

                using System.Collections.Generic;

                public class TestMe
                {
                    /// <summary>
                    /// Does something.
                    /// </summary>
                    /// <returns>
                    /// ###.
                    /// </returns>
                    public HashSet<int> DoSomething { get; set; }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", fixedText));
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
        public void Code_gets_fixed_by_replacing_with_byte_array_phrase_(string text)
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
        public void Code_gets_fixed_by_normalizing_collection_phrases_for_non_generic_collection_([ValueSource(nameof(StartingPhrases))] string originalPhrase)
        {
            const string Template = """

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
                    public ICollection DoSomething { get; set; }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", "A collection of"));
        }

        [TestCase("Some integers.", "A collection of some integers.")]
        [TestCase("The mapping information.", "A collection of the mapping information.")]
        public void Code_gets_fixed_by_replacing_with_collection_phrase_for_non_generic_collection_(string originalPhrase, string fixedPhrase)
        {
            const string Template = """

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
                    public ICollection DoSomething { get; set; }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", fixedPhrase));
        }

        [Test]
        public void Code_gets_fixed_by_normalizing_readonly_phrases_for_generic_readonly_collection_([Values("readonly", "read only", "read-only")] string modification, [Values("list", "collection")] string collection)
        {
            const string Template = """

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
                    public IReadOnlyCollection<int> DoSomething { get; set; }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", "A " + modification + " " + collection + " of"), Template.Replace("###", "A collection of"));
        }

        [Test]
        public void Code_gets_fixed_by_normalizing_collection_phrases_for_generic_collection_([ValueSource(nameof(StartingPhrases))] string originalPhrase)
        {
            const string Template = """

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
                    public ICollection<int> DoSomething { get; set; }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", "A collection of"));
        }

        [TestCase("Some integers.", "A collection of some integers.")]
        [TestCase("The mapping information.", "A collection of the mapping information.")]
        [TestCase("Gets the integers.", "A collection of the integers.")]
        [TestCase("Get the integers.", "A collection of the integers.")]
        [TestCase("The List with the integers.", "A collection of the integers.")]
        [TestCase("A List with the integers.", "A collection of the integers.")]
        public void Code_gets_fixed_by_replacing_with_collection_phrase_for_generic_collection_(string originalPhrase, string fixedPhrase)
        {
            const string Template = """

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
                    public ICollection<int> DoSomething { get; set; }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", fixedPhrase));
        }

        [TestCase("Some integers.", "A collection of some integers.")]
        [TestCase("The mapping information.", "A collection of the mapping information.")]
        public void Code_gets_fixed_by_replacing_with_collection_phrase_for_generic_collection_on_same_line_(string originalPhrase, string fixedPhrase)
        {
            var originalCode = @"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>" + originalPhrase + @"</returns>
    public ICollection<int> DoSomething { get; set; }
}
";

            var fixedCode = @"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// " + fixedPhrase + @"
    /// </returns>
    public ICollection<int> DoSomething { get; set; }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("Some data", "A collection of my items that contains some data")]
        [TestCase("Gets the information.", "A collection of my items that contains the information.")]
        [TestCase("Get the information.", "A collection of my items that contains the information.")]
        public void Code_gets_fixed_by_replacing_with_collection_phrase_for_generic_collection_with_non_primitive_type_(string originalPhrase, string fixedPhrase)
        {
            const string Template = """

                using System;
                using System.Collections.Generic;

                public record MyItem
                {
                }

                public class TestMe
                {
                    /// <summary>
                    /// Does something.
                    /// </summary>
                    /// <returns>
                    /// ###.
                    /// </returns>
                    public ICollection<MyItem> DoSomething { get; set; }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", fixedPhrase));
        }

        [Test]
        public void Code_gets_fixed_by_normalizing_sequence_phrases_for_non_generic_enumerable_([ValueSource(nameof(StartingPhrases))] string originalPhrase)
        {
            const string Template = """

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

                """;

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", "A sequence that contains"));
        }

        [TestCase("Some integers.", "A sequence that contains some integers.")]
        [TestCase("The mapping information.", "A sequence that contains the mapping information.")]
        [TestCase("An enumerable collection of invocation expressions that represent LINQ extension methods.", "A sequence that contains invocation expressions that represent LINQ extension methods.")]
        [TestCase("The syntax list of type parameter constraint clauses.", "A sequence that contains type parameter constraint clauses.")]
        [TestCase("A read-only list of attributes of the specified type.", "A sequence that contains attributes of the specified type.")]
        [TestCase("A separated syntax list of parameters accessible from the given context.", "A sequence that contains parameters accessible from the given context.")]
        [TestCase("An enumerable of strings that represents the text content without trivia.", "A sequence that contains the text content without trivia.")]
        public void Code_gets_fixed_by_replacing_with_sequence_phrase_for_non_generic_enumerable_(string originalPhrase, string fixedPhrase)
        {
            const string Template = """

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

                """;

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", fixedPhrase));
        }

        [Test]
        public void Code_gets_fixed_by_normalizing_sequence_phrases_for_non_generic_enumerable_on_same_line_([ValueSource(nameof(StartingPhrases))] string originalPhrase)
        {
            var originalCode = @"
using System;
using System.Collections;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>" + originalPhrase + @" some integers.</returns>
    public IEnumerable DoSomething { get; set; }
}
";

            const string FixedCode = @"
using System;
using System.Collections;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A sequence that contains some integers.
    /// </returns>
    public IEnumerable DoSomething { get; set; }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase("Some integers.", "A sequence that contains some integers.")]
        [TestCase("The mapping information.", "A sequence that contains the mapping information.")]
        [TestCase("An enumerable collection of invocation expressions that represent LINQ extension methods.", "A sequence that contains invocation expressions that represent LINQ extension methods.")]
        [TestCase("The syntax list of type parameter constraint clauses.", "A sequence that contains type parameter constraint clauses.")]
        [TestCase("A read-only list of attributes of the specified type.", "A sequence that contains attributes of the specified type.")]
        [TestCase("A separated syntax list of parameters accessible from the given context.", "A sequence that contains parameters accessible from the given context.")]
        [TestCase("An enumerable of strings that represents the text content without trivia.", "A sequence that contains the text content without trivia.")]
        public void Code_gets_fixed_by_replacing_with_sequence_phrase_for_non_generic_enumerable_on_same_line_(string originalPhrase, string fixedPhrase)
        {
            var originalCode = @"
using System;
using System.Collections;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>" + originalPhrase + @"</returns>
    public IEnumerable DoSomething { get; set; }
}
";

            var fixedCode = @"
using System;
using System.Collections;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// " + fixedPhrase + @"
    /// </returns>
    public IEnumerable DoSomething { get; set; }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_normalizing_sequence_phrases_for_generic_enumerable_([ValueSource(nameof(StartingPhrases))] string originalPhrase)
        {
            const string Template = """

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

                """;

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", "A sequence that contains"));
        }

        [TestCase("Some integers.", "A sequence that contains some integers.")]
        [TestCase("The mapping information.", "A sequence that contains the mapping information.")]
        public void Code_gets_fixed_by_replacing_with_sequence_phrase_for_generic_enumerable_with_primitive_type_(string originalPhrase, string fixedPhrase)
        {
            const string Template = """

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

                """;

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", fixedPhrase));
        }

        [TestCase("Some integers.", "A sequence that contains some integers.")]
        [TestCase("The mapping information.", "A sequence that contains the mapping information.")]
        public void Code_gets_fixed_by_replacing_with_sequence_phrase_for_generic_enumerable_with_primitive_type_on_same_line_(string originalPhrase, string fixedPhrase)
        {
            var originalCode = @"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>" + originalPhrase + @"</returns>
    public IEnumerable<int> DoSomething { get; set; }
}
";

            var fixedCode = @"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// " + fixedPhrase + @"
    /// </returns>
    public IEnumerable<int> DoSomething { get; set; }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("Some data", "A sequence that contains some data")]
        [TestCase("All ancestors of the specified type", "A sequence that contains all ancestors of the specified type")]
        public void Code_gets_fixed_by_replacing_with_sequence_phrase_for_generic_enumerable_with_non_primitive_type_(string originalPhrase, string fixedPhrase)
        {
            const string Template = """

                using System;
                using System.Collections.Generic;

                public record GroupedRow
                {
                }

                public class TestMe
                {
                    /// <summary>
                    /// Does something.
                    /// </summary>
                    /// <returns>
                    /// ###.
                    /// </returns>
                    public IEnumerable<GroupedRow> DoSomething { get; set; }
                }

                """;

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", fixedPhrase));
        }

        [TestCase("Some data", "A sequence that contains some data")]
        [TestCase("All ancestors of the specified type", "A sequence that contains all ancestors of the specified type")]
        public void Code_gets_fixed_by_replacing_with_sequence_phrase_for_generic_enumerable_with_generic_type_(string originalPhrase, string fixedPhrase)
        {
            const string Template = """

                using System;
                using System.Collections.Generic;

                public record MyStuff
                {
                }

                public class TestMe
                {
                    /// <summary>
                    /// Does something.
                    /// </summary>
                    /// <returns>
                    /// ###.
                    /// </returns>
                    public IEnumerable<T> DoSomething<T>() where T : MyStuff => null;
                }

                """;

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", fixedPhrase));
        }

        [TestCase("Some data", "A collection of my nodes that contains some data")]
        [TestCase("All ancestors of the specified type", "A collection of my nodes that contains all ancestors of the specified type")]
        public void Code_gets_fixed_by_replacing_with_collection_phrase_for_generic_collection_with_generic_closed_type_(string originalPhrase, string fixedPhrase)
        {
            const string Template = """

                using System;
                using System.Collections.Generic;

                public record MyNode
                {
                }

                public class TestMe
                {
                    /// <summary>
                    /// Does something.
                    /// </summary>
                    /// <returns>
                    /// ###.
                    /// </returns>
                    public IList<T> DoSomething<T>() where T : MyNode => null;
                }

                """;

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", fixedPhrase));
        }

        [TestCase("Some data", "A collection of some data")]
        [TestCase("All ancestors of the specified type", "A collection of all ancestors of the specified type")]
        public void Code_gets_fixed_by_replacing_with_collection_phrase_for_generic_collection_with_generic_open_type_(string originalPhrase, string fixedPhrase)
        {
            const string Template = """

                using System;
                using System.Collections.Generic;

                public class TestMe
                {
                    /// <summary>
                    /// Does something.
                    /// </summary>
                    /// <returns>
                    /// ###.
                    /// </returns>
                    public IList<T> DoSomething<T>() where T : class => null;
                }

                """;

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
        public void Code_gets_fixed_by_replacing_with_Task_collection_phrase_(string originalText, string fixedText)
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
        [TestCase("A task that can be used to await.", "")]
        [TestCase("A task that can be used to await", "")]
        [TestCase("A task to await.", "")]
        [TestCase("A task to await", "")]
        [TestCase("An awaitable task.", "")]
        [TestCase("An awaitable task", "")]
        [TestCase("A task that represents the asynchronous operation. The Result is something", "something")]
        public void Code_gets_fixed_by_replacing_with_Task_collection_phrase_on_same_line_(string originalText, string fixedText)
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
        public void Code_gets_fixed_by_replacing_with_Task_array_phrase_(string originalText, string fixedText)
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
        public void Code_gets_fixed_by_replacing_with_Task_byte_array_phrase_(string originalText, string fixedText)
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

        [Test]
        public void Code_gets_fixed_by_normalizing_collection_of_phrases_with_see_cref_([ValueSource(nameof(OfStartingPhrases))] string start)
        {
            var originalCode = @"
using System;
using System.Collections.Generic;
using System.Xml;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// " + start + @" <see cref=""XmlNode""/> that represents the value XML nodes.
    /// </returns>
    public List<XmlNode> DoSomething { get; set; }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Xml;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A collection of <see cref=""XmlNode""/> that represents the value XML nodes.
    /// </returns>
    public List<XmlNode> DoSomething { get; set; }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_normalizing_collection_of_phrases_with_see_cref_on_same_line_([ValueSource(nameof(OfStartingPhrases))] string start)
        {
            var originalCode = @"
using System;
using System.Collections.Generic;
using System.Xml;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>" + start + @" <see cref=""XmlNode""/> that represents the value XML nodes.</returns>
    public List<XmlNode> DoSomething { get; set; }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Xml;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A collection of <see cref=""XmlNode""/> that represents the value XML nodes.
    /// </returns>
    public List<XmlNode> DoSomething { get; set; }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_replacing_collection_phrase_with_array_phrase_for_array()
        {
            const string OriginalCode = """

                using System;
                using System.Collections.Generic;
                using System.Xml;

                public class TestMe
                {
                    /// <summary>
                    /// Does something.
                    /// </summary>
                    /// <returns>
                    /// A collection of XML nodes.
                    /// </returns>
                    public XmlNode[] DoSomething() => null;
                }

                """;

            const string FixedCode = """

                using System;
                using System.Collections.Generic;
                using System.Xml;

                public class TestMe
                {
                    /// <summary>
                    /// Does something.
                    /// </summary>
                    /// <returns>
                    /// An array of XML nodes.
                    /// </returns>
                    public XmlNode[] DoSomething() => null;
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2035_CodeFixProvider();

//// ncrunch: no coverage start

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> CreateStartingPhrases()
        {
            string[] startingWords = ["a", "an", "the", "a new", "the new"];
            string[] modifications = ["read-only", /* commented out to limit tests: "filtered", "concurrent", "single" */];
            string[] collections = [
                                       "array", "arraylist", "array list", "list", "dictionary", "enumerable", "queue", "stack", "map", "bag",
                                       //// commented out to limit tests: "hashset", "hashSet", "hashtable", "hashTable", "hash set", "hashed set", "hash table", "hashed table", "hashing set", "hashing table",
                                       "syntax list", "enumerable collection", "separated syntax list", "immutable array",
                                   ];
            string[] prepositions = ["of", "with", "that contains", "which contains", "containing"];

            foreach (var collection in collections)
            {
                foreach (var preposition in prepositions)
                {
                    var phrase = string.Concat(collection, " ", preposition);

                    yield return phrase;
                    yield return phrase.ToUpperCaseAt(0);

                    foreach (var modification in modifications)
                    {
                        var modificationPhrase = string.Concat(modification, " ", phrase);

                        yield return modificationPhrase;
                        yield return modificationPhrase.ToUpperCaseAt(0);

                        foreach (var startingWord in startingWords)
                        {
                            var shortStartingPhrase = string.Concat(startingWord, " ", collection);

                            if (shortStartingPhrase.StartsWith("a i", StringComparison.Ordinal) || shortStartingPhrase.StartsWith("a a", StringComparison.Ordinal))
                            {
                                // do not test "a array" or "a immutable array", to limit tests
                                continue;
                            }

                            if (shortStartingPhrase.StartsWith("an ", StringComparison.Ordinal) && shortStartingPhrase[3] != 'a' && shortStartingPhrase[3] != 'i')
                            {
                                // do not test "an dictionary" or "an hashset", to limit tests
                                continue;
                            }

                            var startingPhrase = string.Concat(startingWord, " ", phrase);
                            var modifiedStartingPhrase = string.Concat(startingWord, " ", modificationPhrase);

                            yield return shortStartingPhrase;
                            yield return shortStartingPhrase.ToUpperCaseAt(0);

                            yield return startingPhrase;
                            yield return startingPhrase.ToUpperCaseAt(0);

                            yield return modifiedStartingPhrase;
                            yield return modifiedStartingPhrase.ToUpperCaseAt(0);
                        }
                    }
                }
            }
        }

//// ncrunch: no coverage end
    }
}