using System.Linq;

namespace MiKoSolutions.Analyzers
{
    internal static class Constants
    {
        internal static readonly string[] EntityMarkers = { "Model", "Models", "model", "models" };
        internal static readonly string[] ViewModelMarkers = { "ViewModel", "ViewModels", "viewModel", "viewModels" };
        internal static readonly string[] SpecialModelMarkers = { "Modeless", "modeless", "ModeLess", "modeLess" };
        internal static readonly string[] CollectionMarkers = { "List", "Dictionary", "ObservableCollection", "Collection", "Array", "HashSet", "list", "dictionary", "observableCollection", "collection", "array", "hashSet" };
        internal static readonly string[] SymbolMarkers = { "T:", "P:", "M:" };

        internal static class Comments
        {
            internal static readonly string[] UnusedPhrase = { "Unused.", "Unused" };

            internal static readonly string[] EventSourcePhrase = new[] { "The source of the event.", "The source of the event" }.Concat(UnusedPhrase).Distinct().ToArray();

            internal static readonly string[] SeeStartingPhrase = { "<see cref=", "<seealso cref=", "see <see cref=", "see <seealso cref=", "seealso <see cref=", "seealso <seealso cref=" };
            internal static readonly string[] SeeEndingPhrase = { "/>", "/>.", "/see>", "/see>.", "/seealso>", "/seealso>." };

            internal static readonly string[] ParameterStartingPhrase = { "A ", "An ", "The " };
            internal static readonly string[] OutParameterStartingPhrase = { "On successful return, contains " };
            internal static readonly string[] EnumParameterStartingPhrase = { "One of the enumeration members that specifies " };

            internal static readonly string[] MeaninglessTypeStartingPhrase =
                {
                    "A ", "An ", "Does implement ", "For ", "Implement ", "Implements ", "Is ", "This ", "That ", "The ", "To ", "Used ", "Which ",
                    "Class", "Interface", "Factory", "Creator", "Builder", "Entity", "Model", "ViewModel", "Command",
                };

            internal static readonly string[] ReturnTypeStartingPhrase = { "A ", "An ", "The " };

            internal static readonly string[] GenericTaskReturnTypeStartingPhrase =
                {
                    "A task that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task{TResult}.Result\" /> parameter contains ", // this is just to have a proposal how to optimize
                    "A task that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task`1.Result\" /> parameter contains ",
                    "A <see cref=\"System.Threading.Tasks.Task`1\" /> that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task`1.Result\" /> parameter contains ",
                    "A <see cref=\"System.Threading.Tasks.Task`1\"/> that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task`1.Result\" /> parameter contains ",
                };
            internal static readonly string[] NonGenericTaskReturnTypePhrase =
                {
                    "A task that represents the asynchronous operation.",
                    "A <see cref=\"System.Threading.Tasks.Task\" /> that represents the asynchronous operation.",
                    "A <see cref=\"System.Threading.Tasks.Task\"/> that represents the asynchronous operation.",
                };

            internal static readonly string[] BooleanReturnTypeStartingPhrase = { "<see langword=\"true\" /> ", "<see langword=\"true\"/> " };
            internal static readonly string[] BooleanReturnTypeEndingPhrase = { "; otherwise, <see langword=\"false\" />.", "; otherwise, <see langword=\"false\"/>." };
        }
    }
}