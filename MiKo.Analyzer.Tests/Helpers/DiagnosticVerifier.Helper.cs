﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml;

using log4net;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

using MiKoSolutions.Analyzers.Rules;

using Moq;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable CheckNamespace
namespace TestHelper
{
    /// <summary>
    /// Class for turning strings into documents and getting the diagnostics on them.
    /// All methods are static.
    /// </summary>
    public abstract partial class DiagnosticVerifier
    {
        private const string TestProjectName = "MiKoSolutions.Analyzers.AdHoc.TestProject";

        private static readonly MetadataReference AspNetCoreMvcAbstractionsReference = MetadataReference.CreateFromFile(typeof(IModelBinder).Assembly.Location);
        private static readonly MetadataReference AttributeReference = MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location);
        private static readonly MetadataReference AttributeTargetsReference = MetadataReference.CreateFromFile(typeof(AttributeTargets).Assembly.Location);
        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);
        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private static readonly MetadataReference DescriptionAttributeReference = MetadataReference.CreateFromFile(typeof(DescriptionAttribute).Assembly.Location);
        private static readonly MetadataReference LogForNetReference = MetadataReference.CreateFromFile(typeof(ILog).Assembly.Location);
        private static readonly MetadataReference MiKoAnalyzersReference = MetadataReference.CreateFromFile(typeof(Analyzer).Assembly.Location);
        private static readonly MetadataReference MiKoAnalyzersTestsReference = MetadataReference.CreateFromFile(typeof(DiagnosticVerifier).Assembly.Location);
        private static readonly MetadataReference MoqReference = MetadataReference.CreateFromFile(typeof(IMock<>).Assembly.Location);
        private static readonly MetadataReference NUnitLegacyReference = MetadataReference.CreateFromFile(typeof(DirectoryAssert).Assembly.Location);
        private static readonly MetadataReference NUnitReference = MetadataReference.CreateFromFile(typeof(Assert).Assembly.Location);
        private static readonly MetadataReference SystemReference = MetadataReference.CreateFromFile(typeof(Console).Assembly.Location);
        private static readonly MetadataReference SystemCompositionReference = MetadataReference.CreateFromFile(typeof(ImportAttribute).Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        private static readonly MetadataReference SystemLinqReference = MetadataReference.CreateFromFile(typeof(Expression).Assembly.Location);
        private static readonly MetadataReference SystemRuntimeReference = MetadataReference.CreateFromFile(typeof(DataContractAttribute).Assembly.Location); // needed also for other attributes
        private static readonly MetadataReference SystemTextReference = MetadataReference.CreateFromFile(typeof(Regex).Assembly.Location);
        private static readonly MetadataReference SystemTextJsonReference = MetadataReference.CreateFromFile(typeof(JsonConstructorAttribute).Assembly.Location);
        private static readonly MetadataReference SystemWindowsInputReference = MetadataReference.CreateFromFile(typeof(ICommand).Assembly.Location);
        private static readonly MetadataReference SystemXmlReference = MetadataReference.CreateFromFile(typeof(XmlNode).Assembly.Location);

        /// <summary>
        /// Avoids error <c>CS0012: The type 'MulticastDelegate' is defined in an assembly that is not referenced. You must add a reference to assembly 'netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'</c>.
        /// <para/>
        /// Needed by some tests as the code references types from .NET standard 2.0.
        /// </summary>
        private static readonly MetadataReference NetStandardReference = MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location);

        private static readonly MetadataReference SystemRuntimeNetStandardReference = MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=0.0.0.0").Location);

        private static readonly MetadataReference[] References =
                                                                 [
                                                                     AspNetCoreMvcAbstractionsReference,
                                                                     AttributeReference,
                                                                     AttributeTargetsReference,
                                                                     CodeAnalysisReference,
                                                                     CorlibReference,
                                                                     CSharpSymbolsReference,
                                                                     DescriptionAttributeReference,
                                                                     LogForNetReference,
                                                                     MiKoAnalyzersReference,
                                                                     MiKoAnalyzersTestsReference,
                                                                     MoqReference,
                                                                     NetStandardReference,
                                                                     NUnitLegacyReference,
                                                                     NUnitReference,
                                                                     SystemCompositionReference,
                                                                     SystemCoreReference,
                                                                     SystemLinqReference,
                                                                     SystemReference,
                                                                     SystemRuntimeNetStandardReference,
                                                                     SystemRuntimeReference,
                                                                     SystemTextJsonReference,
                                                                     SystemTextReference,
                                                                     SystemWindowsInputReference,
                                                                     SystemXmlReference,
                                                                 ];

        /// <summary>
        /// Given an analyzer and a document to apply it to, run the analyzers and gather an array of diagnostics found in it.
        /// The returned diagnostics are then ordered by location in the source document.
        /// </summary>
        /// <param name="analyzer">
        /// The analyzer to run on the documents.
        /// </param>
        /// <param name="document">
        /// The Document that the analyzers will be run on.
        /// </param>
        /// <returns>
        /// An array of Diagnostics that surfaced in the source code, sorted by Location.
        /// </returns>
        protected static Diagnostic[] GetSortedDiagnosticsFromDocument(DiagnosticAnalyzer analyzer, Document document) => GetSortedDiagnosticsFromDocuments([analyzer], [document], false);

        /// <summary>
        /// Given an analyzer and a document to apply it to, run the analyzers and gather an array of diagnostics found in it.
        /// The returned diagnostics are then ordered by location in the source document.
        /// </summary>
        /// <param name="analyzers">
        /// The analyzers to run on the documents.
        /// </param>
        /// <param name="documents">
        /// The Documents that the analyzers will be run on.
        /// </param>
        /// <param name="profileAnalysis">
        /// <see langword="true"/> to collect and save profiling data; otherwise, <see langword="false"/>.
        /// </param>
        /// <returns>
        /// An array of Diagnostics that surfaced in the source code, sorted by Location.
        /// </returns>
        protected static Diagnostic[] GetSortedDiagnosticsFromDocuments(in ReadOnlySpan<DiagnosticAnalyzer> analyzers, in ReadOnlySpan<Document> documents, in bool profileAnalysis)
        {
            var projects = new HashSet<Project>();

            foreach (var document in documents)
            {
                projects.Add(document.Project);
            }

            var documentsLength = documents.Length;
            var diagnostics = new List<Diagnostic>();

            foreach (var project in projects)
            {
                if (profileAnalysis)
                {
                    JetBrains.Profiler.Api.MeasureProfiler.StartCollectingData();
                }

                var compilation = project.GetCompilationAsync().Result;
                var compilationWithAnalyzers = compilation.WithAnalyzers([..analyzers]);
                var diags = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;
                var diagsLength = diags.Length;

                if (profileAnalysis)
                {
                    JetBrains.Profiler.Api.MeasureProfiler.SaveData();
                }

                if (diagsLength > 0)
                {
                    for (var index = 0; index < diagsLength; index++)
                    {
                        var diag = diags[index];

                        if (diag.Location == Location.None || diag.Location.IsInMetadata)
                        {
                            diagnostics.Add(diag);
                        }
                        else
                        {
                            for (var documentIndex = 0; documentIndex < documentsLength; documentIndex++)
                            {
                                var document = documents[documentIndex];
                                var tree = document.GetSyntaxTreeAsync().Result;

                                if (tree == diag.Location.SourceTree)
                                {
                                    diagnostics.Add(diag);
                                }
                            }
                        }
                    }
                }
            }

            return SortDiagnostics(diagnostics);
        }

        /// <summary>
        /// Create a Document from a string through creating a project that contains it.
        /// </summary>
        /// <param name="source">
        /// Classes in the form of a string.
        /// </param>
        /// <param name="languageVersion">
        /// The version of the programming language.
        /// </param>
        /// <returns>
        /// A Document created from the source string.
        /// </returns>
        protected static Document CreateDocument(string source, in LanguageVersion languageVersion)
        {
            return CreateProject([source], languageVersion).Documents.First();
        }

        /// <summary>
        /// Given classes in the form of strings, their language, and an <see cref="DiagnosticAnalyzer"/> to apply to it, return the diagnostics found in the string after converting it to a document.
        /// </summary>
        /// <param name="sources">
        /// Classes in the form of strings.
        /// </param>
        /// <param name="languageVersion">
        /// The version of the programming language.
        /// </param>
        /// <param name="analyzers">
        /// The analyzers to be run on the sources.
        /// </param>
        /// <param name="profileAnalysis">
        /// <see langword="true"/> to collect and save profiling data; otherwise, <see langword="false"/>.
        /// </param>
        /// <returns>
        /// An array of <see cref="Diagnostic"/>s that surfaced in the source code, sorted by <see cref="Diagnostic.Location"/>.
        /// </returns>
        private static Diagnostic[] GetSortedDiagnostics(in ReadOnlySpan<string> sources, in LanguageVersion languageVersion, in ReadOnlySpan<DiagnosticAnalyzer> analyzers, in bool profileAnalysis)
        {
            return GetSortedDiagnosticsFromDocuments(analyzers, GetDocuments(sources, languageVersion), profileAnalysis);
        }

        /// <summary>
        /// Sort diagnostics by location in source document.
        /// </summary>
        /// <param name="diagnostics">
        /// The list of Diagnostics to be sorted.
        /// </param>
        /// <returns>
        /// An array of <see cref="Diagnostic"/>s in order of <see cref="Diagnostic.Location"/>.
        /// </returns>
        private static Diagnostic[] SortDiagnostics(List<Diagnostic> diagnostics)
        {
            if (diagnostics.Count == 0)
            {
                return [];
            }

            return [.. diagnostics.OrderBy(_ => _.Location.SourceSpan.Start)];
        }

        /// <summary>
        /// Given an array of strings as sources and a language, turn them into a project and return the documents.
        /// </summary>
        /// <param name="sources">
        /// Classes in the form of strings.
        /// </param>
        /// <param name="languageVersion">
        /// The version of the programming language.
        /// </param>
        /// <returns>
        /// The <see cref="Document"/>s produced from the sources.
        /// </returns>
        private static Document[] GetDocuments(in ReadOnlySpan<string> sources, in LanguageVersion languageVersion)
        {
            var project = CreateProject(sources, languageVersion);
            var documents = project.Documents.ToArray();

            if (sources.Length != documents.Length)
            {
                ThrowInvalidOperation("Amount of sources did not match amount of Documents created");
            }

            return documents;
        }

        private static void ThrowInvalidOperation(string message) => throw new InvalidOperationException(message);

        /// <summary>
        /// Create a project using the inputted strings as sources.
        /// </summary>
        /// <param name="sources">
        /// Classes in the form of strings.
        /// </param>
        /// <param name="languageVersion">
        /// The version of the programming language.
        /// </param>
        /// <returns>
        /// A Project created out of the Documents created from the source strings.
        /// </returns>
        private static Project CreateProject(in ReadOnlySpan<string> sources, in LanguageVersion languageVersion)
        {
            var projectId = ProjectId.CreateNewId(debugName: TestProjectName);
            var projectInfo = ProjectInfo.Create(projectId, VersionStamp.Default, TestProjectName, TestProjectName, LanguageNames.CSharp, parseOptions: CSharpParseOptions.Default.WithLanguageVersion(languageVersion));

            var solution = new AdhocWorkspace().CurrentSolution
                                               .AddProject(projectInfo)
                                               .AddMetadataReferences(projectId, References);

            var length = sources.Length;

            if (length == 1)
            {
                const string FileName = "Test.cs";
                var documentId = DocumentId.CreateNewId(projectId, debugName: FileName);

                solution = solution.AddDocument(documentId, FileName, SourceText.From(sources[0]));
            }
            else
            {
                for (var index = 0; index < length; index++)
                {
                    var source = sources[index];

                    var newFileName = "Test" + index + ".cs";
                    var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);

                    solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                }
            }

            return solution.GetProject(projectId);
        }
    }
}