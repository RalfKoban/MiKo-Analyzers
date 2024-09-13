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
    public sealed class MiKo_2060_FactoryAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ClassSummaryStartingPhrases = [.. CreateTypeSummaryStartingPhrases().Take(TestLimit).OrderBy(_ => _.Length).ThenBy(_ => _)];
        private static readonly string[] InterfaceSummaryStartingPhrases = [.. ClassSummaryStartingPhrases.Take(100)];

        [Test]
        public void No_issue_is_reported_for_undocumented_non_factory_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_documented_non_factory_class() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Does something.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_factory_class() => No_issue_is_reported_for(@"
using System;

public class TestMeFactory
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_factory_class() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Provides support for creating something.
/// </summary>
public class TestMeFactory
{
}
");

        [Test]
        public void No_issue_is_reported_for_private_method_on_factory_class() => No_issue_is_reported_for(@"
using System;

public class TestMeFactory
{
    /// <summary>Some documentation.</summary>
    private object DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_boolean_public_method_on_factory_class() => No_issue_is_reported_for(@"
using System;

public class TestMeFactory
{
    /// <summary>Some documentation.</summary>
    public bool DoSomething() => false;
}
");

        [Test]
        public void No_issue_is_reported_for_void_public_method_on_factory_class() => No_issue_is_reported_for(@"
using System;

public class TestMeFactory
{
    /// <summary>Some documentation.</summary>
    public void DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_factory_method_([Values(" ", "")] string gap) => No_issue_is_reported_for(@"
using System;

public class Whatever : IWhatever
{
}

public interface IWhatever
{
}

public class TestMeFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""IWhatever""" + gap + @"/> type with a result.
    /// </summary>
    public IWhatever Create() => new Whatever();
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_factory_method_on_generic_type_([Values(" ", "")] string gap) => No_issue_is_reported_for(@"
using System;

public class Whatever<T> : IWhatever<T>
{
}

public interface IWhatever<T>
{
}

public class TestMeFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""IWhatever{T}""" + gap + @"/> type with a result.
    /// </summary>
    public IWhatever<int> Create() => new Whatever<int>();
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_factory_method_on_generic_collection_type_([Values(" ", "")] string gap) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class Whatever<T> : IWhatever<T>
{
}

public interface IWhatever<T>
{
}

public class TestMeFactory
{
    /// <summary>
    /// Creates a collection of new instances of the <see cref=""IWhatever{T}""" + gap + @"/> type with a result.
    /// </summary>
    public IList<IWhatever<int>> Create() => new List<Whatever<int>>();
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_factory_method_on_string() => No_issue_is_reported_for(@"
public class TestMeFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""string""/> type with a result.
    /// </summary>
    public string Create() => string.Empty;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_factory_method_on_generic_string_collection() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMeFactory
{
    /// <summary>
    /// Creates a collection of new instances of the <see cref=""string""/> type with a result.
    /// </summary>
    public IList<string> Create() => new string[0];
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_factory_class() => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Provides something.
/// </summary>
public class TestMeFactory
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_factory_method_([Values(" ", "")] string gap) => An_issue_is_reported_for(@"
using System;

public class Whatever : IWhatever
{
}

public interface IWhatever
{
}

public class TestMeFactory
{
    /// <summary>
    /// Create a <see cref=""IWhatever""" + gap + @"/> with a result.
    /// </summary>
    public IWhatever Create() => new Whatever();
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_factory_method_on_generic_type_([Values(" ", "")] string gap) => An_issue_is_reported_for(@"
using System;

public class Whatever<T> : IWhatever<T>
{
}

public interface IWhatever<T>
{
}

public class TestMeFactory
{
    /// <summary>
    /// Create <see cref=""IWhatever{T}""" + gap + @"/> with a result.
    /// </summary>
    public IWhatever<int> Create() => new Whatever<int>();
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_factory_method_on_generic_collection_type_([Values(" ", "")] string gap) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class Whatever<T> : IWhatever<T>
{
}

public interface IWhatever<T>
{
}

public class TestMeFactory
{
    /// <summary>
    /// Create <see cref=""IWhatever{T}""" + gap + @"/> with a result.
    /// </summary>
    public IList<IWhatever<int>> Create() => new List<Whatever<int>>();
}
");

        [Test]
        public void Code_gets_fixed_for_class_summary_([ValueSource(nameof(ClassSummaryStartingPhrases))] string summary)
        {
            var originalCode = @"
/// <summary>
/// " + summary + @" something.
/// </summary>
public class TestMeFactory
{
    public string Create() => new string();
}
";

            const string FixedCode = @"
/// <summary>
/// Provides support for creating something.
/// </summary>
public class TestMeFactory
{
    public string Create() => new string();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_summary_([ValueSource(nameof(InterfaceSummaryStartingPhrases))] string summary)
        {
            var originalCode = @"
/// <summary>
/// " + summary + @" <see cref=""Xyz"" /> for a given <see cref=""IXyz"" /> object.
/// </summary>
public interface ITestMeFactory
{
}
";

            const string FixedCode = @"
/// <summary>
/// Provides support for creating <see cref=""Xyz"" /> for a given <see cref=""IXyz"" /> object.
/// </summary>
public interface ITestMeFactory
{
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase("A factory method for creating a")]
        [TestCase("A factory method that creates a")]
        [TestCase("A factory method which creates a")]
        [TestCase("A")]
        [TestCase("Factory method for creating a")]
        [TestCase("Factory method that creates a")]
        [TestCase("Factory method which creates a")]
        [TestCase("The factory method for creating a")]
        [TestCase("The factory method that creates a")]
        [TestCase("The factory method which creates a")]
        [TestCase("This factory method creates a")]
        [TestCase("This method creates a")]
        [TestCase("Used for creating a")]
        [TestCase("Used to create a")]
        [TestCase(@"Creates an <see cref=""string""/> with a")]
        public void Code_gets_fixed_for_method_summary_(string summary)
        {
            var originalCode = @"
public class TestMeFactory
{
    /// <summary>
    /// " + summary + @" result.
    /// </summary>
    public string Create() => new string();
}
";

            const string FixedCode = @"
public class TestMeFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""string""/> type with a result.
    /// </summary>
    public string Create() => new string();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_specific_method_summary_that_continues_with_based_on()
        {
            const string OriginalCode = @"
public class TestMeFactory
{
    /// <summary>
    /// Creates an instance of <see cref=""string""/> based on the identifier.
    /// </summary>
    public string Create() => new string();
}
";

            const string FixedCode = @"
public class TestMeFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""string""/> type with default values for the identifier.
    /// </summary>
    public string Create() => new string();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("Creates a factory")]
        [TestCase("Create a factory")]
        [TestCase("Creates an instance of <see cref=\"string\"/>")]
        [TestCase("Create an instance of <see cref=\"string\"/>")]
        public void Code_gets_fixed_for_specific_method_summary_that_continues_with_that_(string phrase)
        {
            var originalCode = @"
public class TestMeFactory
{
    /// <summary>
    /// " + phrase + @" that does something with the result.
    /// </summary>
    public string Create() => new string();
}
";

            const string FixedCode = @"
public class TestMeFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""string""/> type with default values that does something with the result.
    /// </summary>
    public string Create() => new string();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_collection_method_summary()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMeFactory
{
    /// <summary>
    /// A result.
    /// </summary>
    public IEnumerable<string> Create() => new string[0];
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMeFactory
{
    /// <summary>
    /// Creates a collection of new instances of the <see cref=""string""/> type with a result.
    /// </summary>
    public IEnumerable<string> Create() => new string[0];
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test] // https://github.com/dotnet/roslyn/issues/47550
        public void Code_gets_fixed_working_around_Roslyn_issue_47550()
        {
            const string OriginalCode = @"
internal interface IFactory
{
    /// <summary>
    /// Blah <see cref=""Xyz""/> blah.
    /// </summary>
    /// <returns></returns>
    IXyz Create();
}
";

            const string FixedCode = @"
internal interface IFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""IXyz""/> type with blah <see cref=""Xyz""/> blah.
    /// </summary>
    /// <returns></returns>
    IXyz Create();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("Construct a instance of the")]
        [TestCase("Construct a instance of")]
        [TestCase("Construct a new instance of the")]
        [TestCase("Construct a new instance of")]
        [TestCase("Construct a new")]
        [TestCase("Construct a")]
        [TestCase("Construct an instance of the")]
        [TestCase("Construct an instance of")]
        [TestCase("Construct instances of the")]
        [TestCase("Construct instances of")]
        [TestCase("Construct new instances of the")]
        [TestCase("Constructs a instance of the")]
        [TestCase("Constructs a instance of")]
        [TestCase("Constructs a new instance of the")]
        [TestCase("Constructs a new instance of")]
        [TestCase("Constructs a new")]
        [TestCase("Constructs a")]
        [TestCase("Constructs an instance of the")]
        [TestCase("Constructs an instance of")]
        [TestCase("Constructs instances of the")]
        [TestCase("Constructs instances of")]
        [TestCase("Constructs new instances of the")]
        [TestCase("Create a instance of the")]
        [TestCase("Create a instance of")]
        [TestCase("Create a new instance of the")]
        [TestCase("Create a new instance of")]
        [TestCase("Create a new")]
        [TestCase("Create a")]
        [TestCase("Create an instance of the")]
        [TestCase("Create an instance of")]
        [TestCase("Create instances of the")]
        [TestCase("Create instances of")]
        [TestCase("Create new instances of the")]
        [TestCase("Creates a instance of the")]
        [TestCase("Creates a instance of")]
        [TestCase("Creates a new")]
        [TestCase("Creates a")]
        [TestCase("Creates an instance of the")]
        [TestCase("Creates an instance of")]
        [TestCase("Creates an new instance of the")]
        [TestCase("Creates an new instance of")]
        [TestCase("Creates instances of the")]
        [TestCase("Creates instances of")]
        [TestCase("Creates new instances of the")]
        [TestCase("Creates and initializes a new instance of the")]
        [TestCase("Creates and initializes new instances of the")]
        [TestCase("Create and initialize a new instance of the")]
        [TestCase("Create and initialize new instances of the")]
        [TestCase("Creates and provides a new instance of the")]
        [TestCase("Creates and provides new instances of the")]
        [TestCase("Creates and returns a new instance of the")]
        [TestCase("Creates and returns new instances of the")]
        [TestCase("Return a new instance of the")]
        [TestCase("Return new instances of the")]
        [TestCase("Returns a new instance of the")]
        [TestCase("Returns new instances of the")]
        [TestCase("Get a new instance of the")]
        [TestCase("Get new instances of the")]
        [TestCase("Gets a new instance of the")]
        [TestCase("Gets new instances of the")]
        public void Code_gets_fixed_for_almost_correct_method_summary_starting_phrase_(string summary)
        {
            var originalCode = @"
internal interface IFactory
{
    /// <summary>
    /// " + summary + @" <see cref=""Xyz""/> type.
    /// </summary>
    IXyz Create();
}
";

            const string FixedCode = @"
internal interface IFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""IXyz""/> type with default values.
    /// </summary>
    IXyz Create();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase("Construct a instance")]
        [TestCase("Construct a new instance")]
        [TestCase("Construct an instance")]
        [TestCase("Construct instances")]
        [TestCase("Construct new instances")]
        [TestCase("Constructs a instance")]
        [TestCase("Constructs a new instance")]
        [TestCase("Constructs an instance")]
        [TestCase("Constructs instances")]
        [TestCase("Constructs new instances")]
        [TestCase("Create a instance")]
        [TestCase("Create a new instance")]
        [TestCase("Create an instance")]
        [TestCase("Create instances")]
        [TestCase("Create new instances")]
        [TestCase("Creates a instance")]
        [TestCase("Creates an instance")]
        [TestCase("Creates an new instance")]
        [TestCase("Creates instances")]
        [TestCase("Creates new instances")]
        [TestCase("Creates and initializes a new instance")]
        [TestCase("Creates and initializes new instances")]
        [TestCase("Create and initialize a new instance")]
        [TestCase("Create and initialize new instances")]
        [TestCase("Creates and provides a new instance")]
        [TestCase("Creates and provides new instances")]
        [TestCase("Creates and returns a new instance")]
        [TestCase("Creates and returns new instances")]
        [TestCase("Return a new instance")]
        [TestCase("Return new instances")]
        [TestCase("Returns a new instance")]
        [TestCase("Returns new instances")]
        [TestCase("Get a new instance")]
        [TestCase("Get new instances")]
        [TestCase("Gets a new instance")]
        [TestCase("Gets new instances")]
        public void Code_gets_fixed_for_almost_correct_interface_summary_starting_phrase_(string summary)
        {
            var originalCode = @"
/// <summary>
/// " + summary + @" of the <see cref=""IXyz""/> type.
/// </summary>
internal interface IFactory
{
    IXyz Create();
}
";

            const string FixedCode = @"
/// <summary>
/// Provides support for creating instances of the <see cref=""IXyz""/> type.
/// </summary>
internal interface IFactory
{
    IXyz Create();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2060_FactoryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2060_FactoryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2060_CodeFixProvider();

//// ncrunch: no coverage start

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local Violates CA1859
        [ExcludeFromCodeCoverage]
        private static HashSet<string> CreateTypeSummaryStartingPhrases()
        {
            string[] phrases =
                               [
                                   "A class containing factory methods",
                                   "A class containing methods",
                                   "A class providing factory methods",
                                   "A class providing methods",
                                   "A class that contains factory methods",
                                   "A class that contains methods",
                                   "A class that provides factory methods",
                                   "A class that provides methods",
                                   "A class which contains factory methods",
                                   "A class which contains methods",
                                   "A class which provides factory methods",
                                   "A class which provides methods",
                                   "A factory that provides methods",
                                   "A factory to provide methods",
                                   "A factory which provides methods",
                                   "A factory",
                                   "A implementation of the abstract factory pattern",
                                   "A implementation of the factory pattern",
                                   "A interface for factories",
                                   "A interface implemented by factories",
                                   "A interface of a factory",
                                   "A interface that is implemented by factories",
                                   "A interface which is implemented by factories",
                                   "A interface",
                                   "An implementation of the abstract factory pattern",
                                   "An implementation of the factory pattern",
                                   "An interface for factories",
                                   "An interface implemented by factories",
                                   "An interface of a factory",
                                   "An interface that is implemented by factories",
                                   "An interface which is implemented by factories",
                                   "An interface",
                                   "Class for factory methods",
                                   "Class for methods",
                                   "Class that provides factory methods",
                                   "Class that provides methods",
                                   "Class to provide factory methods",
                                   "Class to provide methods",
                                   "Class which provides factory methods",
                                   "Class which provides methods",
                                   "Class",
                                   "Defines a factory",
                                   "Defines a method",
                                   "Defines factories",
                                   "Defines methods",
                                   "Defines the factory",
                                   "Factory",
                                   "Implementation of the abstract factory pattern",
                                   "Implementation of the factory pattern",
                                   "Interface for factories",
                                   "Interface of a factory",
                                   "Interface of factories",
                                   "Interface",
                                   "Provides a factory",
                                   "Provides a method",
                                   "Provides factories",
                                   "Provides methods",
                                   "Provides",
                                   "Represents a factory",
                                   "Represents a method",
                                   "Represents factories",
                                   "Represents methods",
                                   "Represents",
                                   "The class containing factory methods",
                                   "The class containing methods",
                                   "The class contains factory methods",
                                   "The class contains methods",
                                   "The class provides factory methods",
                                   "The class provides methods",
                                   "The class providing factory methods",
                                   "The class providing methods",
                                   "The class that contains factory methods",
                                   "The class that contains methods",
                                   "The class that provides factory methods",
                                   "The class that provides methods",
                                   "The class which contains factory methods",
                                   "The class which contains methods",
                                   "The class which provides factory methods",
                                   "The class which provides methods",
                                   "The factory providing methods",
                                   "The factory that provides methods",
                                   "The factory to provide methods",
                                   "The factory which provides methods",
                                   "The factory",
                                   "The implementation of the abstract factory pattern",
                                   "The implementation of the factory pattern",
                                   "The interface implemented by factories",
                                   "The interface of a factory",
                                   "The interface that is implemented by factories",
                                   "The interface which is implemented by factories",
                                   "This class contains factory methods",
                                   "This class contains methods",
                                   "This class provides factory methods",
                                   "This class provides methods",
                                   "This factory provides methods",
                                   "This factory",
                                   "This interface is implemented by factories",
                                   "Used",
                                   "Uses", // typo in 'used'
                               ];

            string[] verbs = [
                                "creates",
                                "for", "for creating", "for creation of", "for the creation of",
                                "that create", "that can create", "that is able to create", "that are able to create", "that is capable to create", "that are capable to create", "that creates",
                                "to create",
                                "which create", "which can create", "which is able to create", "which are able to create", "which is capable to create", "which are capable to create", "which creates"
                             ];

            // we do not test them to limit number of tests
            // string[] articles = [string.Empty, " a", " an", " the"];
            // string[] middles = [string.Empty, " instance of", " instances of", " new instance of", " new instances of"];
            string[] articles = [string.Empty, " the"];
            string[] middles = [string.Empty, " instances of", " new instances of"];

            var s = verbs.SelectMany(_ => phrases, (verb, phrase) => phrase + " " + verb).ToList();

            var startingPhrases = s.Concat(s.Select(_ => _.Replace("actory", "actory class"))).ToList();
            var constructionPhrases = startingPhrases.Select(_ => _.Replace("creation", "construction").Replace("creating", "constructing").Replace("create", "construct")).ToList();
            var buildingPhrases = startingPhrases.Select(_ => _.Replace("creation", "building").Replace("creating", "building").Replace("create", "build")).ToList();
            var providingPhrases = startingPhrases.Select(_ => _.Replace("creation", "providing").Replace("creating", "providing").Replace("create", "provide")).ToList();

            var results = new HashSet<string>();

            foreach (var startingPhrase in startingPhrases.Concat(constructionPhrases).Concat(buildingPhrases).Concat(providingPhrases))
            {
                foreach (var article in articles)
                {
                    foreach (var middle in middles)
                    {
                        var start = startingPhrase + article;

                        if (string.IsNullOrWhiteSpace(middle))
                        {
                            results.Add(start);
                        }
                        else
                        {
                            results.Add(string.Concat(start, middle, article));
                        }
                    }
                }
            }

            string[] strangePhrases =
                                      [

                                          // "a instance",
                                          // "a new instances",
                                          // "an instances",
                                          // "an new instance",
                                          "epresents buil",
                                          "epresents construc",
                                          "epresents creat",
                                          "epresents for",
                                          "epresents provid",
                                          "epresents that",
                                          "epresents to",
                                          "epresents which",
                                          "factory class method",
                                          "method that are",
                                          "method which are",
                                          "methods a",
                                          "methods instance",
                                          "methods new",
                                          "methods that builds",
                                          "methods that constructs",
                                          "methods that creates",
                                          "methods that is",
                                          "methods that provides",
                                          "methods the",
                                          "methods which builds",
                                          "methods which constructs",
                                          "methods which creates",
                                          "methods which is",
                                          "methods which provides",
                                          "Provides buil",
                                          "Provides construct",
                                          "Provides creat",
                                          "Provides for",
                                          "Provides provid",
                                          "Provides that",
                                          "Provides to",
                                          "Provides which",
                                          "for providing of",
                                          "sed buil",
                                          "sed construc",
                                          "sed creat",
                                          "sed provid",
                                          "sed that",
                                          "sed which",
                                          "ses buil",
                                          "ses construc",
                                          "ses creat",
                                          "ses provid",
                                          "ses that",
                                          "ses which",
                                          "es that is capable",
                                          "es which is capable",
                                          "es that is able",
                                          "es which is able",
                                          "ss that are capable",
                                          "ss which are capable",
                                          "ss that are able",
                                          "ss which are able",
                                          "y that are capable",
                                          "y which are capable",
                                          "y that are able",
                                          "y which are able",
                                          "rn that are capable",
                                          "rn which are capable",
                                          "rn that are able",
                                          "rn which are able",
                                          "ace that are capable",
                                          "ace which are capable",
                                          "ace that are able",
                                          "ace which are able",
                                          "ies buil",
                                          "ies construc",
                                          "ies creat",
                                          "ies provid",
                                          "ies that provides",
                                          "ies which provides",
                                          "ds buil",
                                          "ds construc",
                                          "ds creat",
                                          "ds provid",
                                          "This factory class that",
                                          "This factory class which",
                                          "This factory class providing",
                                          "This factory providing",
                                          "for the constructing",
                                          "for the creating",
                                          "for the providing",
                                      ];

            results.RemoveWhere(_ => _.ContainsAny(strangePhrases));

            results.Add("Implementations create ");
            results.Add("Implementations construct ");
            results.Add("Implementations build ");
            results.Add("Implementations provide ");

            return results;
        }

//// ncrunch: no coverage end
    }
}