using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1405_LibraryNamespacesAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1405";

        private static readonly string[] LibraryNamespaces = { "Lib", "Libs", "Library", "Libraries" };

        public MiKo_1405_LibraryNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(IEnumerable<SyntaxToken> names)
        {
            var libraryNamespacesLength = LibraryNamespaces.Length;

            foreach (var name in names)
            {
                var namespaceName = name.ValueText;

                for (var index = 0; index < libraryNamespacesLength; index++)
                {
                    var libraryNamespace = LibraryNamespaces[index];

                    if (namespaceName.EndsWith(libraryNamespace, StringComparison.Ordinal))
                    {
                        yield return Issue(namespaceName, name, libraryNamespace);

                        break;
                    }
                }
            }
        }
    }
}