using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1030_BaseTypePrefixSuffixAnalyzer : TypeSyntaxNamingAnalyzer
    {
        public const string Id = "MiKo_1030";

        public MiKo_1030_BaseTypePrefixSuffixAnalyzer() : base(Id)
        {
        }

        protected override Diagnostic[] AnalyzeName(string typeName, in SyntaxToken typeNameIdentifier, BaseTypeDeclarationSyntax declaration)
        {
            var symbolName = typeName.Without("Abstraction").Replace("BasedOn", "#");

            List<Diagnostic> issues = null;

            foreach (var marker in Constants.Markers.BaseClasses)
            {
                if (symbolName.Contains(marker))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    var betterName = symbolName.Without(Constants.Markers.BaseClasses);

                    issues.Add(Issue(typeNameIdentifier, betterName, marker));
                }
            }

            return issues?.ToArray() ?? Array.Empty<Diagnostic>();
        }
    }
}