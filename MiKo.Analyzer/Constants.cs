using System.Linq;

namespace MiKoSolutions.Analyzers
{
    internal static class Constants
    {
        internal static readonly string[] EntityMarkers = { "Model", "Models", "model", "models" };
        internal static readonly string[] ViewModelMarkers = { "ViewModel", "ViewModels", "viewModel", "viewModels" };
        internal static readonly string[] SpecialModelMarkers = { "Modeless", "modeless", "ModeLess", "modeLess" };
        internal static readonly string[] CollectionMarkers = { "List", "Dictionary", "ObservableCollection", "Collection", "Array", "HashSet", "list", "dictionary", "observableCollection", "collection", "array", "hashSet" };


        internal static class Comments
        {
            internal static readonly string[] ParameterStartingPhrase = { "A ", "An ", "The " };
            internal static readonly string[] OutParameterStartingPhrase = { "On successful return, contains " };
            internal static readonly string[] EnumParameterStartingPhrase = { "One of the enumeration members that specifies " };
            internal static readonly string[] MeaninglessTypeStartingPhrase = { "Class that is used to ", "Class used to ", "Class to ", "Implements ", "Used to ", "Model which is used for ", "Viewmodel which is used for ", "Used for ", "The ", "A ", "An ", "Interface that is used to ", "Interface used to ", "Interface to ", "Used to " };
            internal static readonly string[] UnusedPhrase = { "Unused.", "Unused" };
            internal static readonly string[] EventSourcePhrase = new[] { "The source of the event.", "The source of the event" }.Concat(UnusedPhrase).ToArray();
            internal static readonly string[] SeeStartingPhrase = { "<see cref=", "<seealso cref=", "see <see cref=", "see <seealso cref=", "seealso <see cref=", "seealso <seealso cref=" };
            internal static readonly string[] SeeEndingPhrase = { "/>", "/>.", "/see>", "/see>.", "/seealso>", "/seealso>." };
        }
    }
}