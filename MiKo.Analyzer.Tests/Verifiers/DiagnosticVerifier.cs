using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

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

        /// <summary>
        /// Gets the identifier of the CSharp analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        protected virtual string GetDiagnosticId() => null;

        #endregion

        #region Verifier wrappers

        protected void Issue_is_reported(string fileContent)
        {
            Assert.Multiple(() =>
                                {
                                    var results = GetDiagnostics(fileContent);
                                    Assert.That(results.Length, Is.GreaterThan(0));

                                    foreach (var result in results)
                                    {
                                        Assert.That(result.Id, Is.EqualTo(GetDiagnosticId()));

                                        foreach (var placeholder in Enumerable.Range(0, 10).Select(_ => "{" + _ + "}"))
                                        {
                                            Assert.That(result.GetMessage(), Does.Not.Contain(placeholder), $"Placeholder {placeholder} found!");
                                        }
                                    }
                                });
        }

        protected void No_issue_is_reported(string fileContent)
        {
            var results = GetDiagnostics(fileContent);

            Assert.That(results, Is.Empty);
        }

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
