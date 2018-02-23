namespace MiKoSolutions.Analyzers
{
    internal static class Constants
    {
        internal static readonly string[] EntityMarkers = { "Model", "Models", "model", "models" };
        internal static readonly string[] ViewModelMarkers = { "ViewModel", "ViewModels", "viewModel", "viewModels" };
        internal static readonly string[] SpecialModelMarkers = { "Modeless", "modeless", "ModeLess", "modeLess" };
        internal static readonly string[] CollectionMarkers = { "List", "Dictionary", "ObservableCollection", "Collection", "Array", "HashSet", "list", "dictionary", "observableCollection", "collection", "array", "hashSet" };
    }
}