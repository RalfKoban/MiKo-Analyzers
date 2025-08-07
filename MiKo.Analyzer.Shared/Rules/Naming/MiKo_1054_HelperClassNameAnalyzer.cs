﻿using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1054_HelperClassNameAnalyzer : TypeSyntaxNamingAnalyzer
    {
        public const string Id = "MiKo_1054";

        private const string CorrectName = "Utilization";

        private const string SpecialNameHandle = "Handle";
        private const string SpecialNameHandler = "Handler";

        private static readonly string[] WrongNames = { "Helper", "Util", "Misc" };

        // sorted by intent so that the best match is found until a more generic is found
        private static readonly string[] WrongNamesForConcreteLookup = { "Helpers", "Helper", "Miscellaneous", "Misc", "Utils", "Utility", "Utilities", "Util" };

        public MiKo_1054_HelperClassNameAnalyzer() : base(Id)
        {
        }

        protected override Diagnostic[] AnalyzeName(string typeName, in SyntaxToken typeNameIdentifier, BaseTypeDeclarationSyntax declaration)
        {
            if (typeName.Contains(CorrectName))
            {
                return Array.Empty<Diagnostic>();
            }

            if (typeName.ContainsAny(WrongNames))
            {
                var wrongName = WrongNamesForConcreteLookup.First(_ => typeName.Contains(_, StringComparison.OrdinalIgnoreCase));

                var proposal = FindBetterName(typeName.AsSpan(), wrongName);

                return new[] { Issue(typeNameIdentifier, proposal, wrongName) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(in ReadOnlySpan<char> typeName, string wrongName)
        {
            if (typeName.Length > SpecialNameHandle.Length && typeName.StartsWith(SpecialNameHandle, StringComparison.Ordinal))
            {
                return typeName.WithoutSuffix(wrongName)
                               .Slice(SpecialNameHandle.Length)
                               .ConcatenatedWith(SpecialNameHandler);
            }

            return typeName.WithoutSuffix(wrongName).ToString();
        }
    }
}