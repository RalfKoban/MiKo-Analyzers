using System;
using System.Linq;

namespace MiKoSolutions.Analyzers
{
    public static class Constants
    {
        internal const string AsyncSuffix = "Async";
        internal const string DependencyPropertyFieldSuffix = "Property";
        internal const string DependencyPropertyKeyFieldSuffix = "Key";
        internal static readonly string[] WhiteSpaces = { " ", "\t", "\r", "\n" };

        internal static class Markers
        {
            internal static readonly string[] Entities = { "Model", "Models", "model", "models" };
            internal static readonly string[] ViewModels = { "ViewModel", "ViewModels", "viewModel", "viewModels" };
            internal static readonly string[] SpecialModels = { "Modeless", "modeless", "ModeLess", "modeLess", "semanticModel", "SemanticModel" };
            internal static readonly string[] Collections = { "List", "Dictionary", "ObservableCollection", "Collection", "Array", "HashSet", "list", "dictionary", "observableCollection", "collection", "array", "hashSet" };
            internal static readonly string[] Symbols = { "T:", "P:", "M:", "F:", "!:" };
            internal static readonly string[] SymbolsAndLineBreaks = Symbols.Concat(new[] { Environment.NewLine }).ToArray();
            internal static readonly string[] Requirements = { "Must", "Need", "Shall", "Should", "Will", "Would" };
        }

        internal static class Comments
        {
            internal static readonly string[] Delimiters = { " ", ".", ",", ";", ":", "!", "?" };
            internal static readonly string[] UnusedPhrase = { "Unused.", "Unused" };

            internal static readonly string[] EventSourcePhrase = new[] { "The source of the event.", "The source of the event" }.Concat(UnusedPhrase).Distinct().ToArray();

            internal static readonly string[] SeeStartingPhrase = { "<see cref=", "<seealso cref=", "see <see cref=", "see <seealso cref=", "seealso <see cref=", "seealso <seealso cref=" };
            internal static readonly string[] SeeEndingPhrase = { "/>", "/>.", "/see>", "/see>.", "/seealso>", "/seealso>." };

            internal static readonly string[] FieldStartingPhrase = { "A ", "An ", "The " };

            internal static readonly string[] ParameterStartingPhrase = { "A ", "An ", "The " };
            internal static readonly string[] OutParameterStartingPhrase = { "On successful return, contains " };
            internal static readonly string[] EnumParameterStartingPhrase = { "One of the enumeration members that specifies " };
            internal static readonly string[] CancellationTokenParameterPhrase = { "The token to monitor for cancellation requests." };

            internal static readonly string[] MeaninglessStartingPhrase =
                {
                    "A ", "An ", "Does implement ", "For ", "Implement ", "Implements ", "Is ", "This ", "That ", "The ", "To ", "Uses ", "Used ", "Which ", "Called ",
                    "Base", "Class", "Interface", "Method", "Field", "Property", "Event", "Constructor", "Ctor", "Delegate", "Action", "Func", "Factory", "Creator", "Builder", "Entity", "Model", "ViewModel", "Command", "Converter",
                    "Interaction logic",
                };

            internal static readonly string[] MeaninglessPhrase = { "does implement", "implements", "that is called", "that is used", "used for", "used to", "which is called", "which is used", };

            internal static readonly string[] MeaninglessFieldStartingPhrase = MeaninglessStartingPhrase.Except(FieldStartingPhrase).ToArray();

            internal static readonly string[] ReturnTypeStartingPhrase = { "A ", "An ", "The " };

            internal static readonly string[] GenericTaskReturnTypeStartingPhrase =
                {
                    "A task that represents the asynchronous operation. The value of the <see cref=\"Task{TResult}.Result\" /> parameter contains ", // this is just to have a proposal how to optimize
                    "A task that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task`1.Result\" /> parameter contains ",
                    "A task that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task`1.Result\"/> parameter contains ",
                    "A <see cref=\"System.Threading.Tasks.Task`1\" /> that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task`1.Result\" /> parameter contains ",
                    "A <see cref=\"System.Threading.Tasks.Task`1\" /> that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task`1.Result\"/> parameter contains ",
                    "A <see cref=\"System.Threading.Tasks.Task`1\"/> that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task`1.Result\" /> parameter contains ",
                    "A <see cref=\"System.Threading.Tasks.Task`1\"/> that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task`1.Result\"/> parameter contains ",
                };

            internal static readonly string[] NonGenericTaskReturnTypePhrase =
                {
                    "A task that represents the asynchronous operation.",
                    "A <see cref=\"System.Threading.Tasks.Task\"/> that represents the asynchronous operation.",
                    "A <see cref=\"System.Threading.Tasks.Task\" /> that represents the asynchronous operation.",
                };

            internal static readonly string[] BooleanReturnTypeStartingPhrase =
                {
                    "<see langword=\"true\"/> if ",
                    "<see langword=\"true\" /> if ",
                };

            internal static readonly string[] BooleanReturnTypeEndingPhrase =
                {
                    "; otherwise, <see langword=\"false\"/>.",
                    "; otherwise, <see langword=\"false\" />.",
                };

            internal static readonly string[] BooleanParameterStartingPhrase =
                {
                    "<see langword=\"true\"/> to ",
                    "<see langword=\"true\" /> to ",
                };

            internal static readonly string[] BooleanParameterEndingPhrase =
                {
                    "; otherwise, <see langword=\"false\"/>.",
                    "; otherwise, <see langword=\"false\" />.",
                };

            internal static readonly string[] BooleanTaskReturnTypeStartingPhrase =
                {
                    "A task that will complete with a result of <see langword=\"true\"/> if ",
                    "A task that will complete with a result of <see langword=\"true\" /> if ",
                };

            internal static readonly string[] BooleanTaskReturnTypeEndingPhrase =
                {
                    ", otherwise with a result of <see langword=\"false\"/>.",
                    ", otherwise with a result of <see langword=\"false\" />.",
                };

            internal static readonly string[] StringReturnTypeStartingPhrase =
                {
                    "A <see cref=\"string\" /> that contains ", // this is just to have a proposal how to optimize
                    "A <see cref=\"string\"/> that contains ",
                    "A <see cref=\"System.String\" /> that contains ",
                    "A <see cref=\"System.String\"/> that contains ",
                    "A <see cref=\"string\" /> that represents ",
                    "A <see cref=\"string\"/> that represents ",
                    "A <see cref=\"System.String\" /> that represents ",
                    "A <see cref=\"System.String\"/> that represents ",
                };

            internal static readonly string[] StringTaskReturnTypeStartingPhrase =
                {
                    "A task that represents the asynchronous operation. The <see cref=\"Task{TResult}.Result\" /> property on the task object returns a <see cref=\"string\" /> that contains ", // this is just to have a proposal how to optimize
                    "A task that represents the asynchronous operation. The <see cref=\"System.Threading.Tasks.Task`1.Result\"/> property on the task object returns a <see cref=\"System.String\"/> that contains ",
                    "A task that represents the asynchronous operation. The <see cref=\"System.Threading.Tasks.Task`1.Result\"/> property on the task object returns a <see cref=\"System.String\" /> that contains ",
                    "A task that represents the asynchronous operation. The <see cref=\"System.Threading.Tasks.Task`1.Result\" /> property on the task object returns a <see cref=\"System.String\"/> that contains ",
                    "A task that represents the asynchronous operation. The <see cref=\"System.Threading.Tasks.Task`1.Result\" /> property on the task object returns a <see cref=\"System.String\" /> that contains ",
                };

            internal static readonly string[] EnumReturnTypeStartingPhrase = { "The enumerated constant that is the ", };

            internal static readonly string[] EnumTaskReturnTypeStartingPhrase = GenericTaskReturnTypeStartingPhrase.Select(_ => _ + "the enumerated constant that is the ").ToArray();

            internal static readonly string[] EnumerableReturnTypeStartingPhrase =
                {
                    "A collection of ",
                    "A <see cref=\"{0}\"/> that contains ",
                    "A <see cref=\"{0}\" /> that contains ",
                    "An <see cref=\"{0}\"/> that contains ",
                    "An <see cref=\"{0}\" /> that contains ",
                };

            internal static readonly string[] EnumerableTaskReturnTypeStartingPhrase = GenericTaskReturnTypeStartingPhrase.Select(_ => _ + "a collection of ").ToArray();

            internal static readonly string[] ArrayReturnTypeStartingPhrase = { "An array of ", "The array of " };

            internal static readonly string[] ArrayTaskReturnTypeStartingPhrase = GenericTaskReturnTypeStartingPhrase.Select(_ => _ + "an array of ").ToArray();

            internal static readonly string[] DependencyPropertyFieldSummaryPhrase =
                {
                    "Identifies the <see cref=\"{0}\"/> dependency property.",
                    "Identifies the <see cref=\"{0}\" /> dependency property.",
                };

            internal static readonly string[] DependencyPropertyFieldValuePhrase =
                {
                    "The identifier for the <see cref=\"{0}\"/> dependency property.",
                    "The identifier for the <see cref=\"{0}\" /> dependency property.",
                };

            internal const string SealedClassPhrase = "This class cannot be inherited.";

            internal const string EventHandlerSummaryStartingPhrase = "Handles the ";

            internal static readonly string[] EventHandlerSummaryPhrase =
                {
                    EventHandlerSummaryStartingPhrase + "<see cref=\"{0}\"/> event",
                    EventHandlerSummaryStartingPhrase + "<see cref=\"{0}\" /> event",
                };

            internal const string DefaultCrefPhrase = "The default is <see cref=\"{0}\"/>.";

            internal static readonly string[] DefaultCrefPhrases =
                {
                    "The default is <see cref=\"{0}\"/>.",
                    "The default is <see cref=\"{0}\" />.",
                };

            internal const string DefaultLangwordPhrase = "The default is <see langword=\"{0}\"/>.";

            internal static readonly string[] DefaultBooleanLangwordPhrases =
                {
                    "The default is <see langword=\"true\"/>.",
                    "The default is <see langword=\"false\"/>.",
                    "The default is <see langword=\"true\" />.",
                    "The default is <see langword=\"false\" />.",
                };

            private static readonly string[] InvalidSummaryCrefXmlTags =
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
                    XmlTag.TypeParamRef,
                    XmlTag.Value,
                };

            internal static readonly string[] InvalidSummaryCrefPhrases = Enumerable.Empty<string>()
                                                                                    .Concat(InvalidSummaryCrefXmlTags.Select(_ => XmlElementStartingTag + _ + " "))
                                                                                    .Concat(InvalidSummaryCrefXmlTags.Select(_ => XmlElementStartingTag + _ + "/"))
                                                                                    .Concat(InvalidSummaryCrefXmlTags.Select(_ => XmlElementStartingTag + _ + ">"))
                                                                                    .ToArray();

            internal const string ExceptionTypeSummaryStartingPhrase = "The exception that is thrown when ";

            internal static readonly string[] ExceptionCtorSummaryStartingPhrase =
                {
                    "Initializes a new instance of the <see cref=\"{0}\"/> class",
                    "Initializes a new instance of the <see cref=\"{0}\" /> class",
                };

            internal const string ExceptionCtorMessageParamSummaryContinueingPhrase = " with a specified error message";
            internal const string ExceptionCtorExceptionParamSummaryContinueingPhrase = " and a reference to the inner exception that is the cause of this exception";
            internal const string ExceptionCtorSerializationParamSummaryContinueingPhrase = " with serialized data";
            internal const string ExceptionCtorSerializationParamRemarksPhrase = "This constructor is invoked during deserialization to reconstitute the exception object transmitted over a stream.";

            internal static readonly string[] ExceptionCtorSerializationInfoParamPhrase = { "The object that holds the serialized object data." };
            internal static readonly string[] ExceptionCtorStreamingContextParamPhrase = { "The contextual information about the source or destination." };
            internal static readonly string[] ExceptionCtorMessageParamPhrase = { "The error message that explains the reason for the exception." };

            internal static readonly string[] ExceptionCtorExceptionParamPhrase =
                {
                    @"The exception that is the cause of the current exception. If the <paramref name=""innerException""/> parameter is not <see langword=""null""/>, the current exception is raised in a <b>catch</b> block that handles the inner exception.",
                    @"The exception that is the cause of the current exception. If the <paramref name=""innerException"" /> parameter is not <see langword=""null""/>, the current exception is raised in a <b>catch</b> block that handles the inner exception.",
                    @"The exception that is the cause of the current exception. If the <paramref name=""innerException"" /> parameter is not <see langword=""null"" />, the current exception is raised in a <b>catch</b> block that handles the inner exception.",
                    @"The exception that is the cause of the current exception. If the <paramref name=""innerException""/> parameter is not <see langword=""null"" />, the current exception is raised in a <b>catch</b> block that handles the inner exception.",
                };

            internal const string FactorySummaryPhrase = "Provides support for creating ";

            internal static readonly string[] FactoryCreateMethodSummaryStartingPhrase =
                {
                    "Creates a new instance of the <see cref=\"{0}\"/> type with ",
                    "Creates a new instance of the <see cref=\"{0}\" /> type with ",
                };

            internal static readonly string[] FactoryCreateCollectionMethodSummaryStartingPhrase =
                {
                    "Creates a collection of new instances of the <see cref=\"{0}\"/> type with ",
                    "Creates a collection of new instances of the <see cref=\"{0}\" /> type with ",
                };

            internal const string AsynchrounouslyStartingPhrase = "Asynchronously ";

            internal const string ParamRefBeginningPhrase = @"<paramref name=""{0}""";

            internal static readonly string[] ExtensionMethodClassStartingPhrase =
                {
                    "Provides a set of <see langword=\"static\"/> methods for ",
                    "Provides a set of <see langword=\"static\" /> methods for ",
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

            internal const string ObjectDisposedExceptionEndingPhrase = " has been disposed.";

            internal const string ObjectDisposedExceptionAlternatingEndingPhrase = " has been closed.";

            internal const string ExceptionPhrase = @"<exception cref=""{0}"">";

            internal const string SpecialOrPhrase = "-or-";
            internal static readonly string[] ExceptionSplittingPhrase = { SpecialOrPhrase };
            internal const string ExceptionSplittingParaPhrase = "<para>" + SpecialOrPhrase + "</para>";

            internal static readonly string[] ExceptionForbiddenStartingPhrase =
                {
                    "Thrown ",
                    "Is thrown ",
                    "Gets thrown ",
                    "Will be thrown ",
                    "If ",
                    "In case ",
                    "When ",
                    "Throws ",
                    "Throw ",
                };

            internal const string XmlElementStartingTag = "<";
            internal const string XmlElementEndingTag = "/>";

            internal const string ExampleDefaultPhrase = "The following example demonstrates ";

            internal const string DelegateSummaryStartingPhrase = "Encapsulates a method that ";

            internal const string CommandSummaryStartingPhrase = "Represents a command that can ";

            internal static readonly string[] CommandPropertyGetterSetterSummaryStartingPhrase =
                {
                    @"Gets or sets the <see cref=""ICommand""/> that can ",
                    @"Gets or sets the <see cref=""ICommand"" /> that can ",
                    @"Gets or sets the <see cref=""System.Windows.Input.ICommand""/> that can ",
                    @"Gets or sets the <see cref=""System.Windows.Input.ICommand"" /> that can ",
                };

            internal static readonly string[] CommandPropertyGetterOnlySummaryStartingPhrase =
                {
                    @"Gets the <see cref=""ICommand""/> that can ",
                    @"Gets the <see cref=""ICommand"" /> that can ",
                    @"Gets the <see cref=""System.Windows.Input.ICommand""/> that can ",
                    @"Gets the <see cref=""System.Windows.Input.ICommand"" /> that can ",
                };

            internal static readonly string[] CommandPropertySetterOnlySummaryStartingPhrase =
                {
                    @"Sets the <see cref=""ICommand""/> that can ",
                    @"Sets the <see cref=""ICommand"" /> that can ",
                    @"Sets the <see cref=""System.Windows.Input.ICommand""/> that can ",
                    @"Sets the <see cref=""System.Windows.Input.ICommand"" /> that can ",
                };

            internal const string FieldIsReadOnly = "This field is read-only.";

            internal static readonly string[] AttributeSummaryStartingPhrase =
                {
                    "Specifies ",
                    "Indicates ",
                    "Defines ",
                    "Provides ",
                    "Allows ",
                    "Represents ",
                };

            internal static readonly string ValueConverterSummaryStartingPhrase = "Represents a converter that converts ";
        }

        public static class MaxNamingLengths
        {
            public const int Types = 40;
            public const int Methods = 25;
            public const int Events = 25;
            public const int Properties = 25;
            public const int Parameters = 20;
            public const int Fields = Parameters + 2;
            public const int LocalVariables = 15;
        }

        internal static class XmlTag
        {
            internal const string Code = "code";
            internal const string Example = "example";
            internal const string Exception = "exception";
            internal const string Include = "include";
            internal const string Inheritdoc = "inheritdoc";
            internal const string Note = "note";
            internal const string Overloads = "overloads";
            internal const string Para = "para";
            internal const string Param = "param";
            internal const string ParamRef = "paramref";
            internal const string Permission = "permission";
            internal const string Remarks = "remarks";
            internal const string Returns = "returns";
            internal const string See = "see";
            internal const string SeeAlso = "seealso";
            internal const string Summary = "summary";
            internal const string TypeParam = "typeparam";
            internal const string TypeParamRef = "typeparamref";
            internal const string Value = "value";
        }

        internal static class Invocations
        {
            internal static class DependencyProperty
            {
                internal const string Register = "DependencyProperty.Register";
                internal const string RegisterAttached = "DependencyProperty.RegisterAttached";
                internal const string RegisterReadOnly = "DependencyProperty.RegisterReadOnly";
                internal const string RegisterAttachedReadOnly = "DependencyProperty.RegisterAttachedReadOnly";
            }
        }
    }
}