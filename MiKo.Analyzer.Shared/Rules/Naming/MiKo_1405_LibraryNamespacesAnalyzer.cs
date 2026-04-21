using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1405_LibraryNamespacesAnalyzer : NamespaceNamingAnalyzer
    {
        public const string Id = "MiKo_1405";

        private static readonly string[] LibraryNamespaces = { "Lib", "Libs", "Library", "Libraries" };

        public MiKo_1405_LibraryNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeNamespaceName(in ReadOnlySpan<SyntaxToken> namespaceNames)
        {
            List<Diagnostic> issues = null;

            var libraryNamespacesLength = LibraryNamespaces.Length;

            foreach (var name in namespaceNames)
            {
                var namespaceName = name.ValueText;

                for (var index = 0; index < libraryNamespacesLength; index++)
                {
                    var libraryNamespace = LibraryNamespaces[index];

                    if (namespaceName.EndsWith(libraryNamespace, StringComparison.OrdinalIgnoreCase))
                    {
                        if (issues is null)
                        {
                            issues = new List<Diagnostic>(1);
                        }

                        issues.Add(Issue(namespaceName, name, libraryNamespace));

                        break;
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}