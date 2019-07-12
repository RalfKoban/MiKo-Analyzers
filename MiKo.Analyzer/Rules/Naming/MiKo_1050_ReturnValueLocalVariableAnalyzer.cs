﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1050_ReturnValueLocalVariableAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1050";

        private static readonly HashSet<string> AllowedCompleteNames = new HashSet<string>
                                                                           {
                                                                               "result",
                                                                               "results",
                                                                           };

        private static readonly string[] WrongNames = CreateWrongNames(AllowedCompleteNames, "ret", "return", "returning", "retval", "res", "resulting");

        public MiKo_1050_ReturnValueLocalVariableAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDeclarationPattern, SyntaxKind.DeclarationPattern);
            context.RegisterSyntaxNodeAction(AnalyzeForEachStatement, SyntaxKind.ForEachStatement);
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => AnalyzeIdentifiers(semanticModel, identifiers);

        private static string[] CreateWrongNames(IEnumerable<string> values, params string[] additionalValues)
        {
            var results = new HashSet<string>();

            foreach (var value in values.Concat(additionalValues))
            {
                results.Add(value);
                results.Add(value + "_");
            }

            return results.ToArray();
        }

        private static bool ContainsWrongName(string identifier, IEnumerable<string> wrongNames)
        {
            var found = false;

            foreach (var wrongName in wrongNames.Where(_ => identifier.Length >= _.Length))
            {
                if (identifier.Length == wrongName.Length)
                {
                    if (string.Equals(identifier, wrongName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (AllowedCompleteNames.Contains(identifier))
                        {
                            continue;
                        }

                        return true;
                    }
                }
                else
                {
                    var nextCharacter = identifier[wrongName.Length];
                    if (nextCharacter.IsLowerCaseLetter())
                    {
                        continue;
                    }

                    if (identifier.StartsWith(wrongName, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                    }
                }
            }

            return found;
        }

        private IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, IEnumerable<SyntaxToken> identifiers)
        {
            List<Diagnostic> results = null;

            foreach (var identifier in identifiers)
            {
                var name = identifier.ValueText;

                var isWrong = ContainsWrongName(name, WrongNames);
                if (isWrong)
                {
                    var symbol = identifier.GetSymbol(semanticModel);

                    if (results is null)
                    {
                        results = new List<Diagnostic>(1);
                    }

                    results.Add(Issue(symbol, name));
                }
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}