using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Input;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

using MiKoSolutions.Analyzers.Rules;

using NUnit.Framework;

using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace TestHelper
{
    /// <summary>
    /// Class for turning strings into documents and getting the diagnostics on them.
    /// All methods are static.
    /// </summary>
    public abstract partial class DiagnosticVerifier
    {
        internal const string DefaultFilePathPrefix = "Test";
        internal const string CSharpDefaultFileExt = "cs";
        internal const string VisualBasicDefaultExt = "vb";
        internal const string TestProjectName = "MiKoSolutions.Analyzers.AdHoc.TestProject";

        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        private static readonly MetadataReference SystemCompositionReference = MetadataReference.CreateFromFile(typeof(ImportAttribute).Assembly.Location);
        private static readonly MetadataReference SystemLinqReference = MetadataReference.CreateFromFile(typeof(Expression).Assembly.Location);
        private static readonly MetadataReference SystemRuntimeReference = MetadataReference.CreateFromFile(typeof(DataContractAttribute).Assembly.Location); // needed also for other attributes
        private static readonly MetadataReference SystemTextReference = MetadataReference.CreateFromFile(typeof(Regex).Assembly.Location);
        private static readonly MetadataReference SystemWindowsInputReference = MetadataReference.CreateFromFile(typeof(ICommand).Assembly.Location);
        private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);
        private static readonly MetadataReference NUnitReference = MetadataReference.CreateFromFile(typeof(Assert).Assembly.Location);
        private static readonly MetadataReference MiKoAnalyzersReference = MetadataReference.CreateFromFile(typeof(Analyzer).Assembly.Location);
        private static readonly MetadataReference MiKoAnalyzersTestsReference = MetadataReference.CreateFromFile(typeof(DiagnosticVerifier).Assembly.Location);
        private static readonly MetadataReference AttributeReference = MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location);
        private static readonly MetadataReference AttributeTargetsReference = MetadataReference.CreateFromFile(typeof(AttributeTargets).Assembly.Location);
        private static readonly MetadataReference DescriptionAttributeReference = MetadataReference.CreateFromFile(typeof(DescriptionAttribute).Assembly.Location);
        private static readonly MetadataReference AspNetCoreMvcAbstractionsReference = MetadataReference.CreateFromFile(typeof(IModelBinder).Assembly.Location);

        /// <summary>
        /// Avoids error CS0012: The type 'MulticastDelegate' is defined in an assembly that is not referenced. You must add a reference to assembly 'netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'.}
        /// Needed by some tests as the code references types from .NET standard 2.0.
        /// </summary>
        private static readonly MetadataReference NetStandardReference = MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location);

        private static readonly MetadataReference SystemRuntimeNetStandardReference = MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=0.0.0.0").Location);

        /// <summary>
        /// Given an analyzer and a document to apply it to, run the analyzers and gather an array of diagnostics found in it.
        /// The returned diagnostics are then ordered by location in the source document.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the documents.</param>
        /// <param name="document">The Document that the analyzers will be run on.</param>
        /// <returns>An array of Diagnostics that surfaced in the source code, sorted by Location.</returns>
        protected static Diagnostic[] GetSortedDiagnosticsFromDocument(DiagnosticAnalyzer analyzer, Document document) => GetSortedDiagnosticsFromDocuments(new[] { analyzer }, new[] { document });

        /// <summary>
        /// Given an analyzer and a document to apply it to, run the analyzers and gather an array of diagnostics found in it.
        /// The returned diagnostics are then ordered by location in the source document.
        /// </summary>
        /// <param name="analyzers">The analyzers to run on the documents.</param>
        /// <param name="documents">The Documents that the analyzers will be run on.</param>
        /// <returns>An array of Diagnostics that surfaced in the source code, sorted by Location.</returns>
        protected static Diagnostic[] GetSortedDiagnosticsFromDocuments(DiagnosticAnalyzer[] analyzers, Document[] documents)
        {
            var projects = new HashSet<Project>();
            foreach (var document in documents)
            {
                projects.Add(document.Project);
            }

            var diagnostics = new List<Diagnostic>();
            foreach (var project in projects)
            {
                var compilationWithAnalyzers = project.GetCompilationAsync().Result.WithAnalyzers(ImmutableArray.Create(analyzers));
                var diags = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;
                foreach (var diag in diags)
                {
                    if (diag.Location == Location.None || diag.Location.IsInMetadata)
                    {
                        diagnostics.Add(diag);
                    }
                    else
                    {
                        for (var i = 0; i < documents.Length; i++)
                        {
                            var document = documents[i];
                            var tree = document.GetSyntaxTreeAsync().Result;

                            if (tree == diag.Location.SourceTree)
                            {
                                diagnostics.Add(diag);
                            }
                        }
                    }
                }
            }

            var results = SortDiagnostics(diagnostics);
            diagnostics.Clear();

            return results;
        }

        /// <summary>
        /// Create a Document from a string through creating a project that contains it.
        /// </summary>
        /// <param name="source">Classes in the form of a string.</param>
        /// <param name="language">The language the source code is in.</param>
        /// <returns>A Document created from the source string.</returns>
        protected static Document CreateDocument(string source, string language = LanguageNames.CSharp)
        {
            return CreateProject(new[] { source }, language).Documents.First();
        }

        /// <summary>
        /// Given classes in the form of strings, their language, and an <see cref="DiagnosticAnalyzer"/> to apply to it, return the diagnostics found in the string after converting it to a document.
        /// </summary>
        /// <param name="sources">Classes in the form of strings.</param>
        /// <param name="language">The language the source classes are in.</param>
        /// <param name="analyzers">The analyzers to be run on the sources.</param>
        /// <returns>An array of <see cref="Diagnostic"/>s that surfaced in the source code, sorted by <see cref="Diagnostic.Location"/>.</returns>
        private static Diagnostic[] GetSortedDiagnostics(string[] sources, string language, params DiagnosticAnalyzer[] analyzers)
        {
            return GetSortedDiagnosticsFromDocuments(analyzers, GetDocuments(sources, language));
        }

        /// <summary>
        /// Sort diagnostics by location in source document.
        /// </summary>
        /// <param name="diagnostics">The list of Diagnostics to be sorted.</param>
        /// <returns>An array of <see cref="Diagnostic"/>s in order of <see cref="Diagnostic.Location"/>.</returns>
        private static Diagnostic[] SortDiagnostics(IEnumerable<Diagnostic> diagnostics)
        {
            return diagnostics.OrderBy(_ => _.Location.SourceSpan.Start).ToArray();
        }

        /// <summary>
        /// Given an array of strings as sources and a language, turn them into a project and return the documents.
        /// </summary>
        /// <param name="sources">Classes in the form of strings.</param>
        /// <param name="language">The language the source code is in.</param>
        /// <returns>The <see cref="Document"/>s produced from the sources.</returns>
        private static Document[] GetDocuments(string[] sources, string language)
        {
            if (language != LanguageNames.CSharp && language != LanguageNames.VisualBasic)
            {
                throw new ArgumentException("Unsupported Language");
            }

            var project = CreateProject(sources, language);
            var documents = project.Documents.ToArray();

            if (sources.Length != documents.Length)
            {
                throw new InvalidOperationException("Amount of sources did not match amount of Documents created");
            }

            return documents;
        }

        /// <summary>
        /// Create a project using the inputted strings as sources.
        /// </summary>
        /// <param name="sources">Classes in the form of strings.</param>
        /// <param name="language">The language the source code is in.</param>
        /// <returns>A Project created out of the Documents created from the source strings.</returns>
        private static Project CreateProject(string[] sources, string language = LanguageNames.CSharp)
        {
            var fileNamePrefix = DefaultFilePathPrefix;
            var fileExt = language == LanguageNames.CSharp ? CSharpDefaultFileExt : VisualBasicDefaultExt;

            var projectId = ProjectId.CreateNewId(debugName: TestProjectName);

            var solution = new AdhocWorkspace().CurrentSolution
                                               .AddProject(projectId, TestProjectName, TestProjectName, language)
                                               .AddMetadataReference(projectId, CorlibReference)
                                               .AddMetadataReference(projectId, SystemCoreReference)
                                               .AddMetadataReference(projectId, SystemCompositionReference)
                                               .AddMetadataReference(projectId, SystemRuntimeReference)
                                               .AddMetadataReference(projectId, SystemWindowsInputReference)
                                               .AddMetadataReference(projectId, AttributeReference)
                                               .AddMetadataReference(projectId, AttributeTargetsReference)
                                               .AddMetadataReference(projectId, DescriptionAttributeReference)
                                               .AddMetadataReference(projectId, AspNetCoreMvcAbstractionsReference)
                                               .AddMetadataReference(projectId, CSharpSymbolsReference)
                                               .AddMetadataReference(projectId, CodeAnalysisReference)
                                               .AddMetadataReference(projectId, NUnitReference)
                                               .AddMetadataReference(projectId, MiKoAnalyzersReference)
                                               .AddMetadataReference(projectId, MiKoAnalyzersTestsReference)
                                               .AddMetadataReference(projectId, NetStandardReference)
                                               .AddMetadataReference(projectId, SystemRuntimeNetStandardReference)
                                               .AddMetadataReference(projectId, SystemLinqReference)
                                               .AddMetadataReference(projectId, SystemTextReference);

            var count = 0;
            foreach (var source in sources)
            {
                var newFileName = fileNamePrefix + count + "." + fileExt;
                var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                count++;
            }

            return solution.GetProject(projectId);
        }
    }
}