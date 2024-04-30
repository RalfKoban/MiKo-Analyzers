using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Formatting;

using NUnit.Framework;

//// ncrunch: rdi off
namespace TestHelper
{
    /// <summary>
    /// Superclass of all Unit tests made for diagnostics with codefixes.
    /// Contains methods to verify correctness of codefixes.
    /// </summary>
    public abstract partial class CodeFixVerifier : DiagnosticVerifier
    {
        //// private static int s_testLimit = int.MaxValue;

        protected static int TestLimit
        {
            get
            {
                // if (s_testLimit < 0)
                // {
                //     // see variable in appveyor.yml; used to limit number of tests as otherwise the test run takes too much time
                //     var environmentVariable = Environment.GetEnvironmentVariable("APP_VEYOR", EnvironmentVariableTarget.Process);
                //
                //     s_testLimit = bool.TryParse(environmentVariable, out var value) && value ? 1000 : int.MaxValue;
                // }
                //
                // return s_testLimit;
                return int.MaxValue;
            }
        }

        /// <summary>
        /// Returns the codefix being tested (C#) - to be implemented in non-abstract class.
        /// </summary>
        /// <returns>
        /// The CodeFixProvider to be used for CSharp code.
        /// </returns>
        protected virtual CodeFixProvider GetCSharpCodeFixProvider() => null;

        protected void Codefix_causes_no_exception_in_folder_(string path)
        {
            Assert.Multiple(() =>
                                 {
                                     foreach (var file in Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories))
                                     {
                                         var oldSource = File.ReadAllText(file);

                                         var issues = GetDiagnostics(oldSource);

                                         if (issues.Any())
                                         {
                                             try
                                             {
                                                 Assert.Multiple(() => VerifyCSharpFix(oldSource, oldSource, allowNewCompilerDiagnostics: true, assertResult: false));
                                             }
                                             catch (Exception ex)
                                             {
                                                 Assert.Fail($"'{file}' failed with {ex}");
                                             }
                                         }
                                     }
                                 });
        }

        /// <summary>
        /// Called to test a C# codefix when applied on the inputted string as a source.
        /// </summary>
        /// <param name="oldSource">
        /// A class in the form of a string before the CodeFix was applied to it.
        /// </param>
        /// <param name="newSource">
        /// A class in the form of a string after the CodeFix was applied to it.
        /// </param>
        /// <param name="codeFixIndex">
        /// Index determining which codefix to apply if there are multiple.
        /// </param>
        /// <param name="allowNewCompilerDiagnostics">
        /// A bool controlling whether or not the test will fail if the CodeFix introduces other warnings after being applied.
        /// </param>
        /// <param name="assertResult">
        /// A bool controlling whether or not the test will assert the result of the CodeFix after being applied.
        /// </param>
        protected void VerifyCSharpFix(string oldSource, string newSource, int? codeFixIndex = null, bool allowNewCompilerDiagnostics = false, bool assertResult = true)
        {
            VerifyFix(GetObjectUnderTest(), GetCSharpCodeFixProvider(), oldSource, newSource, codeFixIndex, allowNewCompilerDiagnostics, assertResult);
        }

        /// <summary>
        /// General verifier for codefixes.
        /// Creates a Document from the source string, then gets diagnostics on it and applies the relevant codefixes.
        /// Then gets the string after the codefix is applied and compares it with the expected result.
        /// Note: If any codefix causes new diagnostics to show up, the test fails unless allowNewCompilerDiagnostics is set to true.
        /// </summary>
        /// <param name="analyzer">
        /// The analyzer to be applied to the source code.
        /// </param>
        /// <param name="codeFixProvider">
        /// The codefix to be applied to the code wherever the relevant Diagnostic is found.
        /// </param>
        /// <param name="oldSource">
        /// A class in the form of a string before the CodeFix was applied to it.
        /// </param>
        /// <param name="newSource">
        /// A class in the form of a string after the CodeFix was applied to it.
        /// </param>
        /// <param name="codeFixIndex">
        /// Index determining which codefix to apply if there are multiple.
        /// </param>
        /// <param name="allowNewCompilerDiagnostics">
        /// A bool controlling whether or not the test will fail if the CodeFix introduces other warnings after being applied.
        /// </param>
        /// <param name="assertResult">
        /// A bool controlling whether or not the test will assert the result of the CodeFix after being applied.
        /// </param>
        private static void VerifyFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFixProvider, string oldSource, string newSource, int? codeFixIndex, bool allowNewCompilerDiagnostics, bool assertResult)
        {
            Assert.That(analyzer, Is.Not.Null, "Missing Analyzer");
            Assert.That(codeFixProvider, Is.Not.Null, "Missing CodeFixProvider");

            var document = CreateDocument(oldSource);
            var analyzerDiagnostics = GetSortedDiagnosticsFromDocument(analyzer, document);
            var compilerDiagnostics = GetCompilerDiagnostics(document);
            var attempts = analyzerDiagnostics.Length;

            for (var i = 0; i < attempts; ++i)
            {
                var actions = new List<CodeAction>();
                var context = new CodeFixContext(document, analyzerDiagnostics[0], (a, _) => actions.Add(a), CancellationToken.None);
                codeFixProvider.RegisterCodeFixesAsync(context).Wait();

                if (actions.Any() is false)
                {
                    break;
                }

                if (codeFixIndex != null)
                {
                    document = ApplyFix(document, actions[(int)codeFixIndex]);

                    break;
                }

                document = ApplyFix(document, actions[0]);
                analyzerDiagnostics = GetSortedDiagnosticsFromDocument(analyzer, document);

                var newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, GetCompilerDiagnostics(document));

                // check if applying the code fix introduced any new compiler diagnostics
                if (allowNewCompilerDiagnostics is false && newCompilerDiagnostics.Any())
                {
                    // Format and get the compiler diagnostics again so that the locations make sense in the output
                    document = document.WithSyntaxRoot(Formatter.Format(document.GetSyntaxRootAsync().Result, Formatter.Annotation, document.Project.Solution.Workspace));
                    newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, GetCompilerDiagnostics(document));

                    Assert.Fail($@"Fix introduced new compiler diagnostics:
{string.Join("\r\n", newCompilerDiagnostics.Select(d => d.ToString()))}

New document:
{document.GetSyntaxRootAsync().Result.ToFullString()}
");
                }

                // check if there are analyzer diagnostics left after the code fix
                if (analyzerDiagnostics.Any() is false)
                {
                    break;
                }
            }

            if (assertResult)
            {
                // after applying all of the code fixes, compare the resulting string to the inputted one
                var actual = GetStringFromDocument(document);

                var message = @"Fix created unexpected document.
New document:
################################################
" + actual + @"
################################################";

                Assert.That(actual, Is.EqualTo(newSource), message);
            }
        }
    }
}
