using System.Collections.Generic;
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
        private static readonly string[] TypeSummaryStartingPhrases = CreateTypeSummaryStartingPhrases().Distinct().OrderBy(_ => _.Length).ThenBy(_ => _).ToArray();

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
        public void Code_gets_fixed_for_class_summary_([ValueSource(nameof(TypeSummaryStartingPhrases))] string summary)
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
        public void Code_gets_fixed_for_interface_summary_([ValueSource(nameof(TypeSummaryStartingPhrases))] string summary)
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

        private static IEnumerable<string> CreateTypeSummaryStartingPhrases()
        {
            var startingPhrases = new[]
                                      {
                                          "A factory for creation of",
                                          "A factory for the creation of",
                                          "A factory for",
                                          "A factory that creates",
                                          "A factory that provides methods to create",
                                          "A factory to create",
                                          "A factory to provide methods to create",
                                          "A factory which creates",
                                          "A factory which provides methods to create",
                                          "A implementation of the abstract factory pattern for creation of",
                                          "A implementation of the factory pattern for creation of",
                                          "A interface for factories that create",
                                          "A interface for factories to create",
                                          "A interface for factories which create",
                                          "A interface implemented by factories that create",
                                          "A interface implemented by factories to create",
                                          "A interface implemented by factories which create",
                                          "A interface that is implemented by factories that create",
                                          "A interface that is implemented by factories which create",
                                          "A interface to create",
                                          "A interface which is implemented by factories that create",
                                          "A interface which is implemented by factories which create",
                                          "An implementation of the abstract factory pattern for creation of",
                                          "An implementation of the factory pattern for creation of",
                                          "An interface for factories that create",
                                          "An interface for factories to create",
                                          "An interface for factories which create",
                                          "An interface implemented by factories that create",
                                          "An interface implemented by factories to create",
                                          "An interface implemented by factories which create",
                                          "An interface that is implemented by factories that create",
                                          "An interface that is implemented by factories which create",
                                          "An interface to create",
                                          "An interface which is implemented by factories that create",
                                          "An interface which is implemented by factories which create",
                                          "Defines a factory that can create",
                                          "Defines a factory that creates",
                                          "Defines a factory that provides",
                                          "Defines a factory that is able to create",
                                          "Defines a factory that is capable to create",
                                          "Defines a factory to create",
                                          "Defines a factory which can create",
                                          "Defines a factory which creates",
                                          "Defines a factory which provides",
                                          "Defines a factory which is able to create",
                                          "Defines a factory which is capable to create",
                                          "Defines a factory for",
                                          "Defines a method to create",
                                          "Defines methods to create",
                                          "Defines the factory that can create",
                                          "Defines the factory that creates",
                                          "Defines the factory that provides",
                                          "Defines the factory that is able to create",
                                          "Defines the factory that is capable to create",
                                          "Defines the factory to create",
                                          "Defines the factory which can create",
                                          "Defines the factory which creates",
                                          "Defines the factory which provides",
                                          "Defines the factory which is able to create",
                                          "Defines the factory which is capable to create",
                                          "Defines the factory for",
                                          "Factory for providing",
                                          "Factory for creating",
                                          "Factory for creation of",
                                          "Factory for the creation of",
                                          "Factory for",
                                          "Factory that can create",
                                          "Factory that provides methods to create",
                                          "Factory that provides",
                                          "Factory to create",
                                          "Factory to provide methods to create",
                                          "Factory to provide",
                                          "Factory which can create",
                                          "Factory which provides methods to create",
                                          "Factory which provides",
                                          "Implementation of the abstract factory pattern for creation of",
                                          "Implementation of the factory pattern for creation of",
                                          "Interface for factories that create",
                                          "Interface for factories to create",
                                          "Interface for factories which create",
                                          "Interface that creates",
                                          "Interface to create",
                                          "Interface which creates",
                                          "Provides a factory that creates",
                                          "Provides a factory to create",
                                          "Provides a factory which creates",
                                          "Provides a factory for",
                                          "Provides a method to create",
                                          "Provides methods to create",
                                          "Provides the factory that creates",
                                          "Provides the factory to create",
                                          "Provides the factory which creates",
                                          "Provides the factory for",
                                          "Provides",
                                          "Represents a factory that can create",
                                          "Represents a factory that creates",
                                          "Represents a factory to create",
                                          "Represents a factory which can create",
                                          "Represents a factory which creates",
                                          "Represents a factory for",
                                          "Represents the factory that can create",
                                          "Represents the factory that creates",
                                          "Represents the factory to create",
                                          "Represents the factory which can create",
                                          "Represents the factory which creates",
                                          "Represents the factory for",
                                          "The factory that can create",
                                          "The factory that creates",
                                          "The factory that provides methods to create",
                                          "The factory to create",
                                          "The factory to provide methods to create",
                                          "The factory which can create",
                                          "The factory which creates",
                                          "The factory which provides methods to create",
                                          "The implementation of the abstract factory pattern for creation of",
                                          "The implementation of the factory pattern for creation of",
                                          "The interface implemented by factories that create",
                                          "The interface implemented by factories to create",
                                          "The interface implemented by factories which create",
                                          "The interface that is implemented by factories that create",
                                          "The interface that is implemented by factories to create",
                                          "The interface that is implemented by factories which create",
                                          "The interface which is implemented by factories that create",
                                          "The interface which is implemented by factories to create",
                                          "The interface which is implemented by factories which create",
                                          "This factory creates",
                                          "This factory provides methods to create",
                                          "This interface is implemented by factories that create",
                                          "This interface is implemented by factories to create",
                                          "This interface is implemented by factories which create",
                                          "Used for creating",
                                          "Used for creation of",
                                          "Used for the creation of",
                                          "Used to create",
                                          "Uses for creating", // typo in 'used'
                                          "Uses for creation of", // typo in 'used'
                                          "Uses for the creation of", // typo in 'used'
                                          "Uses to create", // typo in 'used'
                                      };

            var articles = new[]
                               {
                                   string.Empty,
                                   " a",
                                   " an",
                                   " the",
                               };

            var middles = new[]
                              {
                                  string.Empty,
                                  " instance of",
                                  " instances of",
                                  " new instance of",
                                  " new instances of",
                              };

            foreach (var startingPhrase in startingPhrases)
            {
                foreach (var article in articles)
                {
                    foreach (var middle in middles)
                    {
                        var start = startingPhrase + article;

                        if (string.IsNullOrWhiteSpace(middle))
                        {
                            yield return start;
                        }
                        else
                        {
                            yield return string.Concat(start, middle, article);
                        }
                    }
                }
            }

            yield return "Implementations create ";
        }
    }
}