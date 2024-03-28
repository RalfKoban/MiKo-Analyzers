﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3219_PublicMembersAreNotVirtualAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3219";

        public MiKo_3219_PublicMembersAreNotVirtualAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.TypeKind == TypeKind.Class;

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            var members = symbol.GetMembers();
            var count = members.Length;

            for (var index = 0; index < count; index++)
            {
                var member = members[index];

                if (member.IsVirtual && member.DeclaredAccessibility == Accessibility.Public)
                {
                    if (member is IMethodSymbol method && method.MethodKind != MethodKind.Ordinary)
                    {
                        // ignore non-ordinary methods
                        continue;
                    }

                    var node = member.GetSyntax();
                    var token = node.FirstChildToken(SyntaxKind.VirtualKeyword);

                    var proposal = member.Name;

                    if (proposal.EndsWith(Constants.Core, StringComparison.Ordinal) is false)
                    {
                        proposal += Constants.Core;
                    }

                    yield return Issue(token, proposal);
                }
            }
        }
    }
}