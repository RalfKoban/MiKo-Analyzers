using System;
using System.Linq;

namespace MiKoSolutions.Analyzers
{
    internal static class Constants
    {
        internal static readonly string[] EntityMarkers = { "Model", "Models", "model", "models" };
        internal static readonly string[] ViewModelMarkers = { "ViewModel", "ViewModels", "viewModel", "viewModels" };
        internal static readonly string[] SpecialModelMarkers = { "Modeless", "modeless", "ModeLess", "modeLess" };
        internal static readonly string[] CollectionMarkers = { "List", "Dictionary", "ObservableCollection", "Collection", "Array", "HashSet", "list", "dictionary", "observableCollection", "collection", "array", "hashSet" };
        internal static readonly string[] SymbolMarkers = { "T:", "P:", "M:", "F:" };
        internal static readonly string[] SymbolMarkersAndLineBreaks = SymbolMarkers.Concat(new[] { Environment.NewLine + "   " }).ToArray();

        internal static class Comments
        {
            internal static readonly string[] UnusedPhrase = { "Unused.", "Unused" };

            internal static readonly string[] EventSourcePhrase = new[] { "The source of the event.", "The source of the event" }.Concat(UnusedPhrase).Distinct().ToArray();

            internal static readonly string[] SeeStartingPhrase = { "<see cref=", "<seealso cref=", "see <see cref=", "see <seealso cref=", "seealso <see cref=", "seealso <seealso cref=" };
            internal static readonly string[] SeeEndingPhrase = { "/>", "/>.", "/see>", "/see>.", "/seealso>", "/seealso>." };

            internal static readonly string[] FieldStartingPhrase = { "A ", "An ", "The " };

            internal static readonly string[] ParameterStartingPhrase = { "A ", "An ", "The " };
            internal static readonly string[] OutParameterStartingPhrase = { "On successful return, contains " };
            internal static readonly string[] EnumParameterStartingPhrase = { "One of the enumeration members that specifies " };

            internal static readonly string[] MeaninglessStartingPhrase =
                {
                    "A ", "An ", "Does implement ", "For ", "Implement ", "Implements ", "Is ", "This ", "That ", "The ", "To ", "Uses ", "Used ", "Which ", "Called ",
                    "Class", "Interface", "Method", "Field", "Property", "Event", "Constructor", "Ctor", "Factory", "Creator", "Builder", "Entity", "Model", "ViewModel", "Command"
                };

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
                    "A <see cref=\"System.String\"/> that contains ",
                    "A <see cref=\"System.String\" /> that contains ",
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

            internal static readonly string[] EnumerableReturnTypeStartingPhrase = { "A collection of ", };

            internal static readonly string[] EnumerableTaskReturnTypeStartingPhrase = GenericTaskReturnTypeStartingPhrase.Select(_ => _ + "a collection of ").ToArray();

            internal static readonly string[] ArrayReturnTypeStartingPhrase = { "An array of ", };

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

            internal static readonly string SealedClassPhrase = "This class cannot be inherited.";

            internal static readonly string EventHandlerSummaryStartingPhrase = "Handles the ";

            internal static readonly string[] EventHandlerSummaryPhrase =
                {
                    EventHandlerSummaryStartingPhrase + "<see cref=\"{0}\"/> event",
                    EventHandlerSummaryStartingPhrase + "<see cref=\"{0}\" /> event",
                };
        }
    }
}