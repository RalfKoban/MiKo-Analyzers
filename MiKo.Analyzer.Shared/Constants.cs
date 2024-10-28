using System;
using System.Collections.Generic;
using System.Linq;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers
{
    internal static class Constants
    {
        internal const int Indentation = 4;

        internal const string Core = "Core";
        internal const string AsyncSuffix = "Async";
        internal const string AsyncCoreSuffix = AsyncSuffix + Core;
        internal const string RoutedEventFieldSuffix = "Event";
        internal const string TestsSuffix = "Tests";

        internal const string ExceptionIdentifier = "ex";
        internal const string InnerExceptionIdentifier = "inner";

        internal const string Entity = "Entity";
        internal const string Entities = "Entities";

#pragma warning disable SA1303 // Const field names should begin with upper-case letter
        internal const string entity = "entity";
        internal const string entities = "entities";
#pragma warning restore SA1303 // Const field names should begin with upper-case letter

        internal const string Element = "Element";

#pragma warning disable SA1303 // Const field names should begin with upper-case letter
        internal const string element = "element";
        internal const string frameworkElement = "frameworkElement";
#pragma warning restore SA1303 // Const field names should begin with upper-case letter

        internal const string EnvironmentNewLine = "\r\n";

        internal const string TODO = "TODO";

        internal const char Underscore = '_';

        internal static readonly char[] SentenceMarkers = ".?!;:".ToCharArray();
        internal static readonly char[] SentenceClauseMarkers = ",;".ToCharArray();
        internal static readonly char[] TrailingSentenceMarkers = " \t.?!;:,".ToCharArray();

        internal static readonly string[] WhiteSpaces = { " ", "\t", "\r", "\n" };
        internal static readonly char[] WhiteSpaceCharacters = { ' ', '\t', '\r', '\n' };

        internal static readonly string[] ParaTags = { "<para>", "<para />", "<para/>", "</para>" };

        internal static readonly char[] Underscores = { Underscore };

        internal static class ILog
        {
            internal const string NamespaceName = "log4net";
            internal const string TypeName = "ILog";
            internal const string FullTypeName = NamespaceName + "." + TypeName;

            internal const string Debug = nameof(Debug);
            internal const string Info = nameof(Info);
            internal const string Warn = nameof(Warn);
            internal const string Error = nameof(Error);
            internal const string Fatal = nameof(Fatal);

            internal const string DebugFormat = nameof(DebugFormat);
            internal const string InfoFormat = nameof(InfoFormat);
            internal const string WarnFormat = nameof(WarnFormat);
            internal const string ErrorFormat = nameof(ErrorFormat);
            internal const string FatalFormat = nameof(FatalFormat);

            internal const string IsDebugEnabled = nameof(IsDebugEnabled);
        }

        internal static class SeriLog
        {
            internal const string NamespaceName = "Serilog";
            internal const string TypeName = "Log";
            internal const string FullTypeName = NamespaceName + "." + TypeName;

            internal const string Verbose = nameof(Verbose);
            internal const string Debug = nameof(Debug);
            internal const string Information = nameof(Information);
            internal const string Warning = nameof(Warning);
            internal const string Error = nameof(Error);
            internal const string Fatal = nameof(Fatal);
        }

        internal static class MicrosoftLogging
        {
            internal const string NamespaceName = "Microsoft.Extensions.Logging";
            internal const string TypeName = "ILogger";
            internal const string FullTypeName = NamespaceName + "." + TypeName;

            internal const string BeginScope = nameof(BeginScope);
            internal const string Log = nameof(Log);
            internal const string LogCritical = nameof(LogCritical);
            internal const string LogDebug = nameof(LogDebug);
            internal const string LogError = nameof(LogError);
            internal const string LogInformation = nameof(LogInformation);
            internal const string LogTrace = nameof(LogTrace);
            internal const string LogWarning = nameof(LogWarning);
        }

        internal static class Moq
        {
            internal const string Mock = nameof(Mock);
            internal const string MockFullQualified = nameof(Moq) + "." + nameof(Mock);
            internal const string Object = nameof(Object);
            internal const string Of = nameof(Of);
            internal const string Setup = nameof(Setup);
            internal const string SetupGet = nameof(SetupGet);
            internal const string SetupSet = nameof(SetupSet);
            internal const string SetupSequence = nameof(SetupSequence);
            internal const string VerifyGet = nameof(VerifyGet);
            internal const string VerifySet = nameof(VerifySet);
            internal const string VerifyAll = nameof(VerifyAll);
            internal const string Verify = nameof(Verify);
            internal const string Verifiable = nameof(Verifiable);

            internal static class ConditionMatcher
            {
                internal const string It = nameof(It);
                internal const string Is = nameof(Is);
            }
        }

        internal static class FluentAssertions
        {
            internal const string Should = nameof(Should);
            internal const string ShouldBeEquivalentTo = nameof(ShouldBeEquivalentTo);
        }

        internal static class MaxNamingLengths
        {
            public const int Types = 40;
            public const int Methods = 25;
            public const int Events = 25;
            public const int Properties = 25;
            public const int Parameters = 20;
            public const int Fields = Parameters + 2;
            public const int LocalVariables = 15;
            public const int LocalVariablesInLoops = 9;
        }

        internal static class Markers
        {
            internal const string ThreadStaticFieldPrefix = "t_";
            internal const string StaticFieldPrefix = "s_";
            internal const string MemberFieldPrefix = "_";
            internal const string AlternativeMemberFieldPrefix = "m_";

            internal static readonly string[] BaseClasses = { "Abstract", "Base" };
            internal static readonly string[] Models = { "Model", "Models", "model", "models" };
            internal static readonly string[] ViewModels = { "ViewModel", "ViewModels", "viewModel", "viewModels" };
            internal static readonly string[] SpecialModels = { "Modeless", "modeless", "ModeLess", "modeLess", "semanticModel", "SemanticModel" };
            internal static readonly string[] Collections = { "List", "Dictionary", "ObservableCollection", "Collection", "Array", "HashSet", "Stack", "list", "dictionary", "observableCollection", "collection", "array", "hashSet", "stack" };
            internal static readonly string[] Symbols = { "T:", "P:", "M:", "F:", "E:", "!:" };
            internal static readonly string[] SymbolsAndLineBreaks = Symbols.Append(EnvironmentNewLine).ToArray();
            internal static readonly string[] Requirements = { "Must", "Need", "Shall", "Should", "Will", "Would" };
            internal static readonly string[] FieldPrefixes =
                                                              {
                                                                  string.Empty,
                                                                  MemberFieldPrefix,
                                                                  AlternativeMemberFieldPrefix,
                                                                  StaticFieldPrefix,
                                                                  ThreadStaticFieldPrefix,
                                                              };

            internal static readonly string[] OSBitNumbers = { "32", "64" };

            internal static readonly string[] ReSharper = { "ReSharper disable", "ReSharper restore" };
        }

        internal static class Comments
        {
            internal const string AlternativeStringReturnTypeStartingPhraseTemplate = "An interned copy of the {0} {1} ";
            internal const string ArrayReturnTypeStartingPhraseA = "An array of ";
            internal const string ArrayReturnTypeStartingPhraseALowerCase = "an array of ";
            internal const string ByteArrayReturnTypeStartingPhraseA = "A byte array containing ";
            internal const string ByteArrayReturnTypeStartingPhraseALowerCase = "a byte array containing ";
            internal const string Asynchronously = "Asynchronously";
            internal const string AsynchronouslyStartingPhrase = Asynchronously + " ";
            internal const string BooleanParameterEndingPhraseTemplate = "; otherwise, {0}.";
            internal const string BooleanParameterStartingPhraseTemplate = "{0} to ";
            internal const string BooleanReturnTypeEndingPhraseTemplate = "; otherwise, {0}.";
            internal const string BooleanReturnTypeStartingPhraseTemplate = "{0} if ";
            internal const string BooleanTaskReturnTypeEndingPhraseTemplate = ", otherwise with a result of {0}.";
            internal const string BooleanTaskReturnTypeStartingPhraseTemplate = "A task that will complete with a result of {0} if ";
            internal const string CallbackTerm = "callback";
            internal const string CollectionReturnTypeStartingPhrase = "A collection of ";
            internal const string CollectionReturnTypeStartingPhraseLowerCase = "a collection of ";
            internal const string CommandPropertyGetterOnlySummaryStartingPhraseTemplate = "Gets the {0} that can ";
            internal const string CommandPropertyGetterSetterSummaryStartingPhraseTemplate = "Gets or sets the {0} that can ";
            internal const string CommandPropertySetterOnlySummaryStartingPhraseTemplate = "Sets the {0} that can ";
            internal const string CommandSummaryStartingPhrase = "Represents a command that can ";
            internal const string ContinueWithTaskReturnTypeStartingPhrase = "A new continuation task.";
            internal const string DefaultCrefPhrase = DefaultStartingPhrase + "<see cref=\"{0}\"/>.";
            internal const string DefaultLangwordPhrase = DefaultStartingPhrase + "<see langword=\"{0}\"/>.";
            internal const string DefaultStartingPhrase = "The default is ";
            internal const string DelegateSummaryStartingPhrase = "Encapsulates a method that ";
            internal const string DependencyPropertyFieldSummaryPhraseTemplate = "Identifies the {0} dependency property.";
            internal const string DependencyPropertyFieldValuePhraseTemplate = "The identifier for the {0} dependency property.";
            internal const string DeterminesWhetherPhrase = "Determines whether";
            internal const string DisposeParameterPhrase = "Indicates whether unmanaged resources shall be freed.";
            internal const string DisposeSummaryPhrase = "Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.";
            internal const string EnumStartingPhrase = "Defines values that specify ";
            internal const string EnumTaskReturnTypeStartingPhraseTemplate = GenericTaskReturnTypeStartingPhraseTemplate + "the enumerated constant that is the ";
            internal const string EventArgsSummaryStartingPhrase = "Provides data for the ";
            internal const string EventHandlerSummaryStartingPhrase = "Handles the ";
            internal const string EventSummaryStartingPhrase = "Occurs ";
            internal const string ExampleDefaultPhrase = "The following example demonstrates ";
            internal const string ExceptionCtorExceptionParamPhraseTemplate = "The exception that is the cause of the current exception.{0}If the {1} parameter is not {2}, the current exception is raised in a {3} block that handles the inner exception.";
            internal const string ExceptionCtorExceptionParamSummaryContinuingPhrase = " and a reference to the inner exception that is the cause of this exception";
            internal const string ExceptionCtorMessageParamSummaryContinuingPhrase = " with a specified error message";
            internal const string ExceptionCtorSerializationParamRemarksPhrase = "This constructor is invoked during deserialization to reconstitute the exception object transmitted over a stream.";
            internal const string ExceptionCtorSerializationParamSummaryContinuingPhrase = " with serialized data";
            internal const string ExceptionCtorSummaryStartingPhraseTemplate = "Initializes a new instance of the {0} class";
            internal const string ExceptionPhrase = @"<exception cref=""{0}"">";
            internal const string ExceptionSplittingParaPhrase = "<para>" + SpecialOrPhrase + "</para>";
            internal const string ExceptionTypeSummaryStartingPhrase = "The exception that is thrown when ";
            internal const string ExtensionMethodClassStartingPhraseTemplate = "Provides a set of {0} methods for ";
            internal const string FactoryCreateCollectionMethodSummaryStartingPhraseTemplate = "Creates a collection of new instances of the {0} type with ";
            internal const string FactoryCreateMethodSummaryStartingPhraseTemplate = "Creates a new instance of the {0} type with ";
            internal const string FactorySummaryPhrase = "Provides support for creating ";
            internal const string FieldIsReadOnly = "This field is read-only.";
            internal const string FromCanceledTaskReturnTypeStartingPhrase = "The canceled task.";
            internal const string FromExceptionTaskReturnTypeStartingPhrase = "The faulted task.";
            internal const string FromResultTaskReturnTypeStartingPhrase = "The successfully completed task.";
            internal const string GenericTaskReturnTypeStartingPhraseTemplate = NonGenericTaskReturnTypeStartingPhraseTemplate + " The value of the {1} parameter contains ";
            internal const string IdTerm = "id";
            internal const string IdentTerm = "ident";
            internal const string InfoTerm = "info";
            internal const string NoDefaultPhrase = "This property has no default value.";
            internal const string NonGenericTaskReturnTypeStartingPhraseTemplate = "A {0} that represents the asynchronous operation.";
            internal const string ObjectDisposedExceptionAlternatingEndingPhrase = " has been closed.";
            internal const string ObjectDisposedExceptionAlternatingPhrase = "The current instance" + ObjectDisposedExceptionAlternatingEndingPhrase;
            internal const string ObjectDisposedExceptionEndingPhrase = " has been disposed.";
            internal const string ObjectDisposedExceptionPhrase = "The current instance" + ObjectDisposedExceptionEndingPhrase;
            internal const string ParamRefBeginningPhrase = @"<paramref name=""{0}""";
            internal const string RecursivelyStartingPhrase = "Recursively ";
            internal const string RoutedEventFieldSummaryPhraseTemplate = "Identifies the {0} routed event.";
            internal const string RoutedEventFieldValuePhraseTemplate = "The identifier for the {0} routed event.";
            internal const string RunTaskReturnTypeStartingPhrase = "A task that represents the work queued to execute in the thread pool.";
            internal const string SealedClassPhrase = "This class cannot be inherited.";
            internal const string SpecialOrPhrase = "-or-";
            internal const string StringReturnTypeStartingPhraseTemplate = "A {0} {1} ";
            internal const string ValueConverterSummaryStartingPhrase = "Represents a converter that converts ";
            internal const string ThatContainsTerm = "that contains";
            internal const string ToSeekTerm = "to seek";
            internal const string TryStartingPhrase = "Attempts to";
            internal const string WasNotSuccessfulPhrase = "was not successful";
            internal const string WhenAllTaskReturnTypeStartingPhrase = "A task that represents the completion of all of the supplied tasks.";
            internal const string WhenAnyTaskReturnTypeStartingPhraseTemplate = "A {0} that represents the completion of one of the supplied tasks. Its {1} is the task that completed first.";
            internal const string XmlElementEndingTag = "/>";
            internal const string XmlElementStartingTag = "<";

            internal static readonly char[] Delimiters = { ' ', '.', ',', ';', ':', '!', '?' };
            internal static readonly string[] UnusedPhrase = { "Unused.", "Unused", "This parameter is not used.", "This parameter is not used" };
            internal static readonly string[] FuturePhrase = { "Reserved for future usage.", "Reserved for future usage", "Reserved.", "Reserved", "future", };

            internal static readonly string[] EventSourcePhrase = new[] { "The source of the event.", "The source of the event" }.Concat(UnusedPhrase).ToArray();

            internal static readonly string[] AAnThePhraseWithSpaces = { "A ", "An ", "The " };
            internal static readonly string[] AAnThePhraseWithoutSpaces = { "A", "An", "The" };
            internal static readonly string[] FieldStartingPhrase = AAnThePhraseWithSpaces;
            internal static readonly string[] ParameterStartingPhrase = AAnThePhraseWithSpaces;
            internal static readonly string[] ParameterStartingCodefixPhrase = AAnThePhraseWithoutSpaces;
            internal static readonly string[] ReturnTypeStartingPhrase = AAnThePhraseWithSpaces;

            internal static readonly string[] OutParameterStartingPhrase = { "On successful return, contains " };
            internal static readonly string[] OutBoolParameterStartingPhrase = { "On successful return, indicates " };
            internal static readonly string[] EnumParameterStartingPhrase =
                                                                            {
                                                                                "One of the enumeration members that specifies ",
                                                                                "One of the enumeration members that determines ",
                                                                                "One of the enumeration values that specifies ",
                                                                                "One of the enumeration values that determines ",
                                                                                "One of the enumeration members specifying ",
                                                                                "One of the enumeration values specifying ",
                                                                                @"A <see cref=""{0}""/> value specifying ",
                                                                                @"A <see cref=""{0}"" /> value specifying ",
                                                                                @"A <see cref=""{0}""/> value that specifies ",
                                                                                @"A <see cref=""{0}"" /> value that specifies ",
                                                                                @"An <see cref=""{0}""/> value specifying ",
                                                                                @"An <see cref=""{0}"" /> value specifying ",
                                                                                @"An <see cref=""{0}""/> value that specifies ",
                                                                                @"An <see cref=""{0}"" /> value that specifies ",
                                                                            };

            internal static readonly string[] CancellationTokenParameterPhrase = { "The token to monitor for cancellation requests." };

            internal static readonly string[] MeaninglessStartingPhrase =
                                                                          {
                                                                              "A ",
                                                                              "Action",
                                                                              "Adapter",
                                                                              "An ",
                                                                              "Attribute",
                                                                              "Base",
                                                                              "Builder",
                                                                              "Called ",
                                                                              "Class",
                                                                              "Command",
                                                                              "Component",
                                                                              "Constructor",
                                                                              "Converter",
                                                                              "Creator",
                                                                              "Ctor",
                                                                              "Default impl ",
                                                                              "Default implementation for ",
                                                                              "Default implementation of ",
                                                                              "Default-Impl ",
                                                                              "Default-Implementation for ",
                                                                              "Default-Implementation of ",
                                                                              "Delegate",
                                                                              "Does implement ",
                                                                              Entity,
                                                                              "Event",
                                                                              "Extension class of",
                                                                              "Extension of",
                                                                              "Extension method ",
                                                                              "Factory",
                                                                              "Fake ",
                                                                              "Field",
                                                                              "For ",
                                                                              "Func",
                                                                              "Function",
                                                                              "Handler ",
                                                                              "Help ",
                                                                              "Helper ",
                                                                              "Impl ",
                                                                              "Implement ",
                                                                              "Implementation for ",
                                                                              "Implementation of ",
                                                                              "Implements ",
                                                                              "Interaction logic",
                                                                              "Interface",
                                                                              "Internal ",
                                                                              "Is ",
                                                                              "It ",
                                                                              "Its ",
                                                                              "It's ",
                                                                              "Method",
                                                                              "Mock ",
                                                                              "Model",
                                                                              "Private ",
                                                                              "Property",
                                                                              "Protected ",
                                                                              "Proxy ",
                                                                              "Public ",
                                                                              "Stub ",
                                                                              "Testclass ",
                                                                              "That ",
                                                                              "The ",
                                                                              "This ",
                                                                              "To ",
                                                                              "Use this ",
                                                                              "Used ",
                                                                              "Uses ",
                                                                              "View", // includes 'ViewModel'
                                                                              "Which ",
                                                                              "Wrapper",
                                                                          };

            internal static readonly string[] MeaninglessTypeStartingPhrase = MeaninglessStartingPhrase.Concat(new[] { "Contains", "Contain", "Has" }).OrderBy(_ => _.Length).ToArray();

            internal static readonly string[] MeaninglessPhrase =
                                                                  {
                                                                      "does implement",
                                                                      "implements",
                                                                      "that is called",
                                                                      "that is capable",
                                                                      "that is used",
                                                                      "used for",
                                                                      "used to",
                                                                      "capable to",
                                                                      "which is called",
                                                                      "which is capable",
                                                                      "which is used",
                                                                  };

            internal static readonly string[] MeaninglessFieldStartingPhrase = MeaninglessStartingPhrase.Except(FieldStartingPhrase).OrderBy(_ => _.Length).ToArray();

            internal static readonly string[] WhenAnyTaskReturnTypeStartingPhrase =
                                                                                    {
                                                                                        WhenAnyTaskReturnTypeStartingPhraseTemplate.FormatWith("task", "<see cref=\"Task{TResult}.Result\" />"), // this is just to have a proposal how to optimize
                                                                                        WhenAnyTaskReturnTypeStartingPhraseTemplate.FormatWith("task", "<see cref=\"System.Threading.Tasks.Task`1.Result\"/>"),
                                                                                        WhenAnyTaskReturnTypeStartingPhraseTemplate.FormatWith("task", "<see cref=\"System.Threading.Tasks.Task`1.Result\" />"),
                                                                                        WhenAnyTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1\" />", "<see cref=\"System.Threading.Tasks.Task`1.Result\" />"),
                                                                                        WhenAnyTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1\" />", "<see cref=\"System.Threading.Tasks.Task`1.Result\"/>"),
                                                                                        WhenAnyTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1\"/>", "<see cref=\"System.Threading.Tasks.Task`1.Result\" />"),
                                                                                        WhenAnyTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1\"/>", "<see cref=\"System.Threading.Tasks.Task`1.Result\"/>"),
                                                                                    };

            internal static readonly string[] NonGenericTaskReturnTypePhrase =
                                                                               {
                                                                                   NonGenericTaskReturnTypeStartingPhraseTemplate.FormatWith("task"),
                                                                                   NonGenericTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task\"/>"),
                                                                                   NonGenericTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task\" />"),
                                                                               };

            internal static readonly string[] GenericTaskReturnTypeStartingPhrase =
                                                                                    {
                                                                                        GenericTaskReturnTypeStartingPhraseTemplate.FormatWith("task", "<see cref=\"Task{TResult}.Result\" />"), // this is just to have a proposal how to optimize
                                                                                        GenericTaskReturnTypeStartingPhraseTemplate.FormatWith("task", "<see cref=\"System.Threading.Tasks.Task`1.Result\"/>"),
                                                                                        GenericTaskReturnTypeStartingPhraseTemplate.FormatWith("task", "<see cref=\"System.Threading.Tasks.Task`1.Result\" />"),
                                                                                        GenericTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1\" />", "<see cref=\"System.Threading.Tasks.Task`1.Result\" />"),
                                                                                        GenericTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1\" />", "<see cref=\"System.Threading.Tasks.Task`1.Result\"/>"),
                                                                                        GenericTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1\"/>", "<see cref=\"System.Threading.Tasks.Task`1.Result\" />"),
                                                                                        GenericTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1\"/>", "<see cref=\"System.Threading.Tasks.Task`1.Result\"/>"),
                                                                                    };

            internal static readonly string[] BooleanReturnTypeStartingPhrase =
                                                                                {
                                                                                    BooleanReturnTypeStartingPhraseTemplate.FormatWith("<see langword=\"true\"/>"),
                                                                                    BooleanReturnTypeStartingPhraseTemplate.FormatWith("<see langword=\"true\" />"),
                                                                                };

            internal static readonly string[] BooleanReturnTypeEndingPhrase =
                                                                              {
                                                                                  BooleanReturnTypeEndingPhraseTemplate.FormatWith("<see langword=\"false\"/>"),
                                                                                  BooleanReturnTypeEndingPhraseTemplate.FormatWith("<see langword=\"false\" />"),
                                                                              };

            internal static readonly string[] BooleanParameterStartingPhrase =
                                                                               {
                                                                                   BooleanParameterStartingPhraseTemplate.FormatWith("<see langword=\"true\"/>"),
                                                                                   BooleanParameterStartingPhraseTemplate.FormatWith("<see langword=\"true\" />"),
                                                                               };

            internal static readonly string[] BooleanParameterEndingPhrase =
                                                                             {
                                                                                 BooleanParameterEndingPhraseTemplate.FormatWith("<see langword=\"false\"/>"),
                                                                                 BooleanParameterEndingPhraseTemplate.FormatWith("<see langword=\"false\" />"),
                                                                             };

            internal static readonly string[] BooleanPropertySetterStartingPhrase = BooleanReturnTypeStartingPhrase.Union(BooleanParameterStartingPhrase).ToArray();

            internal static readonly string[] BooleanTaskReturnTypeStartingPhrase =
                                                                                    {
                                                                                        BooleanTaskReturnTypeStartingPhraseTemplate.FormatWith("<see langword=\"true\"/>"),
                                                                                        BooleanTaskReturnTypeStartingPhraseTemplate.FormatWith("<see langword=\"true\" />"),
                                                                                    };

            internal static readonly string[] BooleanTaskReturnTypeEndingPhrase =
                                                                                  {
                                                                                      BooleanTaskReturnTypeEndingPhraseTemplate.FormatWith("<see langword=\"false\"/>"),
                                                                                      BooleanTaskReturnTypeEndingPhraseTemplate.FormatWith("<see langword=\"false\" />"),
                                                                                  };

            internal static readonly string[] StringReturnTypeStartingPhrase =
                                                                               {
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\"/>", ThatContainsTerm), // this is just to have a proposal how to optimize
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\" />", ThatContainsTerm),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\"/>", ThatContainsTerm),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\" />", ThatContainsTerm),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\"/>", "containing"),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\" />", "containing"),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\"/>", "containing"),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\" />", "containing"),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\"/>", "that consists of"),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\" />", "that consists of"),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\"/>", "that consists of"),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\" />", "that consists of"),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\"/>", "that represents"),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\" />", "that represents"),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\"/>", "that represents"),
                                                                                   StringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\" />", "that represents"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\"/>", "where"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\" />", "where"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\"/>", "where"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\" />", "where"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\"/>", ThatContainsTerm),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\" />", ThatContainsTerm),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\"/>", ThatContainsTerm),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\" />", ThatContainsTerm),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\"/>", "containing"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\" />", "containing"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\"/>", "containing"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\" />", "containing"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\"/>", "that consists of"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\" />", "that consists of"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\"/>", "that consists of"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\" />", "that consists of"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\"/>", "that represents"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"string\" />", "that represents"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\"/>", "that represents"),
                                                                                   AlternativeStringReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.String\" />", "that represents"),
                                                                               };

            internal static readonly string StringTaskReturnTypeStartingPhraseTemplate = NonGenericTaskReturnTypeStartingPhraseTemplate.FormatWith("task") + " The value of the {0} parameter returns a {1} that {2} ";

            internal static readonly string[] StringTaskReturnTypeStartingPhrase =
                                                                                   {
                                                                                       StringTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"Task{TResult}.Result\"/>", "<see cref=\"string\"/>", "contains"), // this is just to have a proposal how to optimize
                                                                                       StringTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1.Result\"/>", "<see cref=\"System.String\"/>", "contains"),
                                                                                       StringTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1.Result\" />", "<see cref=\"System.String\" />", "contains"),
                                                                                       StringTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1.Result\" />", "<see cref=\"System.String\"/>", "contains"),
                                                                                       StringTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1.Result\"/>", "<see cref=\"System.String\" />", "contains"),
                                                                                       StringTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"Task{TResult}.Result\"/>", "<see cref=\"string\"/>", "consists of"),
                                                                                       StringTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1.Result\"/>", "<see cref=\"System.String\"/>", "consists of"),
                                                                                       StringTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1.Result\" />", "<see cref=\"System.String\" />", "consists of"),
                                                                                       StringTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1.Result\" />", "<see cref=\"System.String\"/>", "consists of"),
                                                                                       StringTaskReturnTypeStartingPhraseTemplate.FormatWith("<see cref=\"System.Threading.Tasks.Task`1.Result\"/>", "<see cref=\"System.String\" />", "consists of"),
                                                                                   };

            internal static readonly string[] EnumReturnTypeStartingPhrase = { "The enumerated constant that is the ", };

            internal static readonly string[] EnumTaskReturnTypeStartingPhrase = GenericTaskReturnTypeStartingPhrase.ToArray(_ => _ + "the enumerated constant that is the ");

            internal static readonly string[] EnumerableReturnTypeStartingPhrase =
                                                                                   {
                                                                                       CollectionReturnTypeStartingPhrase,
                                                                                       "A <see cref=\"{0}\"/> that contains ",
                                                                                       "A <see cref=\"{0}\" /> that contains ",
                                                                                       "An <see cref=\"{0}\"/> that contains ",
                                                                                       "An <see cref=\"{0}\" /> that contains ",
                                                                                   };

            internal static readonly string[] EnumerableTaskReturnTypeStartingPhrase = GenericTaskReturnTypeStartingPhrase.ToArray(_ => _ + CollectionReturnTypeStartingPhraseLowerCase);

            internal static readonly string[] ArrayReturnTypeStartingPhrase = { ArrayReturnTypeStartingPhraseA, "The array of " };

            internal static readonly string[] ByteArrayReturnTypeStartingPhrase = { ByteArrayReturnTypeStartingPhraseA, "The byte array containing " };

            internal static readonly string[] ArrayTaskReturnTypeStartingPhrase = GenericTaskReturnTypeStartingPhrase.ToArray(_ => _ + ArrayReturnTypeStartingPhraseALowerCase);

            internal static readonly string[] ByteArrayTaskReturnTypeStartingPhrase = GenericTaskReturnTypeStartingPhrase.ToArray(_ => _ + ByteArrayReturnTypeStartingPhraseALowerCase);

            internal static readonly string[] DependencyPropertyFieldSummaryPhrase =
                                                                                     {
                                                                                         DependencyPropertyFieldSummaryPhraseTemplate.FormatWith("<see cref=\"{0}\"/>"),
                                                                                         DependencyPropertyFieldSummaryPhraseTemplate.FormatWith("<see cref=\"{0}\" />"),
                                                                                     };

            internal static readonly string[] DependencyPropertyFieldValuePhrase =
                                                                                   {
                                                                                       DependencyPropertyFieldValuePhraseTemplate.FormatWith("<see cref=\"{0}\"/>"),
                                                                                       DependencyPropertyFieldValuePhraseTemplate.FormatWith("<see cref=\"{0}\" />"),
                                                                                   };

            internal static readonly string[] RoutedEventFieldSummaryPhrase =
                                                                              {
                                                                                  RoutedEventFieldSummaryPhraseTemplate.FormatWith("<see cref=\"{0}\"/>"),
                                                                                  RoutedEventFieldSummaryPhraseTemplate.FormatWith("<see cref=\"{0}\" />"),
                                                                              };

            internal static readonly string[] RoutedEventFieldValuePhrase =
                                                                            {
                                                                                RoutedEventFieldValuePhraseTemplate.FormatWith("<see cref=\"{0}\"/>"),
                                                                                RoutedEventFieldValuePhraseTemplate.FormatWith("<see cref=\"{0}\" />"),
                                                                            };

            internal static readonly string[] EventHandlerSummaryPhrase =
                                                                          {
                                                                              EventHandlerSummaryStartingPhrase + "<see cref=\"{0}\"/> event",
                                                                              EventHandlerSummaryStartingPhrase + "<see cref=\"{0}\" /> event",
                                                                          };

            internal static readonly string[] DefaultCrefPhrases =
                                                                   {
                                                                       DefaultCrefPhrase,
                                                                       DefaultStartingPhrase + "<see cref=\"{0}\" />.",
                                                                   };

            internal static readonly string[] DefaultBooleanLangwordPhrases =
                                                                              {
                                                                                  DefaultStartingPhrase + "<see langword=\"true\"/>.",
                                                                                  DefaultStartingPhrase + "<see langword=\"false\"/>.",
                                                                                  DefaultStartingPhrase + "<see langword=\"true\" />.",
                                                                                  DefaultStartingPhrase + "<see langword=\"false\" />.",
                                                                              };

            internal static readonly ISet<string> InvalidSummaryCrefXmlTags = new HashSet<string>
                                                                                  {
                                                                                      XmlTag.Example,
                                                                                      XmlTag.Exception,
                                                                                      XmlTag.Include,
                                                                                      XmlTag.Inheritdoc,
                                                                                      XmlTag.Overloads,
                                                                                      XmlTag.Param,
                                                                                      XmlTag.ParamRef,
                                                                                      XmlTag.Permission,
                                                                                      XmlTag.Remarks,
                                                                                      XmlTag.Returns,
                                                                                      XmlTag.SeeAlso,
                                                                                      XmlTag.Summary,
                                                                                      XmlTag.TypeParam,
                                                                                      XmlTag.Value,
                                                                                  };

            internal static readonly string[] ExceptionCtorSummaryStartingPhrase =
                                                                                   {
                                                                                       ExceptionCtorSummaryStartingPhraseTemplate.FormatWith("<see cref=\"{0}\"/>"),
                                                                                       ExceptionCtorSummaryStartingPhraseTemplate.FormatWith("<see cref=\"{0}\" />"),
                                                                                   };

            internal static readonly string[] CtorSerializationInfoParamPhrase = { "The object that holds the serialized object data." };
            internal static readonly string[] CtorStreamingContextParamPhrase = { "The contextual information about the source or destination." };

            internal static readonly string[] ExceptionCtorMessageParamPhrase = { "The error message that explains the reason for the exception." };

            internal static readonly string[] ExceptionCtorExceptionParamPhrase =
                                                                                  {
                                                                                      ExceptionCtorExceptionParamPhraseTemplate.FormatWith(" ", @"<paramref name=""innerException""/>", @"<see langword=""null""/>", "<b>catch</b>"),
                                                                                      ExceptionCtorExceptionParamPhraseTemplate.FormatWith(" ", @"<paramref name=""innerException"" />", @"<see langword=""null"" />", "<b>catch</b>"),
                                                                                      ExceptionCtorExceptionParamPhraseTemplate.FormatWith(" ", @"<paramref name=""innerException"" />", @"<see langword=""null""/>", "<b>catch</b>"),
                                                                                      ExceptionCtorExceptionParamPhraseTemplate.FormatWith(" ", @"<paramref name=""innerException""/>", @"<see langword=""null"" />", "<b>catch</b>"),
                                                                                  };

            internal static readonly string[] FactoryCreateMethodSummaryStartingPhrase =
                                                                                         {
                                                                                             FactoryCreateMethodSummaryStartingPhraseTemplate.FormatWith("<see cref=\"{0}\"/>"),
                                                                                             FactoryCreateMethodSummaryStartingPhraseTemplate.FormatWith("<see cref=\"{0}\" />"),
                                                                                         };

            internal static readonly string[] FactoryCreateCollectionMethodSummaryStartingPhrase =
                                                                                                   {
                                                                                                       FactoryCreateCollectionMethodSummaryStartingPhraseTemplate.FormatWith("<see cref=\"{0}\"/>"),
                                                                                                       FactoryCreateCollectionMethodSummaryStartingPhraseTemplate.FormatWith("<see cref=\"{0}\" />"),
                                                                                                   };

            internal static readonly string[] ExtensionMethodClassStartingPhrase =
                                                                                   {
                                                                                       ExtensionMethodClassStartingPhraseTemplate.FormatWith("<see langword=\"static\"/>"),
                                                                                       ExtensionMethodClassStartingPhraseTemplate.FormatWith("<see langword=\"static\" />"),
                                                                                   };

            internal static readonly string[] ArgumentNullExceptionStartingPhrase =
                                                                                    {
                                                                                        ParamRefBeginningPhrase + "/> is <see langword=\"null\"/>.",
                                                                                        ParamRefBeginningPhrase + " /> is <see langword=\"null\"/>.",
                                                                                        ParamRefBeginningPhrase + "/> is <see langword=\"null\" />.",
                                                                                        ParamRefBeginningPhrase + " /> is <see langword=\"null\" />.",
                                                                                    };

            internal static readonly string[] ArgumentExceptionStartingPhrase =
                                                                                {
                                                                                    ParamRefBeginningPhrase + "/> is ",
                                                                                    ParamRefBeginningPhrase + " /> is ",
                                                                                    ParamRefBeginningPhrase + "/> contains ",
                                                                                    ParamRefBeginningPhrase + " /> contains ",
                                                                                    ParamRefBeginningPhrase + "/> does ",
                                                                                    ParamRefBeginningPhrase + " /> does ",
                                                                                    ParamRefBeginningPhrase + "/> has ",
                                                                                    ParamRefBeginningPhrase + " /> has ",
                                                                                };

            internal static readonly string[] ArgumentOutOfRangeExceptionStartingPhrase = ArgumentExceptionStartingPhrase;

            internal static readonly string[] ExceptionSplittingPhrase = { SpecialOrPhrase };

            internal static readonly string[] ExceptionForbiddenStartingPhrase =
                                                                                 {
                                                                                     "A exception ",
                                                                                     "An exception ",
                                                                                     "Can be thrown ",
                                                                                     "Exception ",
                                                                                     "Fired ",
                                                                                     "Gets thrown ",
                                                                                     "If ",
                                                                                     "In case ",
                                                                                     "Is fired ",
                                                                                     "Is thrown ",
                                                                                     "Should be thrown ",
                                                                                     "The exception ",
                                                                                     "This exception ",
                                                                                     "Throw ",
                                                                                     "Thrown ",
                                                                                     "Throws ",
                                                                                     "When ",
                                                                                     "Will be thrown ",
                                                                                 };

            internal static readonly string[] CommandPropertyGetterSetterSummaryStartingPhrase =
                                                                                                 {
                                                                                                     CommandPropertyGetterSetterSummaryStartingPhraseTemplate.FormatWith(@"<see cref=""ICommand""/>"),
                                                                                                     CommandPropertyGetterSetterSummaryStartingPhraseTemplate.FormatWith(@"<see cref=""ICommand"" />"),
                                                                                                     CommandPropertyGetterSetterSummaryStartingPhraseTemplate.FormatWith(@"<see cref=""System.Windows.Input.ICommand""/>"),
                                                                                                     CommandPropertyGetterSetterSummaryStartingPhraseTemplate.FormatWith(@"<see cref=""System.Windows.Input.ICommand"" />"),
                                                                                                 };

            internal static readonly string[] CommandPropertyGetterOnlySummaryStartingPhrase =
                                                                                               {
                                                                                                   CommandPropertyGetterOnlySummaryStartingPhraseTemplate.FormatWith(@"<see cref=""ICommand""/>"),
                                                                                                   CommandPropertyGetterOnlySummaryStartingPhraseTemplate.FormatWith(@"<see cref=""ICommand"" />"),
                                                                                                   CommandPropertyGetterOnlySummaryStartingPhraseTemplate.FormatWith(@"<see cref=""System.Windows.Input.ICommand""/>"),
                                                                                                   CommandPropertyGetterOnlySummaryStartingPhraseTemplate.FormatWith(@"<see cref=""System.Windows.Input.ICommand"" />"),
                                                                                               };

            internal static readonly string[] CommandPropertySetterOnlySummaryStartingPhrase =
                                                                                               {
                                                                                                   CommandPropertySetterOnlySummaryStartingPhraseTemplate.FormatWith(@"<see cref=""ICommand""/>"),
                                                                                                   CommandPropertySetterOnlySummaryStartingPhraseTemplate.FormatWith(@"<see cref=""ICommand"" />"),
                                                                                                   CommandPropertySetterOnlySummaryStartingPhraseTemplate.FormatWith(@"<see cref=""System.Windows.Input.ICommand""/>"),
                                                                                                   CommandPropertySetterOnlySummaryStartingPhraseTemplate.FormatWith(@"<see cref=""System.Windows.Input.ICommand"" />"),
                                                                                               };

            internal static readonly string[] AttributeSummaryStartingPhrase =
                                                                               {
                                                                                   "Specifies ",
                                                                                   "Indicates ",
                                                                                   "Defines ",
                                                                                   "Provides ",
                                                                                   "Allows ",
                                                                                   "Represents ",
                                                                                   "Marks ",
                                                                               };

            internal static readonly string[] NotContradictionPhrase =
                                                                       {
                                                                           "n't",
                                                                           "cant",
                                                                           "dont",
                                                                           "wont",
                                                                           "couldnt",
                                                                           "shouldnt",
                                                                           "wouldnt",
                                                                           "doesnt",
                                                                           "didnt",
                                                                           "isnt",
                                                                           "hasnt",
                                                                           "havent",
                                                                           "hadnt",
                                                                           "wasnt",
                                                                           "werent",
                                                                           "darent",
                                                                           "neednt",
                                                                       };

            internal static readonly Pair[] NotContradictionReplacementMap =
                                                                             {
                                                                                 new Pair("adnt", "ad not"),
                                                                                 new Pair("an't", "annot"),
                                                                                 new Pair("arent", "are not"),
                                                                                 new Pair("Arent", "Are not"),
                                                                                 new Pair("asnt", "as not"),
                                                                                 new Pair("avent", "ave not"),
                                                                                 new Pair("cant", "cannot"),
                                                                                 new Pair("Cant", "Cannot"),
                                                                                 new Pair("dont", "do not"),
                                                                                 new Pair("Dont", "Do not"),
                                                                                 new Pair("eednt", "eed not"),
                                                                                 new Pair("erent", "ere not"),
                                                                                 new Pair("idnt", "id not"),
                                                                                 new Pair("oesnt", "oes not"),
                                                                                 new Pair("ouldnt", "ould not"),
                                                                                 new Pair("snt", "s not"),
                                                                                 new Pair("wont", "will not"),
                                                                                 new Pair("won't", "will not"),
                                                                                 new Pair("Wont", "Will not"),
                                                                                 new Pair("Won't", "Will not"),
                                                                                 new Pair("n't", " not"),
                                                                             };

            internal static readonly string[] IntentionallyPhrase =
                                                                    {
                                                                        "intentionally",
                                                                        "intentionaly", // be able to detect typos
                                                                        "by intention",
                                                                        "with intention",
                                                                        "by intent",
                                                                        "with intent",
                                                                        "indentionally", // be able to detect typos
                                                                        "indentionaly", // be able to detect typos
                                                                        "by indention", // be able to detect typos
                                                                        "with indention", // be able to detect typos
                                                                        "by indent", // be able to detect typos
                                                                        "with indent", // be able to detect typos
                                                                        "on purpose",
                                                                        "purposely",
                                                                        "purposly", // be able to detect typos
                                                                        "does not matter",
                                                                        "doesn't matter",
                                                                        "doesnt matter", // be able to detect typos
                                                                    };

            internal static readonly string[] ReasoningPhrases = { "because", "reason" };

            internal static readonly string[] LangwordReferences = { "true", "false", "null" };

            internal static readonly HashSet<string> LangwordWrongAttributes = new HashSet<string>
                                                                                   {
                                                                                       XmlTag.Attribute.Langref,
                                                                                       "langowrd", // be able to detect typos
                                                                                       "langwrod", // be able to detect typos
                                                                                       "langwowd", // be able to detect typos
                                                                                   };

            internal static readonly string[] TryWords = { "Try", "Tries" };
            internal static readonly string[] ReturnWords = { "Return", "Returns" };
            internal static readonly string[] ActionTerms = { "action", "Action", "function", "Function", "func", "Func" };
            internal static readonly string[] IdTerms = IdTerm.WithDelimiters();
            internal static readonly string[] IdentTerms = IdentTerm.WithDelimiters();
            internal static readonly string[] InfoTerms = InfoTerm.WithDelimiters();
            internal static readonly string[] EventArgsTermsWithDelimiters = new[] { "event args", "event arg" }.WithDelimiters();
            internal static readonly string[] FindTerms = { "to find", "to inspect for", "to look for", "to test for" };
            internal static readonly string[] FlagTermsWithDelimiters = new[] { "flag", "flags" }.WithDelimiters();
            internal static readonly string[] Guids = { "guid", "Guid", "GUID" };
            internal static readonly string[] GuidTermsWithDelimiters = Guids.WithDelimiters();

            internal static readonly string[] InstanceOfPhrases =
                                                                  {
                                                                      "An instance of ",
                                                                      "an instance of ",
                                                                      "A instance of ",
                                                                      "a instance of ",
                                                                      "The instance of ",
                                                                      "the instance of ",
                                                                      "An object of ",
                                                                      "an object of ",
                                                                      "A object of ",
                                                                      "a object of ",
                                                                      "The object of ",
                                                                      "the object of ",
                                                                      "An instance if ",
                                                                      "an instance if ",
                                                                      "A instance if ",
                                                                      "a instance if ",
                                                                      "The instance if ",
                                                                      "the instance if ",
                                                                  };

            internal static readonly string[] EnumMemberWrongStartingWords =
                                                                             {
                                                                                 "Defines",
                                                                                 "Indicates",
                                                                                 "Represents",
                                                                                 "Specifies",
                                                                                 "Enum",
                                                                             };
        }

        internal static class XmlTag
        {
            internal const string C = "c";
            internal const string Code = "code";
            internal const string Description = "description";
            internal const string Example = "example";
            internal const string Exception = "exception";
            internal const string Include = "include";
            internal const string Inheritdoc = "inheritdoc";
            internal const string Item = "item";
            internal const string List = "list";
            internal const string ListHeader = "listheader";
            internal const string Note = "note";
            internal const string Overloads = "overloads";
            internal const string Para = "para";
            internal const string Param = "param";
            internal const string ParamRef = "paramref";
            internal const string Permission = "permission";
            internal const string Remarks = "remarks";
            internal const string Response = "response"; // used e.g. for Swagger
            internal const string Returns = "returns";
            internal const string See = "see";
            internal const string SeeAlso = "seealso";
            internal const string Summary = "summary";
            internal const string Term = "term";
            internal const string TypeParam = "typeparam";
            internal const string TypeParamRef = "typeparamref";
            internal const string Value = "value";

            internal static class Attribute
            {
                internal const string Cref = "cref";
                internal const string Langword = "langword";
                internal const string Langref = "langref";
                internal const string Name = "name";
                internal const string Type = "type";
            }

            internal static class ListType
            {
                internal const string Bullet = "bullet";
                internal const string Number = "number";
                internal const string Table = "table";
            }
        }

        internal static class DependencyProperty
        {
            internal const string FieldSuffix = "Property";
            internal const string TypeName = "DependencyProperty";
            internal const string FullyQualifiedTypeName = "System.Windows." + TypeName;

            internal const string Register = TypeName + ".Register";
            internal const string RegisterAttached = TypeName + ".RegisterAttached";
            internal const string RegisterReadOnly = TypeName + ".RegisterReadOnly";
            internal const string RegisterAttachedReadOnly = TypeName + ".RegisterAttachedReadOnly";
        }

        internal static class DependencyPropertyKey
        {
            internal const string FieldSuffix = "Key";
            internal const string TypeName = "DependencyPropertyKey";
            internal const string FullyQualifiedTypeName = "System.Windows." + TypeName;

            internal const string DependencyProperty = "DependencyProperty";
        }

        internal static class EventManager
        {
            internal const string FieldSuffix = "Event";
            internal const string TypeName = "EventManager";
            internal const string FullyQualifiedTypeName = "System.Windows." + TypeName;

            internal const string RegisterRoutedEvent = TypeName + ".RegisterRoutedEvent";
        }

        internal static class LambdaIdentifiers
        {
            internal const string Default = "_";
            internal const string FallbackUnderscores2 = "__";
            internal const string FallbackUnderscores3 = "___";
            internal const string FallbackUnderscores4 = "____";
            internal const string Fallback0 = "_0";
            internal const string Fallback1 = "_1";
            internal const string Fallback2 = "_2";
            internal const string Fallback3 = "_3";
            internal const string Fallback4 = "_4";
            internal const string Fallback5 = "_5";
        }

        internal static class Names
        {
            internal const string Create = "Create";
            internal const string Factory = "Factory";

            internal const string DefaultPropertyParameterName = "value";

            internal const string IMultiValueConverter = "IMultiValueConverter";
            internal const string IMultiValueConverterFullName = "System.Windows.Data.IMultiValueConverter";

            internal const string IValueConverter = "IValueConverter";
            internal const string IValueConverterFullName = "System.Windows.Data.IValueConverter";

            internal static readonly string[] DefaultPropertyParameterNames = { DefaultPropertyParameterName };

            internal static readonly ISet<string> FlagsAttributeNames = new HashSet<string>
                                                                            {
                                                                                "Flags",
                                                                                nameof(FlagsAttribute),
                                                                            };

            internal static readonly ISet<string> LinqMethodNames = typeof(Enumerable).GetMethods()
                                                                                      .ToHashSet(_ => _.Name)
                                                                                      .Except(nameof(Equals), nameof(ToString), nameof(GetHashCode), nameof(GetType));

            internal static readonly ISet<string> GeneratedAttributeNames = new HashSet<string>
                                                                                {
                                                                                    "CompilerGenerated",
                                                                                    "CompilerGeneratedAttribute",
                                                                                    "DebuggerNonUserCode",
                                                                                    "DebuggerNonUserCodeAttribute",
                                                                                    "GeneratedCode",
                                                                                    "GeneratedCodeAttribute",
                                                                                };

            internal static readonly ISet<string> TestMethodAttributeNames = new HashSet<string>
                                                                                 {
                                                                                     "Test",
                                                                                     "TestAttribute",
                                                                                     "TestCase",
                                                                                     "TestCaseAttribute",
                                                                                     "TestCaseSource",
                                                                                     "TestCaseSourceAttribute",
                                                                                     "Theory",
                                                                                     "TheoryAttribute",
                                                                                     "Fact",
                                                                                     "FactAttribute",
                                                                                     "TestMethod",
                                                                                     "TestMethodAttribute",
                                                                                 };

            internal static readonly ISet<string> TestClassAttributeNames = new HashSet<string>
                                                                                {
                                                                                    "TestFixture",
                                                                                    "TestFixtureAttribute",
                                                                                    "TestClass",
                                                                                    "TestClassAttribute",
                                                                                };

            internal static readonly ISet<string> TestSetupAttributeNames = new HashSet<string>
                                                                                {
                                                                                    "SetUp",
                                                                                    "SetUpAttribute",
                                                                                    "TestInitialize",
                                                                                    "TestInitializeAttribute",
                                                                                };

            internal static readonly ISet<string> TestTearDownAttributeNames = new HashSet<string>
                                                                                   {
                                                                                       "TearDown",
                                                                                       "TearDownAttribute",
                                                                                       "TestCleanup",
                                                                                       "TestCleanupAttribute",
                                                                                   };

            internal static readonly ISet<string> TestOneTimeSetupAttributeNames = new HashSet<string>
                                                                                       {
                                                                                           "OneTimeSetUp",
                                                                                           "OneTimeSetUpAttribute",
                                                                                           "TestFixtureSetUp", // deprecated NUnit 2.6
                                                                                           "TestFixtureSetUpAttribute", // deprecated NUnit 2.6
                                                                                       };

            internal static readonly ISet<string> TestOneTimeTearDownAttributeNames = new HashSet<string>
                                                                                          {
                                                                                              "OneTimeTearDown",
                                                                                              "OneTimeTearDownAttribute",
                                                                                              "TestFixtureTearDown", // deprecated NUnit 2.6
                                                                                              "TestFixtureTearDownAttribute", // deprecated NUnit 2.6
                                                                                          };

            internal static readonly ISet<string> ImportAttributeNames = new HashSet<string>
                                                                             {
                                                                                 "Import",
                                                                                 "ImportAttribute",
                                                                                 "ImportMany",
                                                                                 "ImportManyAttribute",
                                                                             };

            internal static readonly ISet<string> ImportingConstructorAttributeNames = new HashSet<string>
                                                                                           {
                                                                                               "ImportingConstructor",
                                                                                               "ImportingConstructorAttribute",
                                                                                           };

            internal static readonly IEnumerable<string> TypeUnderTestRawFieldNames = new[]
                                                                                          {
                                                                                              "ObjectUnderTest",
                                                                                              "objectUnderTest",
                                                                                              "ObjectToTest",
                                                                                              "objectToTest",
                                                                                              "SubjectToTest",
                                                                                              "subjectToTest",
                                                                                              "SubjectUnderTest",
                                                                                              "subjectUnderTest",
                                                                                              "Sut",
                                                                                              "sut",
                                                                                              "UnitToTest",
                                                                                              "unitToTest",
                                                                                              "UnitUnderTest",
                                                                                              "unitUnderTest",
                                                                                              "Uut",
                                                                                              "uut",
                                                                                              "TestCandidate",
                                                                                              "testCandidate",
                                                                                              "TestObject",
                                                                                              "testObject",
                                                                                              "candidateToTest",
                                                                                              "candidateUnderTest",
                                                                                          };

            internal static readonly ISet<string> TypeUnderTestFieldNames = Markers.FieldPrefixes.SelectMany(_ => TypeUnderTestRawFieldNames, (prefix, name) => prefix + name).ToHashSet();

            internal static readonly ISet<string> TypeUnderTestPropertyNames = new HashSet<string>
                                                                                   {
                                                                                       "ObjectUnderTest",
                                                                                       "ObjectToTest",
                                                                                       "Sut",
                                                                                       "SuT",
                                                                                       "SUT",
                                                                                       "SubjectToTest",
                                                                                       "SubjectUnderTest",
                                                                                       "UnitToTest",
                                                                                       "UnitUnderTest",
                                                                                       "Uut",
                                                                                       "UuT",
                                                                                       "UUT",
                                                                                       "TestCandidate",
                                                                                       "TestObject",
                                                                                       "Testee",
                                                                                   };

            internal static readonly ISet<string> TypeUnderTestMethodNames = new[] { "Create", "Get" }.SelectMany(_ => TypeUnderTestPropertyNames, (prefix, name) => prefix + name).ToHashSet();

            internal static readonly ISet<string> TypeUnderTestVariableNames = new HashSet<string>
                                                                                   {
                                                                                       "objectUnderTest",
                                                                                       "objectToTest",
                                                                                       "sut",
                                                                                       "subjectToTest",
                                                                                       "subjectUnderTest",
                                                                                       "unitToTest",
                                                                                       "unitUnderTest",
                                                                                       "uut",
                                                                                       "testCandidate",
                                                                                       "testObject",
                                                                                       "testee",
                                                                                       "candidateToTest",
                                                                                       "candidateUnderTest",
                                                                                   };

            internal static readonly ISet<string> ObjectUnderTestNames = Enumerable.Empty<string>()
                                                                                   .Concat(TypeUnderTestFieldNames)
                                                                                   .Concat(TypeUnderTestVariableNames)
                                                                                   .Concat(TypeUnderTestPropertyNames)
                                                                                   .ToHashSet();

            internal static readonly ISet<string> AssertionTypes = new HashSet<string>
                                                                       {
                                                                           "Assert",
                                                                           "CollectionAssert",
                                                                           "DirectoryAssert",
                                                                           "FileAssert",
                                                                           "StringAssert",
                                                                       };

            internal static readonly ISet<string> AssertionNamespaces = new HashSet<string>
                                                                            {
                                                                                "NUnit.Framework",
                                                                                "NUnit.Framework.Constraints",
                                                                                "NUnit.Framework.Legacy",
                                                                            };

            internal static readonly ISet<string> KnownWindowsEventHandlers = new HashSet<string>
                                                                                  {
                                                                                      "AnnotationAuthorChangedEventHandler",
                                                                                      "AnnotationResourceChangedEventHandler",
                                                                                      "BamlLocalizerErrorNotifyEventHandler",
                                                                                      "BindingCompleteEventHandler",
                                                                                      "BindingManagerDataErrorEventHandler",
                                                                                      "CacheVirtualItemsEventHandler",
                                                                                      "CleanUpVirtualizedItemEventHandler",
                                                                                      "ColumnClickEventHandler",
                                                                                      "ColumnReorderedEventHandler",
                                                                                      "ColumnWidthChangedEventHandler",
                                                                                      "ColumnWidthChangingEventHandler",
                                                                                      "ContentsResizedEventHandler",
                                                                                      "ContextMenuEventHandler",
                                                                                      "ControlEventHandler",
                                                                                      "ConvertEventHandler",
                                                                                      "DataGridSortingEventHandler",
                                                                                      "DataGridViewAutoSizeColumnModeEventHandler",
                                                                                      "DataGridViewAutoSizeColumnsModeEventHandler",
                                                                                      "DataGridViewAutoSizeModeEventHandler",
                                                                                      "DataGridViewBindingCompleteEventHandler",
                                                                                      "DataGridViewCellCancelEventHandler",
                                                                                      "DataGridViewCellContextMenuStripNeededEventHandler",
                                                                                      "DataGridViewCellErrorTextNeededEventHandler",
                                                                                      "DataGridViewCellEventHandler",
                                                                                      "DataGridViewCellFormattingEventHandler",
                                                                                      "DataGridViewCellMouseEventHandler",
                                                                                      "DataGridViewCellPaintingEventHandler",
                                                                                      "DataGridViewCellParsingEventHandler",
                                                                                      "DataGridViewCellStateChangedEventHandler",
                                                                                      "DataGridViewCellStyleContentChangedEventHandler",
                                                                                      "DataGridViewCellToolTipTextNeededEventHandler",
                                                                                      "DataGridViewCellValidatingEventHandler",
                                                                                      "DataGridViewCellValueEventHandler",
                                                                                      "DataGridViewColumnDividerDoubleClickEventHandler",
                                                                                      "DataGridViewColumnEventHandler",
                                                                                      "DataGridViewColumnStateChangedEventHandler",
                                                                                      "DataGridViewDataErrorEventHandler",
                                                                                      "DataGridViewEditingControlShowingEventHandler",
                                                                                      "DataGridViewRowCancelEventHandler",
                                                                                      "DataGridViewRowContextMenuStripNeededEventHandler",
                                                                                      "DataGridViewRowDividerDoubleClickEventHandler",
                                                                                      "DataGridViewRowErrorTextNeededEventHandler",
                                                                                      "DataGridViewRowEventHandler",
                                                                                      "DataGridViewRowHeightInfoNeededEventHandler",
                                                                                      "DataGridViewRowHeightInfoPushedEventHandler",
                                                                                      "DataGridViewRowPostPaintEventHandler",
                                                                                      "DataGridViewRowPrePaintEventHandler",
                                                                                      "DataGridViewRowsAddedEventHandler",
                                                                                      "DataGridViewRowsRemovedEventHandler",
                                                                                      "DataGridViewRowStateChangedEventHandler",
                                                                                      "DataGridViewSortCompareEventHandler",
                                                                                      "DateBoldEventHandler",
                                                                                      "DateRangeEventHandler",
                                                                                      "DpiChangedEventHandler",
                                                                                      "DragCompletedEventHandler",
                                                                                      "DragDeltaEventHandler",
                                                                                      "DragEventHandler",
                                                                                      "DragStartedEventHandler",
                                                                                      "DrawItemEventHandler",
                                                                                      "DrawListViewColumnHeaderEventHandler",
                                                                                      "DrawListViewItemEventHandler",
                                                                                      "DrawListViewSubItemEventHandler",
                                                                                      "DrawToolTipEventHandler",
                                                                                      "DrawTreeNodeEventHandler",
                                                                                      "ExitEventHandler",
                                                                                      "FilterEventHandler",
                                                                                      "FormClosedEventHandler",
                                                                                      "FormClosingEventHandler",
                                                                                      "FragmentNavigationEventHandler",
                                                                                      "GetPageRootCompletedEventHandler",
                                                                                      "GiveFeedbackEventHandler",
                                                                                      "HelpEventHandler",
                                                                                      "HtmlElementErrorEventHandler",
                                                                                      "HtmlElementEventHandler",
                                                                                      "InitializingNewItemEventHandler",
                                                                                      "InkCanvasGestureEventHandler",
                                                                                      "InkCanvasSelectionChangingEventHandler",
                                                                                      "InkCanvasSelectionEditingEventHandler",
                                                                                      "InkCanvasStrokeCollectedEventHandler",
                                                                                      "InkCanvasStrokeErasingEventHandler",
                                                                                      "InkCanvasStrokesReplacedEventHandler",
                                                                                      "InputLanguageChangedEventHandler",
                                                                                      "InputLanguageChangingEventHandler",
                                                                                      "InvalidateEventHandler",
                                                                                      "ItemChangedEventHandler",
                                                                                      "ItemCheckedEventHandler",
                                                                                      "ItemCheckEventHandler",
                                                                                      "ItemDragEventHandler",
                                                                                      "ItemsChangedEventHandler",
                                                                                      "KeyEventHandler",
                                                                                      "KeyPressEventHandler",
                                                                                      "LabelEditEventHandler",
                                                                                      "LayoutEventHandler",
                                                                                      "LinkClickedEventHandler",
                                                                                      "LinkLabelLinkClickedEventHandler",
                                                                                      "ListControlConvertEventHandler",
                                                                                      "ListViewItemMouseHoverEventHandler",
                                                                                      "ListViewItemSelectionChangedEventHandler",
                                                                                      "ListViewVirtualItemsSelectionRangeChangedEventHandler",
                                                                                      "LoadCompletedEventHandler",
                                                                                      "MaskInputRejectedEventHandler",
                                                                                      "MeasureItemEventHandler",
                                                                                      "MouseEventHandler",
                                                                                      "NavigatedEventHandler",
                                                                                      "NavigateEventHandler",
                                                                                      "NavigatingCancelEventHandler",
                                                                                      "NavigationFailedEventHandler",
                                                                                      "NavigationProgressEventHandler",
                                                                                      "NavigationStoppedEventHandler",
                                                                                      "NodeLabelEditEventHandler",
                                                                                      "PaintEventHandler",
                                                                                      "PopupEventHandler",
                                                                                      "PreviewKeyDownEventHandler",
                                                                                      "PropertyTabChangedEventHandler",
                                                                                      "PropertyValueChangedEventHandler",
                                                                                      "QueryAccessibilityHelpEventHandler",
                                                                                      "QueryContinueDragEventHandler",
                                                                                      "QuestionEventHandler",
                                                                                      "RequestBringIntoViewEventHandler",
                                                                                      "RequestNavigateEventHandler",
                                                                                      "RetrieveVirtualItemEventHandler",
                                                                                      "ScrollChangedEventHandler",
                                                                                      "ScrollEventHandler",
                                                                                      "SearchForVirtualItemEventHandler",
                                                                                      "SelectedCellsChangedEventHandler",
                                                                                      "SelectedGridItemChangedEventHandler",
                                                                                      "SelectionChangedEventHandler",
                                                                                      "SessionEndingCancelEventHandler",
                                                                                      "SizeChangedEventHandler",
                                                                                      "SplitterCancelEventHandler",
                                                                                      "SplitterEventHandler",
                                                                                      "StartupEventHandler",
                                                                                      "StatusBarDrawItemEventHandler",
                                                                                      "StatusBarPanelClickEventHandler",
                                                                                      "StoreContentChangedEventHandler",
                                                                                      "TabControlCancelEventHandler",
                                                                                      "TabControlEventHandler",
                                                                                      "TableLayoutCellPaintEventHandler",
                                                                                      "TextChangedEventHandler",
                                                                                      "ToolBarButtonClickEventHandler",
                                                                                      "ToolStripArrowRenderEventHandler",
                                                                                      "ToolStripContentPanelRenderEventHandler",
                                                                                      "ToolStripDropDownClosedEventHandler",
                                                                                      "ToolStripDropDownClosingEventHandler",
                                                                                      "ToolStripGripRenderEventHandler",
                                                                                      "ToolStripItemClickedEventHandler",
                                                                                      "ToolStripItemEventHandler",
                                                                                      "ToolStripItemImageRenderEventHandler",
                                                                                      "ToolStripItemRenderEventHandler",
                                                                                      "ToolStripItemTextRenderEventHandler",
                                                                                      "ToolStripPanelRenderEventHandler",
                                                                                      "ToolStripRenderEventHandler",
                                                                                      "ToolStripSeparatorRenderEventHandler",
                                                                                      "ToolTipEventHandler",
                                                                                      "TreeNodeMouseClickEventHandler",
                                                                                      "TreeNodeMouseHoverEventHandler",
                                                                                      "TreeViewCancelEventHandler",
                                                                                      "TreeViewEventHandler",
                                                                                      "TypeValidationEventHandler",
                                                                                      "UICuesEventHandler",
                                                                                      "UpDownEventHandler",
                                                                                      "WebBrowserDocumentCompletedEventHandler",
                                                                                      "WebBrowserNavigatedEventHandler",
                                                                                      "WebBrowserNavigatingEventHandler",
                                                                                      "WebBrowserProgressChangedEventHandler",
                                                                                      "WritingCancelledEventHandler",
                                                                                      "WritingCompletedEventHandler",
                                                                                      "WritingPrintTicketRequiredEventHandler",
                                                                                      "WritingProgressChangedEventHandler",
                                                                                  };
        }

        internal static class AnalyzerCodeFixSharedData
        {
            internal const string BetterName = nameof(BetterName);

            internal const string AddSpaceAfter = nameof(AddSpaceAfter);
            internal const string AddSpaceBefore = nameof(AddSpaceBefore);

            internal const string NoLineBefore = nameof(NoLineBefore);
            internal const string NoLineAfter = nameof(NoLineAfter);

            internal const string LineNumber = nameof(LineNumber);
            internal const string CharacterPosition = nameof(CharacterPosition);
            internal const string Spaces = nameof(Spaces);
            internal const string AdditionalSpaces = nameof(AdditionalSpaces);

            internal const string StartingPhrase = nameof(StartingPhrase);
            internal const string EndingPhrase = nameof(EndingPhrase);
            internal const string Phrase = nameof(Phrase);

            internal const string IsBoolean = nameof(IsBoolean);

            internal const string DefaultSeeLangwordValue = nameof(DefaultSeeLangwordValue);
            internal const string DefaultSeeCrefValue = nameof(DefaultSeeCrefValue);
            internal const string DefaultCodeValue = nameof(DefaultCodeValue);

            internal const string TextKey = nameof(TextKey);
            internal const string TextReplacementKey = nameof(TextReplacementKey);

            internal const string GetPropertyName = nameof(GetPropertyName); // Cinch, use nameof()
            internal const string CreateArgs = nameof(CreateArgs); // Cinch, use new PropertyChanged(nameof())
            internal const string PropertyTypeName = nameof(PropertyTypeName);
            internal const string PropertyName = nameof(PropertyName);

            internal const string Position = nameof(Position);
            internal const string IsFlagged = nameof(IsFlagged);

            internal const string ParameterValue = nameof(ParameterValue);

            internal const string Marker = nameof(Marker);
        }
    }
}
