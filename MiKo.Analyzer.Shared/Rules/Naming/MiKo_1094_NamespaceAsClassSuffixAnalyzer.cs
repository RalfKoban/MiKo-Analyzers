using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1094_NamespaceAsClassSuffixAnalyzer : TypeSyntaxNamingAnalyzer
    {
        public const string Id = "MiKo_1094";

        private static readonly Dictionary<string, string> Suffixes = new Dictionary<string, string>
                                                                          {
                                                                              { "Management", "Manager" },
                                                                              { "Handling", "Handler" },
                                                                          };

        public MiKo_1094_NamespaceAsClassSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(string typeName, BaseTypeDeclarationSyntax declaration) => declaration.IsTestClass() is false;

        protected override Diagnostic[] AnalyzeName(string typeName, in SyntaxToken typeNameIdentifier, BaseTypeDeclarationSyntax declaration)
        {
            List<Diagnostic> issues = null;

            foreach (var pair in Suffixes)
            {
                if (typeName.EndsWith(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    var shortened = typeName.AsSpan().WithoutSuffix(pair.Key);

                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(typeNameIdentifier, shortened.ConcatenatedWith(pair.Value)));
                }
            }

            return issues?.ToArray() ?? Array.Empty<Diagnostic>();
        }
    }
}