using System;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace TestHelper
{
    /// <summary>
    /// Superclass of all Unit Tests for <see cref="DiagnosticAnalyzer"/>s.
    /// </summary>
    public abstract partial class DiagnosticVerifier
    {
        internal static Diagnostic[] GetDiagnostics(string[] sources, DiagnosticAnalyzer[] analyzers) => GetSortedDiagnostics(sources, LanguageNames.CSharp, analyzers);

        /// <summary>
        /// Gets the CSharp analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        /// <returns>
        /// The CSharp analyzer under test.
        /// </returns>
        protected virtual DiagnosticAnalyzer GetObjectUnderTest() => null;

        /// <summary>
        /// Gets the identifier of the CSharp analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        /// <returns>
        /// The identifier of the CSharp analyzer under test.
        /// </returns>
        protected virtual string GetDiagnosticId() => null;

        protected void An_issue_is_reported_for(string fileContent, int violations = 0)
        {
            Assert.Multiple(() =>
                                {
                                    var constraint = (violations == 0) ? Is.GreaterThan(violations) : (IResolveConstraint)Is.EqualTo(violations);

                                    var results = GetDiagnostics(fileContent);

                                    Assert.That(results.Length, constraint, string.Join(Environment.NewLine, results.Select(_ => _.ToString())));

                                    foreach (var result in results)
                                    {
                                        Assert.That(result.Id, Is.EqualTo(GetDiagnosticId()));

                                        var message = result.GetMessage();

                                        Assert.That(message, Does.Not.Contain("tring[]"), "Wrong parameter provided, string array is not converted.");

                                        foreach (var placeholder in Enumerable.Range(0, 10).Select(_ => "{" + _ + "}"))
                                        {
                                            Assert.That(message, Does.Not.Contain(placeholder), $"Placeholder {placeholder} found!");
                                        }
                                    }
                                });
        }

        protected void An_issue_is_reported_for_file(string path, int violations = 0) => An_issue_is_reported_for(File.ReadAllText(path), violations);

        protected void No_issue_is_reported_for(string fileContent)
        {
            var results = GetDiagnostics(fileContent);

            Assert.That(results, Is.Empty, Environment.NewLine + string.Join(Environment.NewLine, results.Select(_ => _.Location + ":" + _)));
        }

        protected void No_issue_is_reported_for_file(string path) => No_issue_is_reported_for(File.ReadAllText(path));

        protected void No_issue_is_reported_for_folder(string path)
        {
            Assert.Multiple(() =>
                                {
                                    foreach (var directory in Directory.EnumerateDirectories(path))
                                    {
                                        No_issue_is_reported_for_folder(directory);
                                    }

                                    foreach (var file in Directory.EnumerateFiles(path, "*.cs"))
                                    {
                                        No_issue_is_reported_for_file(file);
                                    }
                                });
        }

        /// <summary>
        /// Applies a C# <see cref="DiagnosticAnalyzer"/> on the single inputted string and returns the found results.
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on.</param>
        /// <returns>An array of Diagnostics that surfaced in the source code, sorted by Location.</returns>
        protected Diagnostic[] GetDiagnostics(string source) => GetDiagnostics(new[] { source });

        /// <summary>
        /// General method that gets a collection of actual diagnostics found in the source after the analyzer is run,
        /// then verifies each of them.
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the analyzers on.</param>
        /// <returns>An array of Diagnostics that surfaced in the source code, sorted by Location.</returns>
        private Diagnostic[] GetDiagnostics(string[] sources) => GetSortedDiagnostics(sources, LanguageNames.CSharp, GetObjectUnderTest());
    }
}
