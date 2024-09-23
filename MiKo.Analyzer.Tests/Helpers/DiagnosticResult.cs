using Microsoft.CodeAnalysis;

//// ncrunch: rdi off
// ReSharper disable CheckNamespace
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TestHelper
{
    /// <summary>
    /// Stores information about a <see cref="Diagnostic"/> appearing in a source.
    /// </summary>
    public struct DiagnosticResult
    {
        private DiagnosticResultLocation[] locations;

        public DiagnosticResultLocation[] Locations
        {
            get => locations ??= [];
            set => locations = value;
        }

        public DiagnosticSeverity Severity { get; set; }

        public string Id { get; set; }

        public string Message { get; set; }

        public string Path => Locations.Length > 0 ? Locations[0].Path : string.Empty;

        public int Line => Locations.Length > 0 ? Locations[0].Line : -1;

        public int Column => Locations.Length > 0 ? Locations[0].Column : -1;
    }
}