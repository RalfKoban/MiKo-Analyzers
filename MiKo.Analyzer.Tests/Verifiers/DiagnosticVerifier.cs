using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TestHelper
{
    /// <summary>
    /// Superclass of all Unit Tests for <see cref="DiagnosticAnalyzer"/>s.
    /// </summary>
    public abstract partial class DiagnosticVerifier
    {
        #region To be implemented by Test classes

        /// <summary>
        /// Gets the CSharp analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        protected virtual DiagnosticAnalyzer GetObjectUnderTest() => null;

        #endregion

        #region Verifier wrappers

        /// <summary>
        /// Applies a C# <see cref="DiagnosticAnalyzer"/> on the single inputted string and returns the found results.
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on</param>
        protected Diagnostic[] GetDiagnostics(string source) => GetDiagnostics(new[] { source });

        /// <summary>
        /// General method that gets a collection of actual diagnostics found in the source after the analyzer is run,
        /// then verifies each of them.
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the analyzers on</param>
        private Diagnostic[] GetDiagnostics(string[] sources) => GetSortedDiagnostics(sources, LanguageNames.CSharp, GetObjectUnderTest());

        #endregion
    }
}
