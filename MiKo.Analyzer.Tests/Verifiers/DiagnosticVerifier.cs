using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

//// ncrunch: rdi off
// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
namespace TestHelper
{
    /// <summary>
    /// Superclass of all Unit Tests for <see cref="DiagnosticAnalyzer"/>s.
    /// </summary>
    public abstract partial class DiagnosticVerifier
    {
        private static readonly string[] Placeholders = [.. Enumerable.Range(0, 10).Select(_ => "{" + _ + "}")];

        internal static Diagnostic[] GetDiagnostics(in ReadOnlySpan<string> sources, in LanguageVersion languageVersion, in ReadOnlySpan<DiagnosticAnalyzer> analyzers, in bool profileAnalysis) => GetSortedDiagnostics(sources, languageVersion, analyzers, profileAnalysis);

        /// <summary>
        /// Gets the C# analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        /// <returns>
        /// The C# analyzer under test.
        /// </returns>
        protected virtual DiagnosticAnalyzer GetObjectUnderTest() => null;

        /// <summary>
        /// Gets the identifier of the C# analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        /// <returns>
        /// The identifier of the C# analyzer under test.
        /// </returns>
        protected virtual string GetDiagnosticId() => null;

        protected void An_issue_is_reported_for(string fileContent, in LanguageVersion languageVersion = LanguageVersion.Default) => An_issue_is_reported_for(1, fileContent, languageVersion);

        protected void An_issue_is_reported_for(int violations, string fileContent, LanguageVersion languageVersion = LanguageVersion.Default)
        {
            Assert.Multiple(() =>
                                 {
                                     var results = GetDiagnostics(fileContent, languageVersion);
                                     var resultsLength = results.Length;

                                     Assert.That(resultsLength, Is.EqualTo(violations), string.Join(Environment.NewLine, results.Select(_ => _.ToString())));

                                     var placeholdersLength = Placeholders.Length;

                                     for (var index = 0; index < resultsLength; index++)
                                     {
                                         var result = results[index];

                                         Assert.That(result.Id, Is.EqualTo(GetDiagnosticId()));
                                         Assert.That(result.Id, Is.Not.EqualTo("AD0001")); // This is a programming error

                                         var message = result.GetMessage(null);

                                         Assert.That(message, Does.Not.Contain("tring[]"), "Wrong parameter provided, string array is not converted.");
                                         Assert.That(message, Does.Not.Contain(" -> "), "Wrong parameter provided, Pair.");

                                         for (var placeholderIndex = 0; placeholderIndex < placeholdersLength; placeholderIndex++)
                                         {
                                             var placeholder = Placeholders[placeholderIndex];

                                             Assert.That(message, Does.Not.Contain(placeholder), "Placeholder " + placeholder + " found!");
                                         }
                                     }
                                 });
        }

        protected void An_issue_is_reported_for_file_(string path, in int violations, in LanguageVersion languageVersion = LanguageVersion.Default) => An_issue_is_reported_for(violations, File.ReadAllText(path), languageVersion);

        protected void No_issue_is_reported_for(string fileContent, string message = null, in LanguageVersion languageVersion = LanguageVersion.Default)
        {
            var results = GetDiagnostics(fileContent, languageVersion);

            // performance optimization to avoid the string creation for the message in case we do not have any issue and therefore do not need to report anything
            if (results.Length > 0)
            {
                Assert.That(results, Is.Empty, message ?? Environment.NewLine + string.Join(Environment.NewLine, results.Select(_ => _.Location + ":" + _)));
            }
        }

        protected void No_issue_is_reported_for_file_(string path, in LanguageVersion languageVersion = LanguageVersion.Default) => No_issue_is_reported_for(File.ReadAllText(path), path, languageVersion);

        protected void No_issue_is_reported_for_folder_(string path, LanguageVersion languageVersion = LanguageVersion.Default)
        {
            Assert.Multiple(() =>
                                 {
                                     foreach (var directory in Directory.EnumerateDirectories(path))
                                     {
                                         No_issue_is_reported_for_folder_(directory, languageVersion);
                                     }

                                     foreach (var file in Directory.EnumerateFiles(path, "*.cs"))
                                     {
                                         No_issue_is_reported_for_file_(file, languageVersion);
                                     }
                                 });
        }

        protected IEnumerable<string> Collect_files_having_issues_in_folder_(string path, LanguageVersion languageVersion = LanguageVersion.Default)
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
                var results = GetDiagnostics(File.ReadAllText(file), languageVersion);

                if (results.Length is not 0)
                {
                    yield return file;
                }
            }
        }

        protected IEnumerable<string> Collect_messages_of_issues_in_folder_(string path, LanguageVersion languageVersion = LanguageVersion.Default)
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
                var results = GetDiagnostics(File.ReadAllText(file), languageVersion);

                foreach (var result in results)
                {
                    yield return result.GetMessage(CultureInfo.CurrentCulture) + "[" + file + "]";
                }
            }
        }

        protected IEnumerable<Diagnostic> Collect_issues_in_folder_(string path, LanguageVersion languageVersion = LanguageVersion.Default)
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
                var issues = GetDiagnostics(File.ReadAllText(file), languageVersion);

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
        /// <param name="languageVersion">
        /// The version of the programming language.
        /// </param>
        /// <returns>
        /// An array of Diagnostics that surfaced in the source code, sorted by Location.
        /// </returns>
        protected Diagnostic[] GetDiagnostics(string source, in LanguageVersion languageVersion) => GetDiagnostics([source], languageVersion);

        /// <summary>
        /// General method that gets a collection of actual diagnostics found in the source after the analyzer is run,
        /// then verifies each of them.
        /// </summary>
        /// <param name="sources">
        /// An array of strings to create source documents from to run the analyzers on.
        /// </param>
        /// <param name="languageVersion">
        /// The version of the programming language.
        /// </param>
        /// <returns>
        /// An array of Diagnostics that surfaced in the source code, sorted by Location.
        /// </returns>
        private Diagnostic[] GetDiagnostics(in ReadOnlySpan<string> sources, in LanguageVersion languageVersion) => GetSortedDiagnostics(sources, languageVersion, [GetObjectUnderTest()], false);
    }
}
