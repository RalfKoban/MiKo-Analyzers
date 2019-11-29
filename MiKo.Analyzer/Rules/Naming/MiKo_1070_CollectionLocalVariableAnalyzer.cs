﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1070_CollectionLocalVariableAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1070";

        public MiKo_1070_CollectionLocalVariableAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDeclarationPattern, SyntaxKind.DeclarationPattern);
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsEnumerable();

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            foreach (var identifier in identifiers)
            {
                var name = identifier.ValueText;

                // skip all short names
                if (name.Length <= 1
                    || name == Constants.LambdaIdentifiers.Fallback
                    || name == Constants.LambdaIdentifiers.Fallback2
                    || name == "result")
                {
                    continue;
                }

                if (name.Last().IsNumber())
                {
                    // TODO RKN: Check for numbers at the end (get rid of them)
                    continue;
                }

                // TODO RKN: Check for numbers at the end (get rid of them)
                var pluralName = name.EndsWithAny(Constants.Markers.Collections)
                                     ? GetPluralName(name, StringComparison.OrdinalIgnoreCase, Constants.Markers.Collections)
                                     : GetPluralName(name);

                if (pluralName is null)
                {
                    continue;
                }

                if (pluralName != name)
                {
                    yield return Issue(name, identifier.GetLocation(), pluralName);
                }
            }
        }
    }
}