using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3082_UsePatternMatchingForBooleanEqualsExpressionAnalyzer : UsePatternMatchingForEqualsExpressionAnalyzer
    {
        public const string Id = "MiKo_3082";

        private static readonly HashSet<SyntaxKind> BooleanValues = new HashSet<SyntaxKind>
                                                                        {
                                                                            SyntaxKind.TrueLiteralExpression,
                                                                            SyntaxKind.FalseLiteralExpression,
                                                                        };

        public MiKo_3082_UsePatternMatchingForBooleanEqualsExpressionAnalyzer() : base(Id)
        {
        }

        protected override bool IsResponsibleNode(SyntaxKind kind) => BooleanValues.Contains(kind);
    }
}