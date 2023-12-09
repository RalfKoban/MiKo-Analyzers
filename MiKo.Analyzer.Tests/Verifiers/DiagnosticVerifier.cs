using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

//// ncrunch: collect values off
namespace TestHelper
{
    /// <summary>
    /// Superclass of all Unit Tests for <see cref="DiagnosticAnalyzer"/>s.
    /// </summary>
    public abstract partial class DiagnosticVerifier
    {
        internal static Diagnostic[] GetDiagnostics(IReadOnlyCollection<string> sources, DiagnosticAnalyzer[] analyzers) => GetSortedDiagnostics(sources, LanguageNames.CSharp, analyzers);

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

        protected void An_issue_is_reported_for(string fileContent) => An_issue_is_reported_for(1, fileContent);

        protected void An_issue_is_reported_for(int violations, string fileContent)
        {
            Assert.Multiple(() =>
                                 {
                                     var results = GetDiagnostics(fileContent);

                                     Assert.That(results.Length, Is.EqualTo(violations), string.Join(Environment.NewLine, results.Select(_ => _.ToString())));

                                     foreach (var result in results)
                                     {
                                         Assert.That(result.Id, Is.EqualTo(GetDiagnosticId()));

                                         var message = result.GetMessage(null);

                                         Assert.That(message, Does.Not.Contain("tring[]"), "Wrong parameter provided, string array is not converted.");

                                         foreach (var placeholder in Enumerable.Range(0, 10).Select(_ => "{" + _ + "}"))
                                         {
                                             Assert.That(message, Does.Not.Contain(placeholder), $"Placeholder {placeholder} found!");
                                         }
                                     }
                                 });
        }

        protected void An_issue_is_reported_for_file_(string path, int violations) => An_issue_is_reported_for(violations, File.ReadAllText(path));

        protected void No_issue_is_reported_for(string fileContent, string message = null)
        {
            var results = GetDiagnostics(fileContent);

            Assert.That(results, Is.Empty, message ?? Environment.NewLine + string.Join(Environment.NewLine, results.Select(_ => _.Location + ":" + _)));
        }

        protected void No_issue_is_reported_for_file_(string path) => No_issue_is_reported_for(File.ReadAllText(path), path);

        protected void No_issue_is_reported_for_folder_(string path)
        {
            Assert.Multiple(() =>
                                 {
                                     foreach (var directory in Directory.EnumerateDirectories(path))
                                     {
                                         No_issue_is_reported_for_folder_(directory);
                                     }

                                     foreach (var file in Directory.EnumerateFiles(path, "*.cs"))
                                     {
                                         No_issue_is_reported_for_file_(file);
                                     }
                                 });
        }

        protected IEnumerable<string> Collect_files_having_issues_in_folder_(string path)
        {
            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                var results = Collect_files_having_issues_in_folder_(directory);

                foreach (var result in results)
                {
                    yield return result;
                }
            }

            foreach (var file in Directory.EnumerateFiles(path, "*.cs"))
            {
                var results = GetDiagnostics(File.ReadAllText(file));

                if (results.Any())
                {
                    yield return file;
                }
            }
        }

        protected IEnumerable<string> Collect_messages_of_issues_in_folder_(string path)
        {
            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                var results = Collect_messages_of_issues_in_folder_(directory);

                foreach (var result in results)
                {
                    yield return result;
                }
            }

            foreach (var file in Directory.EnumerateFiles(path, "*.cs"))
            {
                var results = GetDiagnostics(File.ReadAllText(file));

                foreach (var result in results)
                {
                    yield return result.GetMessage(CultureInfo.CurrentCulture) + "[" + file + "]";
                }
            }
        }

        protected IEnumerable<Diagnostic> Collect_issues_in_folder_(string path)
        {
            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                var issues = Collect_issues_in_folder_(directory);

                foreach (var issue in issues)
                {
                    yield return issue;
                }
            }

            foreach (var file in Directory.EnumerateFiles(path, "*.cs"))
            {
                var issues = GetDiagnostics(File.ReadAllText(file));

                foreach (var issue in issues)
                {
                    yield return issue;
                }
            }
        }

        /// <summary>
        /// Applies a C# <see cref="DiagnosticAnalyzer"/> on the single inputted string and returns the found results.
        /// </summary>
        /// <param name="source">
        /// A class in the form of a string to run the analyzer on.
        /// </param>
        /// <returns>
        /// An array of Diagnostics that surfaced in the source code, sorted by Location.
        /// </returns>
        protected Diagnostic[] GetDiagnostics(string source) => GetDiagnostics(new[] { source });

        /// <summary>
        /// General method that gets a collection of actual diagnostics found in the source after the analyzer is run,
        /// then verifies each of them.
        /// </summary>
        /// <param name="sources">
        /// An array of strings to create source documents from to run the analyzers on.
        /// </param>
        /// <returns>
        /// An array of Diagnostics that surfaced in the source code, sorted by Location.
        /// </returns>
        private Diagnostic[] GetDiagnostics(string[] sources) => GetSortedDiagnostics(sources, LanguageNames.CSharp, GetObjectUnderTest());
    }
}
