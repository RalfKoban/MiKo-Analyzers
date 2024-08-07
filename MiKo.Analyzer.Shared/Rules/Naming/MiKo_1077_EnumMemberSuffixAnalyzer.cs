﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1077_EnumMemberSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1077";

        private static readonly string[] WrongSuffixes = { "Enum", "EnumValue", "EnumMember" };

        public MiKo_1077_EnumMemberSuffixAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeEnumDeclaration, SyntaxKind.EnumDeclaration);

        private void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is EnumDeclarationSyntax declaration)
            {
                var issues = AnalyzeEnumDeclaration(declaration.Members);

                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeEnumDeclaration(SeparatedSyntaxList<EnumMemberDeclarationSyntax> members)
        {
            foreach (var member in members)
            {
                var identifier = member.Identifier;
                var name = identifier.ValueText;

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var suffix in WrongSuffixes)
                {
                    if (name.EndsWith(suffix, StringComparison.Ordinal))
                    {
                        var betterName = name.WithoutSuffix(suffix);

                        yield return Issue(identifier, betterName, CreateBetterNameProposal(betterName));

                        break;
                    }
                }
            }
        }
    }
}